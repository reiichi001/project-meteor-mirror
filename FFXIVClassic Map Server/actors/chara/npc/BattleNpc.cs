using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.chara;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.actors.chara.ai.utils;

namespace FFXIVClassic_Map_Server.Actors
{
    [Flags]
    enum AggroType
    {
        None = 0x00,
        Sight = 0x01,
        Scent = 0x02,
        LowHp = 0x04,
        IgnoreLevelDifference = 0x08
    }

    class BattleNpc : Npc
    {
        public HateContainer hateContainer;
        public AggroType aggroType;
        public bool neutral;
        private uint despawnTime;
        private uint spawnDistance;


        public BattleNpc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot,
            ushort actorState, uint animationId, string customDisplayName)
            : base(actorNumber, actorClass, uniqueId, spawnedArea, posX, posY, posZ, rot, actorState, animationId, customDisplayName)  
        {
            this.aiContainer = new AIContainer(this, new BattleNpcController(this), new PathFind(this), new TargetFind(this));

            this.currentSubState = SetActorStatePacket.SUB_STATE_MONSTER;
            //this.currentMainState = SetActorStatePacket.MAIN_STATE_ACTIVE;

            //charaWork.property[2] = 1;
            //npcWork.hateType = 1;

            this.hateContainer = new HateContainer(this);
            this.allegiance = CharacterTargetingAllegiance.BattleNpcs;

            spawnX = posX;
            spawnY = posY;
            spawnZ = posZ;

            // todo: read this from db
            aggroType = AggroType.Sight;
            this.moveState = 2;
            ResetMoveSpeeds();
            despawnTime = 10;

            CalculateBaseStats();
        }

        public uint GetAggroType()
        {
            return (uint)aggroType;
        }
        
        public void SetAggroType(uint aggroType)
        {
            this.aggroType = (AggroType)aggroType;
        }

        public override void Update(DateTime tick)
        {
            this.aiContainer.Update(tick);
            this.statusEffects.Update(tick);
        }

        public override void PostUpdate(DateTime tick, List<SubPacket> packets = null)
        {
            // todo: should probably add another flag for battleTemp since all this uses reflection
            packets = new List<SubPacket>();
            if ((updateFlags & ActorUpdateFlags.HpTpMp) != 0)
            {
                var propPacketUtil = new ActorPropertyPacketUtil("charaWork.parameterSave", this);

                propPacketUtil.AddProperty("charaWork.parameterSave.hp[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.hpMax[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkillLevel");

                propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[0]");
                propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[1]");
                packets.AddRange(propPacketUtil.Done());
            }
            base.PostUpdate(tick, packets);
        }

        public override bool CanAttack()
        {
            // todo:
            return true;
        }

        public override bool CanCast(Character target, BattleCommand spell)
        {
            // todo:
            return false;
        }

        public override bool CanWeaponSkill(Character target, BattleCommand skill)
        {
            // todo:
            return false;
        }

        public override bool CanUseAbility(Character target, BattleCommand ability)
        {
            // todo:
            return false;
        }

        public uint GetDespawnTime()
        {
            return despawnTime;
        }

        public void SetDespawnTime(uint seconds)
        {
            despawnTime = seconds;
        }

        ///<summary> // todo: create an action object? </summary>
        public bool OnAttack(AttackState state)
        {
            return false;
        }

        public override void Spawn(DateTime tick)
        {
            base.Spawn(tick);

            this.isMovingToSpawn = false;
            this.ResetMoveSpeeds();
            this.hateContainer.ClearHate();
            this.ChangeState(SetActorStatePacket.MAIN_STATE_PASSIVE);
        }

        public override void Die(DateTime tick)
        {
            if (IsAlive())
            {
                aiContainer.InternalDie(tick, despawnTime);
                aiContainer.pathFind.Clear();

                this.ResetMoveSpeeds();
                this.positionX = oldPositionX;
                this.positionY = oldPositionY;
                this.positionZ = oldPositionZ;
                
                
                this.isAtSpawn = true;
            }
            else
            {
                var err = $"[{actorId}][{customDisplayName}] {positionX} {positionY} {positionZ} {GetZoneID()} tried to die ded";
                Program.Log.Error(err);
                //throw new Exception(err);
            }
        }

        public override void Despawn(DateTime tick)
        {
            aiContainer.ClearStates();
            // todo: probably didnt need to make a new state...
            aiContainer.ForceChangeState(new DespawnState(this, null, 0));
        }

        public void OnRoam(DateTime tick)
        {
            // todo: move this to battlenpccontroller..
            bool foundActor = false;

            // leash back to spawn
            if (!IsCloseToSpawn())
            {
                if (!isMovingToSpawn)
                {
                    aiContainer.Reset();
                    isMovingToSpawn = true;
                }
                else
                {
                    if (target == null && !aiContainer.pathFind.IsFollowingPath())
                        aiContainer.pathFind.PathInRange(spawnX, spawnY, spawnZ, 1.5f, 15.0f);
                }
            }
            else
            {
                this.isMovingToSpawn = false;
            }
        }

        public bool IsCloseToSpawn()
        {
            return this.isAtSpawn = Utils.DistanceSquared(positionX, positionY, positionZ, spawnX, spawnY, spawnZ) <= 2500.0f;
        }

        public override void OnAttack(State state, BattleAction action, ref BattleAction error)
        {
            base.OnAttack(state, action, ref error);
            // todo: move this somewhere else prolly and change based on model/appearance (so maybe in Character.cs instead)
            action.animation = 0x11001000; // (temporary) wolf anim
        }
    }
}

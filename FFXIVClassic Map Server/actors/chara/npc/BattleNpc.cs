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
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.packets.send;

namespace FFXIVClassic_Map_Server.Actors
{
    [Flags]
    enum DetectionType
    {
        None                  = 0x00,
        Sight                 = 0x01,
        Scent                 = 0x02,
        Sound                 = 0x04,
        LowHp                 = 0x08,
        IgnoreLevelDifference = 0x10,
        Magic                 = 0x20,
    }

    enum KindredType
    {
        Unknown   = 0,
        Beast     = 1,
        Plantoid  = 2,
        Aquan     = 3,
        Spoken    = 4,
        Reptilian = 5,
        Insect    = 6,
        Avian     = 7,
        Undead    = 8,
        Cursed    = 9,
        Voidsent  = 10,
    }

    class BattleNpc : Npc
    {
        public HateContainer hateContainer;
        public DetectionType detectionType;
        public KindredType kindredType;
        public bool neutral;
        private uint despawnTime;
        private uint respawnTime;
        private uint spawnDistance;
        
        public Character lastAttacker;

        public uint spellListId, skillListId, dropListId;
        public Dictionary<uint, BattleCommand> skillList = new Dictionary<uint, BattleCommand>();
        public Dictionary<uint, BattleCommand> spellList = new Dictionary<uint, BattleCommand>();

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

            // todo: read these from db also
            detectionType = DetectionType.Sight;
            this.moveState = 2;
            ResetMoveSpeeds();
            despawnTime = 10;
            respawnTime = 30;
            CalculateBaseStats();
        }

        public override List<SubPacket> GetSpawnPackets(Player player, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            if (IsAlive())
            {
                subpackets.Add(CreateAddActorPacket());
                subpackets.AddRange(GetEventConditionPackets());
                subpackets.Add(CreateSpeedPacket());
                subpackets.Add(CreateSpawnPositonPacket(0x0));

                subpackets.Add(CreateAppearancePacket());

                subpackets.Add(CreateNamePacket());
                subpackets.Add(CreateStatePacket());
                subpackets.Add(CreateIdleAnimationPacket());
                subpackets.Add(CreateInitStatusPacket());
                subpackets.Add(CreateSetActorIconPacket());
                subpackets.Add(CreateIsZoneingPacket());
                subpackets.Add(CreateScriptBindPacket(player));
            }
            return subpackets;
        }

        public uint GetDetectionType()
        {
            return (uint)detectionType;
        }
        
        public void SetDetectionType(uint detectionType)
        {
            this.detectionType = (DetectionType)detectionType;
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

        public uint GetRespawnTime()
        {
            return respawnTime;
        }

        public void SetRespawnTime(uint seconds)
        {
            respawnTime = seconds;
        }

        ///<summary> // todo: create an action object? </summary>
        public bool OnAttack(AttackState state)
        {
            return false;
        }

        public override void Spawn(DateTime tick)
        {
            if (respawnTime > 0)
            {
                base.Spawn(tick);

                this.isMovingToSpawn = false;
                this.ResetMoveSpeeds();
                this.hateContainer.ClearHate();
                zone.BroadcastPacketsAroundActor(this, GetSpawnPackets(null, 0x01));
                zone.BroadcastPacketsAroundActor(this, GetInitPackets());
                charaWork.parameterSave.hp = charaWork.parameterSave.hpMax;
                charaWork.parameterSave.mp = charaWork.parameterSave.mpMax;
                RecalculateStats();

                OnSpawn();
                updateFlags |= ActorUpdateFlags.AllNpc;
            }
        }

        public override void Die(DateTime tick)
        {
            if (IsAlive())
            {
                // todo: does retail 
                if (lastAttacker is Pet && lastAttacker.aiContainer.GetController<PetController>()?.GetPetMaster() is Player)
                {
                    lastAttacker = lastAttacker.aiContainer.GetController<PetController>().GetPetMaster();
                }

                if (lastAttacker is Player)
                {
                    if (lastAttacker.currentParty != null && lastAttacker.currentParty is Party)
                    {
                        foreach (var memberId in ((Party)lastAttacker.currentParty).members)
                        {
                            var partyMember = zone.FindActorInArea<Player>(memberId);
                            // onDeath(monster, player, killer)
                            lua.LuaEngine.CallLuaBattleFunction(this, "onDeath", this, partyMember, lastAttacker);
                            // <actor> defeat/defeats <target>
                            ((Player)lastAttacker).QueuePacket(BattleActionX01Packet.BuildPacket(lastAttacker.actorId, 0, 0, new BattleAction(actorId, 30108, 0)));
                        }
                    }
                    else
                    {
                        // onDeath(monster, player, killer)
                        lua.LuaEngine.CallLuaBattleFunction(this, "onDeath", this, lastAttacker, lastAttacker);
                        ((Player)lastAttacker).QueuePacket(BattleActionX01Packet.BuildPacket(lastAttacker.actorId, 0, 0, new BattleAction(actorId, 30108, 0)));
                    }
                }
                positionUpdates?.Clear();
                aiContainer.InternalDie(tick, despawnTime);
                this.ResetMoveSpeeds();
                
                // todo: reset cooldowns
            }
            else
            {
                var err = $"[{actorId}][{GetUniqueId()}] {positionX} {positionY} {positionZ} {GetZone().GetName()} tried to die ded";
                Program.Log.Error(err);
                //throw new Exception(err);
            }
        }

        public override void Despawn(DateTime tick)
        {
            // todo: probably didnt need to make a new state...
            aiContainer.InternalDespawn(tick, respawnTime);
            lua.LuaEngine.CallLuaBattleFunction(this, "onDespawn", this);
            this.isAtSpawn = true;
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
                lua.LuaEngine.CallLuaBattleFunction(this, "onRoam", this);
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

        public override void OnSpawn()
        {
            base.OnSpawn();
            lua.LuaEngine.CallLuaBattleFunction(this, "onSpawn", this);
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
        }
    }
}

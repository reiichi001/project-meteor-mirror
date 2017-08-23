
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Map_Server.actors.chara.ai;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    /// <summary> Which Character types am I friendly with </summary>
    enum CharacterTargetingAllegiance
    {
        /// <summary> Friendly to Players </summary>
        Player,
        /// <summary> Friendly to BattleNpcs </summary>
        BattleNpcs
    }

    class Character : Actor
    {
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int MAINHAND = 5;
        public const int OFFHAND = 6;
        public const int SPMAINHAND = 7;
        public const int SPOFFHAND = 8;
        public const int THROWING = 9;
        public const int PACK = 10;
        public const int POUCH = 11;
        public const int HEADGEAR = 12;
        public const int BODYGEAR = 13;
        public const int LEGSGEAR = 14;
        public const int HANDSGEAR = 15;
        public const int FEETGEAR = 16;
        public const int WAISTGEAR = 17;
        public const int NECKGEAR = 18;
        public const int L_EAR = 19;
        public const int R_EAR = 20;
        public const int R_WRIST = 21;
        public const int L_WRIST = 22;
        public const int R_RINGFINGER = 23;
        public const int L_RINGFINGER = 24;
        public const int R_INDEXFINGER = 25;
        public const int L_INDEXFINGER = 26;
        public const int UNKNOWN = 27;

        public bool isStatic = false;

        public bool isMovingToSpawn = false;

        public uint modelId;
        public uint[] appearanceIds = new uint[28];

        public uint animationId = 0;

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public uint currentActorIcon = 0;

        public Work work = new Work();
        public CharaWork charaWork = new CharaWork();

        public Group currentParty = null;
        public ContentGroup currentContentGroup = null;

        //public DateTime lastAiUpdate;

        public AIContainer aiContainer;
        public StatusEffectContainer statusEffects;
        public float meleeRange;
        protected uint attackDelayMs;

        public CharacterTargetingAllegiance allegiance;

        public Pet pet;

        public Character(uint actorID) : base(actorID)
        {            
            //Init timer array to "notimer"
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
                charaWork.statusShownTime[i] = 0xFFFFFFFF;

            this.statusEffects = new StatusEffectContainer(this);

            // todo: move this somewhere more appropriate
            attackDelayMs = 4200;
            meleeRange = 2.5f;
            ResetMoveSpeeds();
        }

        public SubPacket CreateAppearancePacket()
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelId, appearanceIds);
            return setappearance.BuildPacket(actorId);
        }

        public SubPacket CreateInitStatusPacket()
        {
            return (SetActorStatusAllPacket.BuildPacket(actorId, charaWork.status));                      
        }

        public SubPacket CreateSetActorIconPacket()
        {
            return SetActorIconPacket.BuildPacket(actorId, currentActorIcon);
        }

        public SubPacket CreateIdleAnimationPacket()
        {
            return SetActorSubStatPacket.BuildPacket(actorId, 0, 0, 0, 0, 0, 0, animationId);
        }

        public void SetQuestGraphic(Player player, int graphicNum)
        {
            player.QueuePacket(SetActorQuestGraphicPacket.BuildPacket(actorId, graphicNum));
        }

        public void SetCurrentContentGroup(ContentGroup group)
        {
            if (group != null)
                charaWork.currentContentGroup = group.GetTypeId();
            else
                charaWork.currentContentGroup = 0;

            currentContentGroup = group;

            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("charaWork/currentContentGroup", this);
            propPacketUtil.AddProperty("charaWork.currentContentGroup");            
            zone.BroadcastPacketsAroundActor(this, propPacketUtil.Done());

        }

        public List<SubPacket> GetActorStatusPackets()
        {
            var propPacketUtil = new ActorPropertyPacketUtil("charaWork/status", this);
            var i = 0;
            foreach (var effect in statusEffects.GetStatusEffects())
            {
                propPacketUtil.AddProperty($"charaWork.statusShownTime[{i}]");
                i++;
            }
            return propPacketUtil.Done();
        }

        public void PlayAnimation(uint animId, bool onlySelf = false)
        {            
            if (onlySelf)
            {
                if (this is Player)
                    ((Player)this).QueuePacket(PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
            }
            else
                zone.BroadcastPacketAroundActor(this, PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
        }

        public void PathTo(float x, float y, float z, float stepSize = 0.70f, int maxPath = 40, float polyRadius = 0.0f)
        {
            aiContainer?.pathFind?.PreparePath(x, y, z, stepSize, maxPath, polyRadius);
        }

        public void FollowTarget(Actor target, float stepSize = 1.2f, int maxPath = 25, float radius = 0.0f)
        {
            var player = target as Player;

            if (player != null)
            {
                if (this.target != player)
                {
                    this.target = target;
                }
                // todo: move this to own function thing
                this.oldMoveState = this.moveState;
                this.moveState = 2;
                updateFlags |= ActorUpdateFlags.Position | ActorUpdateFlags.Speed;
                //this.moveSpeeds = player.moveSpeeds;

                PathTo(player.positionX, player.positionY, player.positionZ, stepSize, maxPath, radius);
            }
        }

        public virtual void OnPath(Vector3 point)
        {
            lua.LuaEngine.CallLuaBattleAction(this, "onPath", this, point);

            updateFlags |= ActorUpdateFlags.Position;
            this.isAtSpawn = false;
        }

        public override void Update(DateTime tick)
        {

        }

        public override void PostUpdate(DateTime tick, List<SubPacket> packets = null)
        {
            if (updateFlags != ActorUpdateFlags.None)
            {
                packets = packets ?? new List<SubPacket>();
                
                if ((updateFlags & ActorUpdateFlags.Appearance) != 0)
                {
                    packets.Add(new SetActorAppearancePacket(modelId, appearanceIds).BuildPacket(actorId));
                }

                // todo: should probably add another flag for battleTemp since all this uses reflection
                if ((updateFlags & ActorUpdateFlags.HpTpMp) != 0)
                {
                    var propPacketUtil = new ActorPropertyPacketUtil("charaWork.parameterSave", this);
                    propPacketUtil.AddProperty("charaWork.parameterSave.mp");
                    propPacketUtil.AddProperty("charaWork.parameterSave.mpMax");
                    propPacketUtil.AddProperty("charaWork.parameterTemp.tp");
                    packets.AddRange(propPacketUtil.Done());
                }
                base.PostUpdate(tick, packets);
            }
        }

        public virtual bool CanAttack()
        {
            return false;
        }

        public virtual bool CanCast()
        {
            return false;
        }

        public virtual uint GetAttackDelayMs()
        {
            return attackDelayMs;
        }

        public bool Engage(uint targid = 0)
        {
            // todo: attack the things
            targid = targid == 0 ? currentTarget: targid;
            if (targid != 0)
            {
                var targ = Server.GetWorldManager().GetActorInWorld(targid);
                if (targ is Character)
                    aiContainer.Engage((Character)targ);
            }
            return false;
        }

        public bool Disengage()
        {
            if (aiContainer != null)
            {
                aiContainer.Disengage();
                return true;
            }
            return false;
        }

        public virtual void Spawn(DateTime tick)
        {
            // todo: reset hp/mp/tp etc here
            RecalculateHpMpTp();
        }

        public virtual void Die(DateTime tick)
        {
            // todo: actual despawn timer
            aiContainer.InternalDie(tick, 10);
        }

        protected virtual void Despawn(DateTime tick)
        {

        }

        public bool IsDead()
        {
            return currentMainState == SetActorStatePacket.MAIN_STATE_DEAD || currentMainState == SetActorStatePacket.MAIN_STATE_DEAD2;
        }

        public bool IsAlive()
        {
            return !IsDead();
        }

        public virtual short GetHP()
        {
            // todo: 
            return charaWork.parameterSave.hp[0];
        }

        public virtual short GetMaxHP()
        {
            return charaWork.parameterSave.hpMax[0];
        }

        public virtual byte GetHPP()
        {
            return (byte)(charaWork.parameterSave.hp[0] / charaWork.parameterSave.hpMax[0]);
        }

        public virtual void AddHP(short hp)
        {
            // todo: +/- hp and die
            // todo: battlenpcs probably have way more hp?
            var addHp = charaWork.parameterSave.hp[0] + hp;
            addHp = addHp.Clamp(short.MinValue, charaWork.parameterSave.hpMax[0]);
            charaWork.parameterSave.hp[0] = (short)addHp;

            if (charaWork.parameterSave.hp[0] < 1)
                Die(Program.Tick);

            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        public virtual void DelHP(short hp)
        {
            AddHP((short)-hp);
        }

        // todo: should this include stats too?
        public virtual void RecalculateHpMpTp()
        {
            // todo: recalculate stats and crap
            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        public virtual float GetSpeed()
        {
            // todo: for battlenpc/player calculate speed
            return moveSpeeds[2];
        }
    }

}

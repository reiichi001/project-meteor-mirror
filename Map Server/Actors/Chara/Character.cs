/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/


using Meteor.Common;
using Meteor.Map.actors.chara.player;
using Meteor.Map.actors.group;
using Meteor.Map.Actors.Chara;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.actor;
using Meteor.Map.packets.send.actor.inventory;
using Meteor.Map.utils;
using Meteor.Map.actors.chara.ai;
using System;
using System.Collections.Generic;
using Meteor.Map.actors.chara;
using Meteor.Map.packets.send.actor.battle;
using Meteor.Map.actors.chara.ai.state;
using Meteor.Map.actors.chara.ai.utils;
using Meteor.Map.actors.chara.npc;

namespace Meteor.Map.Actors
{
    /// <summary> Which Character types am I friendly with </summary>
    enum CharacterTargetingAllegiance
    {
        /// <summary> Friendly to BattleNpcs </summary>
        BattleNpcs,
        /// <summary> Friendly to Players </summary>
        Player
    }

    enum DamageTakenType
    {
        None,
        Attack,
        Magic,
        Weaponskill,
        Ability
    }

    class Character : Actor
    {
        public const int CLASSID_PUG = 2;
        public const int CLASSID_GLA = 3;
        public const int CLASSID_MRD = 4;
        public const int CLASSID_ARC = 7;
        public const int CLASSID_LNC = 8;
        public const int CLASSID_THM = 22;
        public const int CLASSID_CNJ = 23;

        public const int CLASSID_CRP = 29;
        public const int CLASSID_BSM = 30;
        public const int CLASSID_ARM = 31;
        public const int CLASSID_GSM = 32;
        public const int CLASSID_LTW = 33;
        public const int CLASSID_WVR = 34;
        public const int CLASSID_ALC = 35;
        public const int CLASSID_CUL = 36;

        public const int CLASSID_MIN = 39;
        public const int CLASSID_BTN = 40;
        public const int CLASSID_FSH = 41;

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
        public bool isAutoAttackEnabled = true;

        public uint modelId;
        public uint[] appearanceIds = new uint[28];

        public uint animationId = 0;

        public uint currentTarget = Actor.INVALID_ACTORID;
        public uint currentLockedTarget = Actor.INVALID_ACTORID;

        public uint currentActorIcon = 0;

        public Work work = new Work();
        public CharaWork charaWork = new CharaWork();

        public Group currentParty = null;
        public ContentGroup currentContentGroup = null;
        
        //public DateTime lastAiUpdate;

        public AIContainer aiContainer;
        public StatusEffectContainer statusEffects;

        public CharacterTargetingAllegiance allegiance;

        public Pet pet;

        private Dictionary<Modifier, double> modifiers = new Dictionary<Modifier, double>();

        protected ushort hpBase, hpMaxBase, mpBase, mpMaxBase, tpBase;
        protected BattleTemp baseStats = new BattleTemp();
        public ushort currentJob;
        public ushort newMainState;
        public float spawnX, spawnY, spawnZ;

        //I needed some values I could reuse for random stuff, delete later
        public int extraInt;
        public uint extraUint;
        public float extraFloat;

        protected Dictionary<string, UInt64> tempVars = new Dictionary<string, UInt64>();
        
        //Inventory        
        protected Dictionary<ushort, ItemPackage> itemPackages = new Dictionary<ushort, ItemPackage>();
        protected ReferencedItemPackage equipment;

        public Character(uint actorID)
            : base(actorID)
        {
            //Init timer array to "notimer"
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
                charaWork.statusShownTime[i] = 0;

            this.statusEffects = new StatusEffectContainer(this);

            // todo: move this somewhere more appropriate
            // todo: base this on equip and shit
            SetMod((uint)Modifier.AttackRange, 3);
            SetMod((uint)Modifier.Delay, (Program.Random.Next(30, 60) * 100));
            SetMod((uint)Modifier.MovementSpeed, (uint)moveSpeeds[2]);

            spawnX = positionX;
            spawnY = positionY;
            spawnZ = positionZ;
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

        public SubPacket CreateSubStatePacket()
        {
            return SetActorSubStatePacket.BuildPacket(actorId, currentSubState);
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

        //This logic isn't correct, order of GetStatusEffects() is not necessarily the same as the actual effects in game. Also sending every time at once isn't needed
        public List<SubPacket> GetActorStatusPackets()
        {
            var propPacketUtil = new ActorPropertyPacketUtil("charaWork/status", this);
            var i = 0;
            foreach (var effect in statusEffects.GetStatusEffects())
            {
                if (!effect.GetHidden())
                {
                    propPacketUtil.AddProperty(String.Format("charaWork.statusShownTime[{0}]", i));
                    propPacketUtil.AddProperty(String.Format("charaWork.statusShownTime[{0}]", i));
                    i++;
                }
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
        
        public void DoBattleAction(ushort commandId, uint animationId)
        {
            zone.BroadcastPacketAroundActor(this, CommandResultX00Packet.BuildPacket(actorId, animationId, commandId));
        }

        public void DoBattleAction(ushort commandId, uint animationId, CommandResult result)
        {
            zone.BroadcastPacketAroundActor(this, CommandResultX01Packet.BuildPacket(actorId, animationId, commandId, result));
        }

        public void DoBattleAction(ushort commandId, uint animationId, CommandResult[] results)
        {
            int currentIndex = 0;

            while (true)
            {
                if (results.Length - currentIndex >= 10)
                    zone.BroadcastPacketAroundActor(this, CommandResultX18Packet.BuildPacket(actorId, animationId, commandId, results, ref currentIndex));
                else if (results.Length - currentIndex > 1)
                    zone.BroadcastPacketAroundActor(this, CommandResultX10Packet.BuildPacket(actorId, animationId, commandId, results, ref currentIndex));
                else if (results.Length - currentIndex == 1)
                {
                    zone.BroadcastPacketAroundActor(this, CommandResultX01Packet.BuildPacket(actorId, animationId, commandId, results[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }
        }

        public void DoBattleAction(ushort commandId, uint animationId, List<CommandResult> results)
        {
            int currentIndex = 0;

            while (true)
            {
                if (results.Count - currentIndex >= 10)
                    zone.BroadcastPacketAroundActor(this, CommandResultX18Packet.BuildPacket(actorId, animationId, commandId, results, ref currentIndex));
                else if (results.Count - currentIndex > 1)
                    zone.BroadcastPacketAroundActor(this, CommandResultX10Packet.BuildPacket(actorId, animationId, commandId, results, ref currentIndex));
                else if (results.Count - currentIndex == 1)
                {
                    zone.BroadcastPacketAroundActor(this, CommandResultX01Packet.BuildPacket(actorId, animationId, commandId, results[currentIndex]));
                    currentIndex++;
                }
                else
                    break;

                //Sending multiple packets at once causes some issues. Setting any combination of these to zero changes what breaks
                //animationId = 0; //If more than one packet is sent out, only send the animation once to avoid double playing.
                //commandId = 0;
                //sourceActorId = 0;
            }            
        }

        #region ai stuff
        public void PathTo(float x, float y, float z, float stepSize = 0.70f, int maxPath = 40, float polyRadius = 0.0f)
        {
            if (aiContainer != null && aiContainer.pathFind != null)
                aiContainer.pathFind.PreparePath(x, y, z, stepSize, maxPath, polyRadius);
        }

        public void FollowTarget(Actor target, float stepSize = 1.2f, int maxPath = 25, float radius = 0.0f)
        {
            if (target != null)
                PathTo(target.positionX, target.positionY, target.positionZ, stepSize, maxPath, radius);
        }

        public double GetMod(Modifier modifier)
        {
            return GetMod((uint)modifier);
        }

        public double GetMod(uint modifier)
        {
            double res;
            if (modifiers.TryGetValue((Modifier)modifier, out res))
                return res;
            return 0;
        }

        public void SetMod(uint modifier, double val)
        {
            if (modifiers.ContainsKey((Modifier)modifier))
                modifiers[(Modifier)modifier] = val;
            else
                modifiers.Add((Modifier)modifier, val);

            if (modifier >= 3 && modifier <= 35)
                updateFlags |= ActorUpdateFlags.Stats;
        }

        public void AddMod(Modifier modifier, double val)
        {
            AddMod((uint)modifier, val);
        }

        public void AddMod(uint modifier, double val)
        {

            double newVal = GetMod(modifier) + val;
            SetMod(modifier, newVal);
        }

        public void SubtractMod(Modifier modifier, double val)
        {
            AddMod((uint)modifier, val);
        }

        public void SubtractMod(uint modifier, double val)
        {
            double newVal = GetMod(modifier) - val;
            SetMod(modifier, newVal);
        }

        public void MultiplyMod(Modifier modifier, double val)
        {
            MultiplyMod((uint)modifier, val);
        }

        public void MultiplyMod(uint modifier, double val)
        {
            double newVal = GetMod(modifier) * val;
            SetMod(modifier, newVal);
        }

        public void DivideMod(Modifier modifier, double val)
        {
            DivideMod((uint)modifier, val);
        }

        public void DivideMod(uint modifier, double val)
        {
            double newVal = GetMod(modifier) / val;
            SetMod(modifier, newVal);
        }


        public virtual void OnPath(Vector3 point)
        {
            //lua.LuaEngine.CallLuaBattleFunction(this, "onPath", this, point);

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

                if ((updateFlags & ActorUpdateFlags.State) != 0)
                {
                    packets.Add(SetActorStatePacket.BuildPacket(actorId, currentMainState, 0x0));
                    packets.Add(CommandResultX00Packet.BuildPacket(actorId, 0x72000062, 0));
                    packets.Add(CommandResultX01Packet.BuildPacket(actorId, 0x7C000062, 21001, new CommandResult(actorId, 0, 1)));

                    updateFlags &= ~ActorUpdateFlags.State;
                    //DoBattleAction(21001, 0x7C000062, new BattleAction(this.actorId, 0, 1, 0, 0, 1)); //Attack Mode
                }

                if ((updateFlags & ActorUpdateFlags.SubState) != 0)
                {
                    packets.Add(SetActorSubStatePacket.BuildPacket(actorId, currentSubState));
                    //packets.Add(CommandResultX00Packet.BuildPacket(actorId, 0x72000062, 0));
                    //packets.Add(CommandResultX01Packet.BuildPacket(actorId, 0x7C000062, 21001, new CommandResult(actorId, 0, 1)));

                    updateFlags &= ~ActorUpdateFlags.SubState;
                    //DoBattleAction(21001, 0x7C000062, new BattleAction(this.actorId, 0, 1, 0, 0, 1)); //Attack Mode
                }

                if ((updateFlags & ActorUpdateFlags.Status) != 0)
                {

                    List<SubPacket> statusPackets = statusEffects.GetStatusPackets();
                    packets.AddRange(statusPackets);
                    statusPackets.Clear();
                    updateFlags &= ~ActorUpdateFlags.Status;
                }

                if ((updateFlags & ActorUpdateFlags.StatusTime) != 0)
                {
                    packets.AddRange(statusEffects.GetStatusTimerPackets());
                    statusEffects.ResetPropPacketUtil();
                    updateFlags &= ~ActorUpdateFlags.StatusTime;
                }

                if ((updateFlags & ActorUpdateFlags.HpTpMp) != 0)
                {
                    var propPacketUtil = new ActorPropertyPacketUtil("charaWork/stateAtQuicklyForAll", this);
                    propPacketUtil.AddProperty("charaWork.parameterSave.hp[0]");
                    propPacketUtil.AddProperty("charaWork.parameterSave.hpMax[0]");
                    propPacketUtil.AddProperty("charaWork.parameterSave.mp");
                    propPacketUtil.AddProperty("charaWork.parameterSave.mpMax");
                    propPacketUtil.AddProperty("charaWork.parameterTemp.tp");
                    packets.AddRange(propPacketUtil.Done());
                }

                base.PostUpdate(tick, packets);
            }
        }

        public virtual bool IsValidTarget(Character target, ValidTarget validTarget)
        {
            return !target.isStatic;
        }

        public virtual bool CanAttack()
        {
            return true;
        }

        public virtual bool CanUse(Character target, BattleCommand skill, CommandResult error = null)
        {
            return false;
        }


        public virtual uint GetAttackDelayMs()
        {
            return (uint)GetMod((uint)Modifier.Delay);
        }

        public virtual uint GetAttackRange()
        {
            return (uint)(GetMod((uint)Modifier.AttackRange) == 0 ? 3 : GetMod((uint)Modifier.AttackRange));
        }

        public virtual bool Engage(uint targid = 0, ushort newMainState = 0xFFFF)
        {
            // todo: attack the things
            /*if (newMainState != 0xFFFF)
            {
                currentMainState = newMainState;// this.newMainState = newMainState;
                updateFlags |= ActorUpdateFlags.State;
            }
            else*/ if (aiContainer.CanChangeState())
            {
                if (targid == 0)
                {
                    if (currentTarget != Actor.INVALID_ACTORID)
                        targid = currentTarget;
                    else if (currentLockedTarget != Actor.INVALID_ACTORID)
                        targid = currentLockedTarget;
                }
                //if (targid != 0)
                {
                    aiContainer.Engage(zone.FindActorInArea<Character>(targid));
                }
            }

            return false;
        }

        public virtual bool Engage(Character target)
        {
            aiContainer.Engage(target);
            return false;
        }

        public virtual bool Disengage(ushort newMainState = 0xFFFF)
        {
            /*if (newMainState != 0xFFFF)
            {
                currentMainState = newMainState;// this.newMainState = newMainState;
                updateFlags |= ActorUpdateFlags.State;
            }
            else*/ if (IsEngaged())
            {
                aiContainer.Disengage();
                return true;
            }
            return false;
        }
        
        public virtual void Cast(uint spellId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.Cast(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), spellId);
        }

        public virtual void Ability(uint abilityId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.Ability(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), abilityId);
        }

        public virtual void WeaponSkill(uint skillId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.WeaponSkill(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), skillId);
        }

        public virtual void Spawn(DateTime tick)
        {
            aiContainer.Reset();
            // todo: reset hp/mp/tp etc here
            ChangeState(SetActorStatePacket.MAIN_STATE_PASSIVE);
            SetHP((uint) GetMaxHP());
            SetMP((uint) GetMaxMP());
            RecalculateStats();
        }

        //AdditionalActions is the list of actions that EXP/Chain messages are added to
        public virtual void Die(DateTime tick, CommandResultContainer actionContainer = null)
        {
            // todo: actual despawn timer
            aiContainer.InternalDie(tick, 10);
            ChangeSpeed(0.0f, 0.0f, 0.0f, 0.0f);
        }

        public virtual void Despawn(DateTime tick)
        {
            
        }

        public bool IsDead()
        {
            return !IsAlive();
        }

        public bool IsAlive()
        {
            return !aiContainer.IsDead();// && GetHP() > 0;
        }

        public short GetHP()
        {
            // todo: 
            return charaWork.parameterSave.hp[0];
        }

        public short GetMaxHP()
        {
            return charaWork.parameterSave.hpMax[0];
        }

        public short GetMP()
        {
            return charaWork.parameterSave.mp;
        }

        public ushort GetTP()
        {
            return tpBase;
        }

        public short GetMaxMP()
        {
            return charaWork.parameterSave.mpMax;
        }

        public byte GetMPP()
        {
            return (byte)((charaWork.parameterSave.mp / charaWork.parameterSave.mpMax) * 100);
        }

        public byte GetTPP()
        {
            return (byte)((tpBase / 3000) * 100);
        }

        public byte GetHPP()
        {
            return (byte)(charaWork.parameterSave.hp[0] == 0 ? 0 : (charaWork.parameterSave.hp[0] / (float) charaWork.parameterSave.hpMax[0]) * 100);
        }

        public void SetHP(uint hp)
        {
            charaWork.parameterSave.hp[0] = (short)hp;
            if (hp > charaWork.parameterSave.hpMax[0])
                SetMaxHP(hp);

            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        public void SetMaxHP(uint hp)
        {
            charaWork.parameterSave.hpMax[0] = (short)hp;
            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        public void SetMP(uint mp)
        {
            charaWork.parameterSave.mp = (short)mp;
            if (mp > charaWork.parameterSave.mpMax)
                SetMaxMP(mp);

            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        public void SetMaxMP(uint mp)
        {
            charaWork.parameterSave.mp = (short)mp;
            updateFlags |= ActorUpdateFlags.HpTpMp;
        }

        // todo: the following functions are virtuals since we want to check hidden item bonuses etc on player for certain conditions
        public virtual void AddHP(int hp, CommandResultContainer resultContainer = null)
        {
            // dont wanna die ded, don't want to send update if hp isn't actually changed
            if (IsAlive() && hp != 0)
            {
                // todo: +/- hp and die
                // todo: battlenpcs probably have way more hp?
                var addHp = charaWork.parameterSave.hp[0] + hp;
                addHp = addHp.Clamp((short)GetMod((uint)Modifier.MinimumHpLock), charaWork.parameterSave.hpMax[0]);
                charaWork.parameterSave.hp[0] = (short)addHp;

                updateFlags |= ActorUpdateFlags.HpTpMp;

                if (charaWork.parameterSave.hp[0] < 1)
                    Die(Program.Tick, resultContainer);
            }
        }

        public short GetClass()
        {
            return charaWork.parameterSave.state_mainSkill[0];
        }

        public short GetLevel()
        {
            return charaWork.parameterSave.state_mainSkillLevel;
        }

        public void AddMP(int mp)
        {
            if (IsAlive() && mp != 0)
            {
                charaWork.parameterSave.mp = (short)(charaWork.parameterSave.mp + mp).Clamp(ushort.MinValue, charaWork.parameterSave.mpMax);

                // todo: check hidden effects and shit

                updateFlags |= ActorUpdateFlags.HpTpMp;
            }
        }

        public void AddTP(int tp)
        {
            if (IsAlive() && tp != 0)
            {
                var addTp = charaWork.parameterTemp.tp + tp;
                
                addTp = addTp.Clamp((int) GetMod(Modifier.MinimumTpLock), 3000);
                charaWork.parameterTemp.tp = (short) addTp;
                tpBase = (ushort)charaWork.parameterTemp.tp;
                updateFlags |= ActorUpdateFlags.HpTpMp;

                if (tpBase >= 1000)
                    lua.LuaEngine.GetInstance().OnSignal("tpOver1000");
            }
        }

        public void DelHP(int hp, CommandResultContainer resultContainer = null)
        {
            AddHP((short)-hp, resultContainer);
        }

        public void DelMP(int mp)
        {
            AddMP(-mp);
        }

        public void DelTP(int tp)
        {
            AddTP(-tp);
        }

        virtual public void CalculateBaseStats()
        {
            // todo: apply mods and shit here, get race/level/job and shit
            uint hpMod = (uint) GetMod((uint)Modifier.Hp);
            if (hpMod != 0)
            {
                SetMaxHP(hpMod);
                uint hpp = (uint) GetMod((uint) Modifier.HpPercent);
                uint hp = hpMod;
                if(hpp != 0)
                {
                    hp = (uint) Math.Ceiling(((float)hpp / 100.0f) * hpMod);
                }
                SetHP(hp);
            }

            uint mpMod = (uint)GetMod((uint)Modifier.Mp);
            if (mpMod != 0)
            {
                SetMaxMP(mpMod);
                uint mpp = (uint)GetMod((uint)Modifier.MpPercent);
                uint mp = mpMod;
                if (mpp != 0)
                {
                    mp = (uint)Math.Ceiling(((float)mpp / 100.0f) * mpMod);
                }
                SetMP(mp);
            }
            // todo: recalculate stats and crap
            updateFlags |= ActorUpdateFlags.HpTpMp;


            SetMod((uint)Modifier.HitCount, 1);
        }

        public void RecalculateStats()
        {
            //CalculateBaseStats();
        }

        public void SetStat(uint statId, int val)
        {
            charaWork.battleTemp.generalParameter[statId] = (short)val;
        }

        public short GetStat(uint statId)
        {
            return charaWork.battleTemp.generalParameter[statId];
        }

        public virtual float GetSpeed()
        {
            // todo: for battlenpc/player calculate speed
            return (float) GetMod((uint)Modifier.MovementSpeed);
        }

        public virtual void OnAttack(State state, CommandResult action, ref CommandResult error)
        {
            var target = state.GetTarget();
            // todo: change animation based on equipped weapon
            // todo: get hitrate and shit, handle protect effect and whatever
            if (BattleUtils.TryAttack(this, target, action, ref error))
            {
                //var packet = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, target.actorId, (uint)0x19001000, (uint)0x8000604, (ushort)0x765D, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, (byte)0x1);
            }

            // todo: call onAttack/onDamageTaken
            //BattleUtils.DamageTarget(this, target, DamageTakenType.Attack, action);
            AddTP(200);
            target.AddTP(100);
        }

        public virtual void OnCast(State state, CommandResult[] actions, BattleCommand spell, ref CommandResult[] errors)
        {
            // damage is handled in script
            var spellCost = spell.CalculateMpCost(this);
            this.DelMP(spellCost); // mpCost can be set in script e.g. if caster has something for free spells

            foreach (CommandResult action in actions)
            {
                if (zone.FindActorInArea<Character>(action.targetId) is Character)
                {
                    //BattleUtils.HandleHitType(this, chara, action);
                    //BattleUtils.DoAction(this, chara, action, DamageTakenType.Magic);
                }
            }
            lua.LuaEngine.GetInstance().OnSignal("spellUsed");
        }

        public virtual void OnWeaponSkill(State state, CommandResult[] actions, BattleCommand skill, ref CommandResult[] errors)
        {
            // damage is handled in script

            foreach (CommandResult action in actions)
            {
                //Should we just store the character insteado f having to find it again?
                if (zone.FindActorInArea<Character>(action.targetId) is Character)
                {
                    //BattleUtils.DoAction(this, chara, action, DamageTakenType.Weaponskill);
                }
            }

            this.DelTP(skill.tpCost);
        }

        public virtual void OnAbility(State state, CommandResult[] actions, BattleCommand ability, ref CommandResult[] errors)
        {
            foreach (var action in actions)
            {
                if (zone.FindActorInArea<Character>(action.targetId) is Character)
                {
                    //BattleUtils.DoAction(this, chara, action, DamageTakenType.Ability);
                }
            }
        }

        public virtual void OnSpawn()
        {

        }

        public virtual void OnDeath()
        {

        }

        public virtual void OnDespawn()
        {

        }

        public virtual void OnDamageDealt(Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            switch (action.hitType)
            {
                case (HitType.Miss):
                    OnMiss(defender, skill, action, actionContainer);
                    break;
                case (HitType.Crit):
                    OnCrit(defender, skill, action, actionContainer);
                    OnHit(defender, skill, action, actionContainer);
                    break;
                default:
                    OnHit(defender, skill, action, actionContainer);
                    break;
            }

            //TP is only gained from autoattacks and abilities
            if ((action.commandType == CommandType.AutoAttack || action.commandType == CommandType.Ability) && action.hitType != HitType.Miss)
            {
                //TP gained on an attack is usually 100 * delay.
                //Store TP seems to add .1% per point.
                double weaponDelay = GetMod(Modifier.Delay) / 1000.0;
                var storeTPPercent = 1 + (GetMod(Modifier.StoreTp) * 0.001);
                AddTP((int)(weaponDelay * 100 * storeTPPercent));
            }
        }

        public virtual void OnDamageTaken(Character attacker, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            switch (action.hitType)
            {
                case (HitType.Miss):
                    OnEvade(attacker, skill, action, actionContainer);
                    break;
                case (HitType.Parry):
                    OnParry(attacker, skill, action, actionContainer);
                    break;
                case (HitType.Block):
                    OnBlock(attacker, skill, action, actionContainer);
                    break;
            }

            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnDamageTaken, "onDamageTaken", attacker, this, skill, action, actionContainer);

            //TP gain formula seems to be something like 5 * e ^ ( -0.667 * [defender's level] ) * damage taken, rounded up
            //This should be completely accurate at level 50, but isn't totally accurate at lower levels.
            //Don't know if store tp impacts this
            double tpModifier = 5 * Math.Pow(Math.E, (-0.0667 * GetLevel()));
            AddTP((int)Math.Ceiling(tpModifier * action.amount));
        }

        public UInt64 GetTempVar(string name)
        {
            UInt64 retVal = 0;
            if (tempVars.TryGetValue(name, out retVal))
                return retVal;
            return 0;
        }

        // cause lua is a dick
        public void SetTempVar(string name, uint val)
        {
            if (tempVars.ContainsKey(name))
                tempVars[name] = val;
        }

        public void SetTempVar(string name, UInt64 val)
        {
            if (tempVars.ContainsKey(name))
                tempVars[name] = val;
        }

        public void ResetTempVars()
        {
            tempVars.Clear();
        }

        #region lua helpers
        public bool IsEngaged()
        {
            return aiContainer.IsEngaged();
        }

        public bool IsPlayer()
        {
            return this is Player;
        }

        public bool IsMonster()
        {
            return this is BattleNpc;
        }

        public bool IsPet()
        {
            return this is Pet;
        }

        public bool IsAlly()
        {
            return this is Ally;
        }

        public bool IsDiscipleOfWar()
        {
            return GetClass() < CLASSID_THM;
        }

        public bool IsDiscipleOfMagic()
        {
            return GetClass() >= CLASSID_THM && currentJob < CLASSID_CRP;
        }

        public bool IsDiscipleOfHand()
        {
            return GetClass() >= CLASSID_CRP && currentJob < CLASSID_MIN;
        }

        public bool IsDiscipleOfLand()
        {
            return GetClass() >= CLASSID_MIN;
        }
        #endregion lua helpers
        #endregion ai stuff

        //Reset procs. Only send packet if any procs were actually reset. 
        //This assumes you can't use weaponskills between getting a proc and using the procced ability
        public void ResetProcs()
        {
            var propPacketUtil = new ActorPropertyPacketUtil("charaWork/timingCommand", this);
            bool shouldSend = false;
            for (int i = 0; i < 4; i++)
            {
                if (charaWork.battleTemp.timingCommandFlag[i])
                {
                    shouldSend = true;
                    charaWork.battleTemp.timingCommandFlag[i] = false;
                    propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.timingCommandFlag[{0}]", i));
                }
            }

            if (shouldSend && this is Player)
                ((Player)this).QueuePackets(propPacketUtil.Done());
        }

        //Set given proc to true and send packet if this is a player
        // todo: hidden status effects for timing when the procs fall off
        public void SetProc(int procId, bool val = true)
        {
            charaWork.battleTemp.timingCommandFlag[procId] = val;
            uint effectId = (uint)StatusEffectId.EvadeProc + (uint)procId;

            //If a proc just occurred, add a hidden effect effect
            if (val)
            {
                StatusEffect procEffect = Server.GetWorldManager().GetStatusEffect(effectId);
                procEffect.SetDuration(5);
                statusEffects.AddStatusEffect(procEffect, this);
            }
            //Otherwise we're reseting a proc, remove the status
            else
            {
                statusEffects.RemoveStatusEffect(statusEffects.GetStatusEffectById((uint)effectId));
            }

            if (this is Player)
            {
                var propPacketUtil = new ActorPropertyPacketUtil("charaWork/timingCommand", this);
                propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.timingCommandFlag[{0}]", procId));
                ((Player)this).QueuePackets(propPacketUtil.Done());
            }
        }

        public HitDirection GetHitDirection(Actor target)
        {
            //Get between taget's position and our position
            double angle = Vector3.GetAngle(target.GetPosAsVector3(), GetPosAsVector3());
            //Add to the target's rotation, mod by 2pi. This is the angle relative to where the target is looking
            //Actor's rotation is 0 degrees on their left side, rotate it by 45 degrees so that quadrants line up with sides
            angle = (angle + target.rotation - (.25 * Math.PI)) % (2 * Math.PI);
            //Make positive
            if (angle < 0)
                angle = angle + (2 * Math.PI);

            //Get the side we're on. 0 is front, 1 is right, 2 is rear, 3 is left
            var side = (int) (angle / (.5 * Math.PI)) % 4;

            return (HitDirection) (1 << side);
        }

        //Called when this character evades attacker's action
        public void OnEvade(Character attacker, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            SetProc((ushort)HitType.Evade);
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnEvade, "onEvade", attacker, this, skill, action, actionContainer);
        }

        //Called when this character blocks attacker's action
        public void OnBlock(Character attacker, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            SetProc((ushort)HitType.Block);
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnBlock, "onBlock", attacker, this, skill, action, actionContainer);
        }

        //Called when this character parries attacker's action
        public void OnParry(Character attacker, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            SetProc((ushort)HitType.Parry);
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnParry, "onParry", attacker, this, skill, action, actionContainer);
        }

        //Called when this character misses
        public void OnMiss(Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            SetProc((ushort)HitType.Miss);
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnMiss, "onMiss", this, defender, skill, action, actionContainer);
        }

        public void OnHit(Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnHit, "onHit", this, defender, skill, action, actionContainer);
        }

        public void OnCrit(Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnCrit, "onCrit", this, defender, skill, action, actionContainer);
        }

        //The order of messages that appears after using a command is:

        //1. Cast start messages. (ie "You begin casting... ")
        //2. Messages from buffs that activate before the command actually starts, like Power Surge or Presence of Mind. (This may be wrong and these could be the same as 4.)
        //3. If the command is a multi-hit command, this is where the "You use [command] on [target]" message goes

        //Then, for each hit:
        //4. Buffs that activate before a command hits, like Blindside
        //5. The hit itself. For single hit commands this message is "Your [command] hits [target] for x damage" for multi hits it's "[Target] takes x points of damage"
        //6. Stoneskin falling off
        //6. Buffs that activate after a command hits, like Aegis Boon and Divine Veil

        //After all hits
        //7. If it's a multi-hit command there's a "{numhits]fold attack..." message or if all hits miss an "All attacks missed" message
        //8. Buffs that fall off after the skill ends, like Excruciate

        //For every target defeated:
        //8. Defeat message
        //9. EXP message
        //10. EXP chain message


        //folder is probably temporary until move to cached scripts is complete
        public void DoBattleCommand(BattleCommand command, string folder)
        {
            //List<BattleAction> actions = new List<BattleAction>();
            CommandResultContainer actions = new CommandResultContainer();

            var targets = command.targetFind.GetTargets();
            bool hitTarget = false;

            if (targets.Count > 0)
            {
                statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnCommandStart, "onCommandStart", this, command, actions);

                foreach (var chara in targets)
                {
                    ushort hitCount = 0;
                    ushort totalDamage = 0;
                    for (int hitNum = 1; hitNum <= command.numHits; hitNum++)
                    {
                        var action = new CommandResult(chara.actorId, command, (byte)GetHitDirection(chara), (byte) hitNum);
                        
                        //uncached script
                        lua.LuaEngine.CallLuaBattleCommandFunction(this, command, folder, "onSkillFinish", this, chara, command, action, actions);
                        //cached script
                        //skill.CallLuaFunction(owner, "onSkillFinish", this, chara, command, action, actions);
                        if (action.ActionLanded())
                        {
                            hitTarget = true;
                            hitCount++;
                            totalDamage += action.amount;
                        }
                    }

                    if (command.numHits > 1)
                    {
                        //30442: [hitCount]fold Attack! [chara] takes a total of totalDamage points of damage.
                        //30450: All attacks miss!
                        ushort textId = (ushort) (hitTarget ? 30442 : 30450);
                        actions.AddAction(new CommandResult(chara.actorId, textId, 0, totalDamage, (byte)hitCount));
                    }
                }

                statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnCommandFinish, "onCommandFinish", this, command, actions);
            }
            else
            {
                actions.AddAction(new CommandResult(actorId, 30202, 0));
            }

            DelMP(command.CalculateMpCost(this));
            DelTP(command.CalculateTpCost(this));

            //Now that we know if we hit the target we can check if the combo continues
            if (this is Player)
            {
                if (command.isCombo && hitTarget)
                    ((Player)this).SetCombos(command.comboNextCommandId);
                //Only reset combo if the command is a spell or weaponskill, since abilities can be used between combo skills
                else if (command.commandType == CommandType.Spell || command.commandType == CommandType.WeaponSkill)
                    ((Player)this).SetCombos();
            }


            actions.CombineLists();
            DoBattleAction(command.id, command.battleAnimation, actions.GetList());
        }

        public List<Character> GetPartyMembersInRange(uint range)
        {
            TargetFind targetFind = new TargetFind(this);
            targetFind.SetAOEType(ValidTarget.Party, TargetFindAOEType.Circle, TargetFindAOETarget.Self, range, 0, 10, 0, 0);
            targetFind.FindWithinArea(this, ValidTarget.Party, TargetFindAOETarget.Self);
            return targetFind.GetTargets();
        }

        #region Inventory
        public void SendItemPackage(Player player, uint id)
        {
            if (!itemPackages.ContainsKey((ushort)id))
                return;

            player.QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId, true));
            itemPackages[(ushort)id].SendFullPackage(player);
            player.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
        }

        public void AddItem(uint catalogID)
        {
            AddItem(catalogID, 1);
        }

        public void AddItem(uint catalogID, int quantity)
        {
            AddItem(catalogID, quantity, 1);
        }

        public void AddItem(uint catalogID, int quantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].AddItem(catalogID, quantity, quality);
            }
        }

        public void AddItem(InventoryItem item)
        {
            ushort itemPackage = GetPackageForItem(item.GetItemData().catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].AddItem(item);
            }
        }

        public void MoveItem(InventoryItem item, ushort destinationPackage)
        {
            ushort sourcePackage = item.itemPackage;

            if (!itemPackages.ContainsKey(sourcePackage) && !itemPackages.ContainsKey(destinationPackage))
                return;
            
            itemPackages[sourcePackage].MoveItem(item, itemPackages[destinationPackage]);
        }

        public void RemoveItem(uint catalogID)
        {
            RemoveItem(catalogID, 1);
        }

        public void RemoveItem(uint catalogID, int quantity)
        {
            RemoveItem(catalogID, quantity, 1);
        }

        public void RemoveItem(uint catalogID, int quantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].RemoveItem(catalogID, quantity, quality);
            }
        }

        public void RemoveItemAtSlot(ushort itemPackage, ushort slot)
        {
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].RemoveItemAtSlot(slot);
            }
        }

        public void RemoveItem(InventoryItem item)
        {
            itemPackages[item.itemPackage].RemoveItem(item);
        }

        public bool HasItem(uint catalogID)
        {
            return HasItem(catalogID, 1);
        }

        public bool HasItem(uint catalogID, int minQuantity)
        {
            return HasItem(catalogID, minQuantity, 1);
        }

        public bool HasItem(uint catalogID, int minQuantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                return itemPackages[itemPackage].HasItem(catalogID, minQuantity, quality);
            }
            return false;
        }

        public bool HasItem(InventoryItem item)
        {
            ushort itemPackage = GetPackageForItem(item.GetItemData().catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                //return itemPackages[itemPackage].HasItem(item);
                return false; //TODO FIX
            }
            else
                return false;
        }

        public InventoryItem GetItem(LuaUtils.ItemRefParam reference)
        {
            if (reference.actorId != actorId)
                return null;
            if (itemPackages.ContainsKey(reference.itemPackage))
            {
                return itemPackages[reference.itemPackage].GetItemAtSlot(reference.slot);
            }
            return null;
        }

        public ItemPackage GetItemPackage(ushort package)
        {
            if (itemPackages.ContainsKey(package))
                return itemPackages[package];
            else
                return null;
        }

        public ushort GetPackageForItem(uint catalogID)
        {
            ItemData data = Server.GetItemGamedata(catalogID);

            if (data == null)
                return ItemPackage.NORMAL;
            else
            {
                if (data.IsMoney())
                    return ItemPackage.CURRENCY_CRYSTALS;
                else if (data.IsImportant())
                    return ItemPackage.KEYITEMS;
                else
                    return ItemPackage.NORMAL;
            }
        }

        //public void removeItem(byUniqueId)
        //public void removeItem(byUniqueId, quantity)
        //public void removeItem(slot)
        //public void removeItem(slot, quantity)

        #endregion

    }
}

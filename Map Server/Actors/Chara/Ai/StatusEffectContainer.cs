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

using System;
using System.Collections.Generic;
using System.Linq;
using Meteor.Common;
using Meteor.Map.Actors;
using Meteor.Map.lua;
using Meteor.Map.packets.send.actor;
using Meteor.Map.packets.send.actor.battle;
using Meteor.Map.utils;

namespace Meteor.Map.actors.chara.ai
{
    class StatusEffectContainer
    {
        private Character owner;
        private readonly Dictionary<uint, StatusEffect> effects;
        public static readonly int MAX_EFFECTS = 20;
        private bool sendUpdate = false;
        private DateTime lastTick;// Do all effects tick at the same time like regen?
        private List<SubPacket> statusSubpackets;
        private ActorPropertyPacketUtil statusTimerPropPacketUtil;

        public StatusEffectContainer(Character owner)
        {
            this.owner = owner;
            this.effects = new Dictionary<uint, StatusEffect>();
            statusSubpackets = new List<SubPacket>();
            statusTimerPropPacketUtil = new ActorPropertyPacketUtil("charawork/Status", owner);
        }

        public void Update(DateTime tick)
        {
            CommandResultContainer resultContainer = new CommandResultContainer();

            //Regen/Refresh/Regain effects tick every 3 seconds
            if ((DateTime.Now - lastTick).Seconds >= 3)
            {
                RegenTick(tick, resultContainer);
                lastTick = DateTime.Now;
            }

            // list of effects to remove
            var removeEffects = new List<StatusEffect>();
            var effectsList = effects.Values;
            for (int i = effectsList.Count - 1; i >= 0; i--)
            {
                // effect's update function returns true if effect has completed
                if (effectsList.ElementAt(i).Update(tick, resultContainer))
                    removeEffects.Add(effectsList.ElementAt(i));
            }

            // remove effects from this list
            foreach (var effect in removeEffects)
            {
                RemoveStatusEffect(effect, resultContainer, effect.GetStatusLossTextId());
            }

            resultContainer.CombineLists();

            if (resultContainer.GetList().Count > 0)
            {
                owner.DoBattleAction(0, 0x7c000062, resultContainer.GetList());
            }
        }

        //regen/refresh/regain
        public void RegenTick(DateTime tick, CommandResultContainer resultContainer)
        {
            ushort dotTick = (ushort) owner.GetMod(Modifier.RegenDown);
            ushort regenTick = (ushort) owner.GetMod(Modifier.Regen);
            ushort refreshtick = (ushort) owner.GetMod(Modifier.Refresh);
            short regainTick = (short) owner.GetMod(Modifier.Regain);

            //DoTs tick before regen and the full dot damage is displayed, even if some or all of it is nullified by regen. Only effects like stoneskin actually alter the number shown
            if (dotTick > 0)
            {
                //Unsure why 10105 is the textId used
                //Also unsure why magicshield is used
                CommandResult action = new CommandResult(owner.actorId, 10105, (uint)(HitEffect.MagicEffectType | HitEffect.MagicShield | HitEffect.NoResist), dotTick);
                utils.BattleUtils.HandleStoneskin(owner, action);
                // todo: figure out how to make red numbers appear for enemies getting hurt by dots
                resultContainer.AddAction(action);
                owner.DelHP(action.amount, resultContainer);
            }

            //DoTs are the only effect to show numbers, so that doesnt need to be handled for these
            if (regenTick != 0)
                owner.AddHP(regenTick);

            if (refreshtick != 0)
                owner.AddMP(refreshtick);

            if (regainTick != 0)
                owner.AddTP(regainTick);
        }

        public bool HasStatusEffect(uint id)
        {
            return effects.ContainsKey(id);
        }

        public bool HasStatusEffect(StatusEffectId id)
        {
            return effects.ContainsKey((uint)id);
        }

        public bool AddStatusEffect(uint id, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30328)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            if (se != null)
            {
                worldmasterTextId = se.GetStatusGainTextId();
            }

            return AddStatusEffect(se, owner, actionContainer, worldmasterTextId);
        }

        public bool AddStatusEffect(uint id, byte tier, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30328)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            if (se != null)
            {
                se.SetTier(tier);
                worldmasterTextId = se.GetStatusGainTextId();
            }

            return AddStatusEffect(se, owner, actionContainer, worldmasterTextId);
        }

        public bool AddStatusEffect(uint id, byte tier, double magnitude, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30328)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            if (se != null)
            {
                se.SetMagnitude(magnitude);
                se.SetTier(tier);
                worldmasterTextId = se.GetStatusGainTextId();
            }

            return AddStatusEffect(se, owner, actionContainer, worldmasterTextId);
        }

        public bool AddStatusEffect(uint id, byte tier, double magnitude, uint duration, int tickMs, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30328)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            if (se != null)
            {
                se.SetDuration(duration);
                se.SetOwner(owner);
                worldmasterTextId = se.GetStatusGainTextId();
            }
            return AddStatusEffect(se ?? new StatusEffect(this.owner, id, magnitude, 3000, duration, tier), owner, actionContainer, worldmasterTextId);
        }

        public bool AddStatusEffect(StatusEffect newEffect, Character source, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30328)
        {
            /*
                worldMasterTextId
                32001 [@2B([@IF($E4($EB(1),$EB(2)),you,[@IF($E9(7),[@SHEETEN(xtx/displayName,2,$E9(7),1,1)],$EB(2))])])] [@IF($E4($EB(1),$EB(2)),resist,resists)] the effect of [@SHEET(xtx/status,$E8(11),3)].
                32002 [@SHEET(xtx/status,$E8(11),3)] fails to take effect.
            */

            var effect = GetStatusEffectById(newEffect.GetStatusEffectId());

            bool canOverwrite = false;
            if (effect != null)
            {
                var overwritable = effect.GetOverwritable();
                canOverwrite = (overwritable == (uint)StatusEffectOverwrite.Always) ||
                   (overwritable == (uint)StatusEffectOverwrite.GreaterOnly && (effect.GetDuration() < newEffect.GetDuration() || effect.GetMagnitude() < newEffect.GetMagnitude())) ||
                   (overwritable == (uint)StatusEffectOverwrite.GreaterOrEqualTo && (effect.GetDuration() <= newEffect.GetDuration() || effect.GetMagnitude() <= newEffect.GetMagnitude()));
            }

            if (canOverwrite || effect == null)
            {
                // send packet to client with effect added message
                if (newEffect != null && !newEffect.GetSilentOnGain())
                {
                    if (actionContainer != null)
                        actionContainer.AddAction(new CommandResult(owner.actorId, worldmasterTextId, newEffect.GetStatusEffectId() | (uint)HitEffect.StatusEffectType));
                }

                // wont send a message about losing effect here
                if (canOverwrite)
                    effects.Remove(newEffect.GetStatusEffectId());

                newEffect.SetStartTime(DateTime.Now);
                newEffect.SetEndTime(DateTime.Now.AddSeconds(newEffect.GetDuration()));
                newEffect.SetOwner(owner);

                if (effects.Count < MAX_EFFECTS)
                {
                    newEffect.CallLuaFunction(this.owner, "onGain", this.owner, newEffect, actionContainer);

                    effects.Add(newEffect.GetStatusEffectId(), newEffect);

                    if (!newEffect.GetHidden())
                    {
                        int index = 0;

                        //If effect is already in the list of statuses, get that index, otherwise find the first open index
                        if (owner.charaWork.status.Contains(newEffect.GetStatusId()))
                            index = Array.IndexOf(owner.charaWork.status, newEffect.GetStatusId());
                        else
                            index = Array.IndexOf(owner.charaWork.status, (ushort) 0);
                        
                        SetStatusAtIndex(index, newEffect.GetStatusId());
                        //Stance statuses need their time set to an extremely high number so their icon doesn't flash
                        //Adding getduration with them doesn't work because it overflows
                        uint time = (newEffect.GetFlags() & (uint) StatusEffectFlags.Stance) == 0 ? Utils.UnixTimeStampUTC(newEffect.GetEndTime()) : 0xFFFFFFFF;
                        SetTimeAtIndex(index, time);
                    }
                    owner.RecalculateStats();
                }
                return true;
            }
            return false;
        }

        //playEffect determines whether the effect of the animation that's going to play with actionContainer is going to play on owner
        //Generally, for abilities removing an effect, this is true and for effects removing themselves it's false.
        public bool RemoveStatusEffect(StatusEffect effect, CommandResultContainer actionContainer = null, ushort worldmasterTextId = 30331, bool playEffect = true)
        {
            bool removedEffect = false;
            if (effect != null && effects.ContainsKey(effect.GetStatusEffectId()))
            {
                // send packet to client with effect remove message
                if (!effect.GetSilentOnLoss())
                {
                    //Only send a message if we're using an actioncontainer and the effect normally sends a message when it's lost
                    if (actionContainer != null)
                        actionContainer.AddAction(new CommandResult(owner.actorId, worldmasterTextId, effect.GetStatusEffectId() | (playEffect ? 0 : (uint)HitEffect.StatusLossType)));
                }

                //hidden effects not in charawork
                var index = Array.IndexOf(owner.charaWork.status, effect.GetStatusId());
                if (!effect.GetHidden() && index != -1)
                {
                    SetStatusAtIndex(index, 0);
                    SetTimeAtIndex(index, 0);
                }

                // function onLose(actor, effect)
                effects.Remove(effect.GetStatusEffectId());
                effect.CallLuaFunction(owner, "onLose", owner, effect, actionContainer);
                owner.RecalculateStats();
                removedEffect = true;
            }

            return removedEffect;
        }

        public bool RemoveStatusEffect(uint effectId, CommandResultContainer resultContainer = null, ushort worldmasterTextId = 30331, bool playEffect = true)
        {
            return RemoveStatusEffect(GetStatusEffectById(effectId), resultContainer, worldmasterTextId, playEffect);
        }

        public StatusEffect CopyEffect(StatusEffect effect)
        {
            var newEffect = new StatusEffect(owner, effect);
            newEffect.SetOwner(owner);
            // todo: should source be copied too?
            return AddStatusEffect(newEffect, effect.GetSource()) ? newEffect : null;
        }

        public bool RemoveStatusEffectsByFlags(uint flags, CommandResultContainer resultContainer = null)
        {
            // build list of effects to remove
            var removeEffects = GetStatusEffectsByFlag(flags);

            // remove effects from main list
            foreach (var effect in removeEffects)
                RemoveStatusEffect(effect, resultContainer, effect.GetStatusLossTextId(), true);

            // removed an effect with one of these flags
            return removeEffects.Count > 0;
        }

        public StatusEffect GetStatusEffectById(uint id, byte tier = 0xFF)
        {
            StatusEffect effect;

            if (effects.TryGetValue(id, out effect) && effect.GetStatusEffectId() == id && (tier != 0xFF ? effect.GetTier() == tier : true))
                return effect;

            return null;
        }

        public List<StatusEffect> GetStatusEffectsByFlag(uint flag)
        {
            var list = new List<StatusEffect>();
            foreach (var effect in effects.Values)
                if ((effect.GetFlags() & flag) != 0)
                    list.Add(effect);

            return list;
        }

        public StatusEffect GetRandomEffectByFlag(uint flag)
        {
            var list = GetStatusEffectsByFlag(flag);

            if (list.Count > 0)
                return list[Program.Random.Next(list.Count)];

            return null;
        }

        // todo: why the fuck cant c# convert enums/
        public bool HasStatusEffectsByFlag(StatusEffectFlags flags)
        {
            return HasStatusEffectsByFlag((uint)flags);
        }

        public bool HasStatusEffectsByFlag(uint flag)
        {
            foreach (var effect in effects.Values)
            {
                if ((effect.GetFlags() & flag) != 0)
                    return true;
            }
            return false;
        }

        public IEnumerable<StatusEffect> GetStatusEffects()
        {
            return effects.Values;
        }

        void SaveStatusEffectsToDatabase(StatusEffectFlags removeEffectFlags = StatusEffectFlags.None)
        {
            if (owner is Player)
            {
                Database.SavePlayerStatusEffects((Player)owner);
            }
        }

        public void CallLuaFunctionByFlag(uint flag, string function, params object[] args)
        {
            var effects = GetStatusEffectsByFlag(flag);

            object[] argsWithEffect = new object[args.Length + 1];

            for (int i = 0; i < args.Length; i++)
                argsWithEffect[i + 1] = args[i];

            foreach (var effect in effects)
            {
                argsWithEffect[0] = effect;
                effect.CallLuaFunction(owner, function, argsWithEffect);
            }
        }

        //Sets the status id at an index.
        //Changing a status to another doesn't seem to work. If updating an index that already has an effect, set it to 0 first then to the correct status
        public void SetStatusAtIndex(int index, ushort statusId)
        {
            owner.charaWork.status[index] = statusId;

            statusSubpackets.Add(SetActorStatusPacket.BuildPacket(owner.actorId, (ushort)index, statusId));
            owner.updateFlags |= ActorUpdateFlags.Status;
        }

        public void SetTimeAtIndex(int index, uint time)
        {
            owner.charaWork.statusShownTime[index] = time;
            statusTimerPropPacketUtil.AddProperty($"charaWork.statusShownTime[{index}]");
            owner.updateFlags |= ActorUpdateFlags.StatusTime;
        }

        public List<SubPacket> GetStatusPackets()
        {
            return statusSubpackets;
        }

        public List<SubPacket> GetStatusTimerPackets()
        {
            return statusTimerPropPacketUtil.Done();
        }

        public void ResetPropPacketUtil()
        {
            statusTimerPropPacketUtil = new ActorPropertyPacketUtil("charaWork/status", owner);
        }

        //Overwrites effectToBeReplaced with a new status effect
        //Returns the message of the new effect being added
        //Doing this instead of simply calling remove then add so that the new effect is in the same slot as the old one
        //There should be a better way to do this
        //Currently causes the icons to blink whenb eing rpelaced
        public CommandResult ReplaceEffect(StatusEffect effectToBeReplaced, uint newEffectId, byte tier, double magnitude, uint duration)
        {
            StatusEffect newEffect = Server.GetWorldManager().GetStatusEffect(newEffectId);
            newEffect.SetTier(tier);
            newEffect.SetMagnitude(magnitude);
            newEffect.SetDuration(duration);
            newEffect.SetOwner(effectToBeReplaced.GetOwner());
            effectToBeReplaced.CallLuaFunction(owner, "onLose", owner, effectToBeReplaced);
            newEffect.CallLuaFunction(owner, "onGain", owner, newEffect);
            effects.Remove(effectToBeReplaced.GetStatusEffectId());

            newEffect.SetStartTime(DateTime.Now);
            newEffect.SetEndTime(DateTime.Now.AddSeconds(newEffect.GetDuration()));
            uint time = (newEffect.GetFlags() & (uint)StatusEffectFlags.Stance) == 0 ? Utils.UnixTimeStampUTC(newEffect.GetEndTime()) : 0xFFFFFFFF;
            int index = Array.IndexOf(owner.charaWork.status, effectToBeReplaced.GetStatusId());

            //owner.charaWork.status[index] = newEffect.GetStatusId();
            owner.charaWork.statusShownTime[index] = time;
            effects[newEffectId] = newEffect;
            
            SetStatusAtIndex(index, 0);

            //charawork/status
            SetStatusAtIndex(index, (ushort) (newEffectId - 200000));
            SetTimeAtIndex(index, time);

            return new CommandResult(owner.actorId, 30330, (uint) HitEffect.StatusEffectType | newEffectId);
        }
    }
}

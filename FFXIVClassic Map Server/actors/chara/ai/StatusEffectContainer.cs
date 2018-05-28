using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using System.Collections.ObjectModel;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai
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
            //Regen/Refresh/Regain effects tick every 3 seconds
            if ((DateTime.Now - lastTick).Seconds >= 3)
            {
                RegenTick(tick);
                lastTick = DateTime.Now;
            }
            // list of effects to remove

           // if (owner is Player) UpdateTimeAtIndex(4, 4294967295);
            var removeEffects = new List<StatusEffect>();
            for (int i = 0; i < effects.Values.Count; i++)
            {
                // effect's update function returns true if effect has completed
                if (effects.Values.ElementAt(i).Update(tick))
                     removeEffects.Add(effects.Values.ElementAt(i));

            }

            // remove effects from this list
            foreach (var effect in removeEffects)
            {
                RemoveStatusEffect(effect);
            }

            if (sendUpdate)
            {
                owner.zone.BroadcastPacketsAroundActor(owner, owner.GetActorStatusPackets());
                sendUpdate = false;
            }
        }

        //regen/refresh/regain
        public void RegenTick(DateTime tick)
        {
            ushort dotTick = (ushort) owner.GetMod(Modifier.RegenDown);
            ushort regenTick = (ushort) owner.GetMod(Modifier.Regen);
            ushort refreshtick = (ushort) owner.GetMod(Modifier.Refresh);
            short regainTick = (short) owner.GetMod(Modifier.Regain);

            //DoTs tick before regen and the full dot damage is displayed, even if some or all of it is nullified by regen. Only effects like stoneskin actually alter the number shown
            if (dotTick > 0)
            {
                BattleAction action = new BattleAction(owner.actorId, 30331, (uint)(HitEffect.HitEffectType | HitEffect.Hit), dotTick);
                utils.BattleUtils.HandleStoneskin(owner, action);
                // todo: figure out how to make red numbers appear for enemies getting hurt by dots
                //owner.DelHP(action.amount);
                utils.BattleUtils.DamageTarget(owner, owner, action, null);
                owner.DoBattleAction(0, 0, action);
            }

            //DoTs are the only effect to show numbers, so that doesnt need to be handled for these
            owner.AddHP(regenTick);
            owner.AddMP(refreshtick);
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

        public BattleAction AddStatusForBattleAction(uint id, byte tier = 1)
        {
            BattleAction action = null;

            if (AddStatusEffect(id, tier))
                action = new BattleAction(owner.actorId, 30328, id | (uint)HitEffect.StatusEffectType);

            return action;
        }

        public bool AddStatusEffect(uint id)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            return AddStatusEffect(se, owner);
        }

        public bool AddStatusEffect(uint id, byte tier)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            se.SetTier(tier);

            return AddStatusEffect(se, owner);
        }

        public bool AddStatusEffect(uint id,  byte tier, UInt64 magnitude)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);

            se.SetMagnitude(magnitude);
            se.SetTier(tier);

            return AddStatusEffect(se, owner);
        }

        public bool AddStatusEffect(uint id, byte tier, UInt64 magnitude, uint duration, int tickMs = 3000)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);
            if (se != null)
            {
                se.SetDuration(duration);
                se.SetStartTime(DateTime.Now);
                se.SetOwner(owner);
            }
            return AddStatusEffect(se ?? new StatusEffect(this.owner, id, magnitude, 3000, duration, tier), owner);
        }

        public bool AddStatusEffect(StatusEffect newEffect, Character source, bool silent = false, bool hidden = false)
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
                if (effect != null && (!silent  || !effect.GetSilent() || (effect.GetFlags() & (uint)StatusEffectFlags.Silent) == 0))
                {
                    // todo: send packet to client with effect added message
                }

                // wont send a message about losing effect here
                if (canOverwrite)
                    effects.Remove(newEffect.GetStatusEffectId());

                newEffect.SetStartTime(DateTime.Now);
                newEffect.SetEndTime(DateTime.Now.AddSeconds(newEffect.GetDuration()));
                newEffect.SetOwner(owner);

                if (effects.Count < MAX_EFFECTS)
                {
                    if(newEffect.script != null)
                        newEffect.CallLuaFunction(this.owner, "onGain", this.owner, newEffect);
                    else
                        LuaEngine.CallLuaStatusEffectFunction(this.owner, newEffect, "onGain", this.owner, newEffect);
                    effects.Add(newEffect.GetStatusEffectId(), newEffect);
                    //newEffect.SetSilent(silent);
                    newEffect.SetHidden(hidden);

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

        public bool RemoveStatusEffect(StatusEffect effect, bool silent = false)
        {
            bool removedEffect = false;
            if (effect != null && effects.ContainsKey(effect.GetStatusEffectId()))
            {
                // send packet to client with effect remove message
                if (!silent && !effect.GetSilent() && (effect.GetFlags() & (uint)StatusEffectFlags.Silent) == 0)
                {
                    owner.DoBattleAction(0, 0, new BattleAction(owner.actorId, 30331, effect.GetStatusEffectId()));
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
                if(effect.script != null)
                    effect.CallLuaFunction(owner, "onLose", owner, effect);
                else
                    LuaEngine.CallLuaStatusEffectFunction(this.owner, effect, "onLose", this.owner, effect);
                owner.RecalculateStats();
                sendUpdate = true;
                removedEffect = true;
            }

            return removedEffect;
        }

        public bool RemoveStatusEffect(uint effectId, bool silent = false)
        {
            bool removedEffect = false;
            foreach (var effect in effects.Values)
            {
                if (effect.GetStatusEffectId() == effectId)
                {
                    RemoveStatusEffect(effect, effect.GetSilent() || silent);
                    removedEffect = true;
                    break;
                }
            }

            return removedEffect;
        }


        //Remove status effect and return the battleaction message instead of sending it immediately
        public BattleAction RemoveStatusEffectForBattleAction(uint effectId, ushort worldMasterTextId = 30331)
        {
            BattleAction action = null;
            if (RemoveStatusEffect(effectId, true))
                action = new BattleAction(owner.actorId, worldMasterTextId, effectId);

            return action;
        }

        //Remove status effect and return the battleaction message instead of sending it immediately
        public BattleAction RemoveStatusEffectForBattleAction(StatusEffect effect, ushort worldMasterTextId = 30331)
        {
            BattleAction action = null;
            if (RemoveStatusEffect(effect, true))
                action = new BattleAction(owner.actorId, worldMasterTextId, effect.GetStatusEffectId());
            return action;
        }

        public StatusEffect CopyEffect(StatusEffect effect)
        {
            var newEffect = new StatusEffect(owner, effect);
            newEffect.SetOwner(owner);
            // todo: should source be copied too?
            return AddStatusEffect(newEffect, effect.GetSource()) ? newEffect : null;
        }

        public bool RemoveStatusEffectsByFlags(uint flags, bool silent = false)
        {
            // build list of effects to remove
            var removeEffects = GetStatusEffectsByFlag(flags);

            // remove effects from main list
            foreach (var effect in removeEffects)
                RemoveStatusEffect(effect, silent);

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
        public BattleAction ReplaceEffect(StatusEffect effectToBeReplaced, uint newEffectId, byte tier, double magnitude, uint duration)
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

            return new BattleAction(owner.actorId, 30328, (uint) HitEffect.StatusEffectType | newEffectId);
        }
    }
}
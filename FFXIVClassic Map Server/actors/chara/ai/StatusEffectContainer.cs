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

        public StatusEffectContainer(Character owner)
        {
            this.owner = owner;
            this.effects = new Dictionary<uint, StatusEffect>();
        }

        public void Update(DateTime tick)
        {
            // list of effects to remove
            var removeEffects = new List<StatusEffect>();
            foreach (var effect in effects.Values)
            {
                // effect's update function returns true if effect has completed
                if (effect.Update(tick))
                    removeEffects.Add(effect);
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

        public bool HasStatusEffect(uint id)
        {
            return effects.ContainsKey(id);
        }

        public bool HasStatusEffect(StatusEffectId id)
        {
            return effects.ContainsKey((uint)id);
        }

        public bool AddStatusEffect(uint id, UInt64 magnitude, uint tickMs, uint duration, byte tier = 0)
        {
            var se = Server.GetWorldManager().GetStatusEffect(id);
            if (se != null)
            {
                se.SetDuration(duration);
                se.SetStartTime(DateTime.Now);
                se.SetOwner(owner);
            }
            return AddStatusEffect(se ?? new StatusEffect(this.owner, id, magnitude, tickMs, duration, tier), owner);
        }

        public bool AddStatusEffect(StatusEffect newEffect, Character source, bool silent = false, bool hidden = false)
        {
            /*
                worldMasterTextId
                32001 [@2B([@IF($E4($EB(1),$EB(2)),you,[@IF($E9(7),[@SHEETEN(xtx/displayName,2,$E9(7),1,1)],$EB(2))])])] [@IF($E4($EB(1),$EB(2)),resist,resists)] the effect of [@SHEET(xtx/status,$E8(11),3)].
                32002 [@SHEET(xtx/status,$E8(11),3)] fails to take effect.
            */

            if (HasStatusEffect(newEffect.GetStatusEffectId()) && (newEffect.GetFlags() & (uint)StatusEffectFlags.Stance) != 0)
            {
                RemoveStatusEffect(newEffect);
                return false;
            }

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
                newEffect.SetOwner(owner);

                if (effects.Count < MAX_EFFECTS)
                {
                    if(newEffect.script != null)
                        newEffect.CallLuaFunction(this.owner, "onGain", this.owner, newEffect);
                    else
                        LuaEngine.CallLuaStatusEffectFunction(this.owner, newEffect, "onGain", this.owner, newEffect);
                    effects.Add(newEffect.GetStatusEffectId(), newEffect);
                    newEffect.SetSilent(silent);
                    newEffect.SetHidden(hidden);

                    if (!newEffect.GetHidden())
                    {
                        int index = 0;
                        if (owner.charaWork.status.Contains(newEffect.GetStatusId()))
                            index = Array.IndexOf(owner.charaWork.status, newEffect.GetStatusId());
                        else
                            index = Array.IndexOf(owner.charaWork.status, (ushort) 0);

                        owner.charaWork.status[index] = newEffect.GetStatusId();

                        //Stance statuses need their time set to an extremely high number so their icon doesn't flash
                        //Adding getduration with them doesn't work because it overflows
                        uint time = (newEffect.GetFlags() & (uint) StatusEffectFlags.Stance) == 0 ? Utils.UnixTimeStampUTC() + (newEffect.GetDuration()) : 0xFFFFFFFF;
                        owner.charaWork.statusShownTime[index] = time;
                        this.owner.zone.BroadcastPacketAroundActor(this.owner, SetActorStatusPacket.BuildPacket(this.owner.actorId, (ushort)index, (ushort)newEffect.GetStatusId()));
                    }

                    {
                        owner.zone.BroadcastPacketsAroundActor(owner, owner.GetActorStatusPackets());
                    }
                    owner.RecalculateStats();
                }
                return true;
            }
            return false;
        }

        public void RemoveStatusEffect(StatusEffect effect, bool silent = false)
        {
            if (effect != null && effects.ContainsKey(effect.GetStatusEffectId()) )
            {
                // send packet to client with effect remove message
                if (!silent && !effect.GetSilent() || (effect.GetFlags() & (uint)StatusEffectFlags.Silent) == 0)
                {
                    // todo: send packet to client with effect added message
                }

                //hidden effects not in charawork
                if(!effect.GetHidden())
                {
                    var index = Array.IndexOf(owner.charaWork.status, effect.GetStatusId());

                    owner.charaWork.status[index] = 0;
                    owner.charaWork.statusShownTime[index] = 0;
                    this.owner.zone.BroadcastPacketAroundActor(this.owner, SetActorStatusPacket.BuildPacket(owner.actorId, (ushort)index, (ushort)0));
                }
                // function onLose(actor, effect)
                effects.Remove(effect.GetStatusEffectId());
                if(effect.script != null)
                    effect.CallLuaFunction(owner, "onLose", owner, effect);
                else
                    LuaEngine.CallLuaStatusEffectFunction(this.owner, effect, "onLose", this.owner, effect);
                owner.RecalculateStats();
                sendUpdate = true;
            }
        }

        public void RemoveStatusEffect(uint effectId, bool silent = false)
        {
            foreach (var effect in effects.Values)
            {
                if (effect.GetStatusEffectId() == effectId)
                {
                    RemoveStatusEffect(effect, effect.GetSilent() || silent);
                    break;
                }
            }
        }

        public StatusEffect CopyEffect(StatusEffect effect)
        {
            var newEffect = new StatusEffect(this.owner, effect);
            newEffect.SetOwner(this.owner);
            // todo: should source be copied too?
            return AddStatusEffect(newEffect, effect.GetSource()) ? newEffect : null;
        }

        public bool RemoveStatusEffectsByFlags(uint flags, bool silent = false)
        {
            // build list of effects to remove
            var removeEffects = new List<StatusEffect>();
            foreach (var effect in effects.Values)
                if ((effect.GetFlags() & flags) != 0)
                    removeEffects.Add(effect);

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
            {
                if ((effect.GetFlags() & flag) != 0)
                {
                    list.Add(effect);
                }
            }
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
    }
}

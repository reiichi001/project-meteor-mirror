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

        public bool AddStatusEffect(uint id, UInt64 magnitude, double tickMs, double durationMs, byte tier = 0)
        {
            return AddStatusEffect(new StatusEffect(this.owner, id, magnitude, (uint)(tickMs * 1000), (uint)(durationMs * 1000), tier), owner);
        }

        public bool AddStatusEffect(StatusEffect newEffect, Character source, bool silent = false)
        {
            // todo: check flags/overwritable and add effect to list
            var effect = GetStatusEffectById(newEffect.GetStatusEffectId());
            bool canOverwrite = false;
            if (effect != null)
            {
                var overwritable = effect.GetOverwritable();
                canOverwrite = (overwritable == (uint)StatusEffectOverwrite.Always) ||
                   (overwritable == (uint)StatusEffectOverwrite.GreaterOnly && (effect.GetDurationMs() < newEffect.GetDurationMs() || effect.GetMagnitude() < newEffect.GetMagnitude())) ||
                   (overwritable == (uint)StatusEffectOverwrite.GreaterOrEqualTo && (effect.GetDurationMs() <= newEffect.GetDurationMs() || effect.GetMagnitude() <= newEffect.GetMagnitude()));
            }

            if (canOverwrite || effect == null)
            {
                // send packet to client with effect added message
                if (!silent || !effect.GetSilent() || (effect.GetFlags() & (uint)StatusEffectFlags.Silent) == 0)
                {
                    // todo: send packet to client with effect added message
                    foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                        player.QueuePacket(packets.send.actor.battle.BattleActionX01Packet.BuildPacket(player.actorId, newEffect.GetSource().actorId, newEffect.GetOwner().actorId, 0x7678, 0, 0, newEffect.GetStatusId(), 0, 0));
                }

                // wont send a message about losing effect here
                if (canOverwrite)
                    effects.Remove(newEffect.GetStatusEffectId());

                if (effects.Count < MAX_EFFECTS)
                {
                    effects.Add(newEffect.GetStatusEffectId(), newEffect);
                    newEffect.SetSilent(silent);
                    // todo: this is retarded..
                    {
                        var index = Array.IndexOf(effects.Values.ToArray(), newEffect);
                        owner.charaWork.status[index] = newEffect.GetStatusId();
                        owner.charaWork.statusShownTime[index] = Utils.UnixTimeStampUTC() + (newEffect.GetDurationMs() / 1000);
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
            if (effects.ContainsKey(effect.GetStatusEffectId()))
            {
                // send packet to client with effect remove message
                if (!silent || !effect.GetSilent() || (effect.GetFlags() & (uint)StatusEffectFlags.Silent) == 0)
                {
                    // todo: send packet to client with effect added message
                    foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                        player.QueuePacket(packets.send.actor.battle.BattleActionX01Packet.BuildPacket(player.actorId, owner.actorId, owner.actorId, 0x7679, 0, 0, effect.GetStatusId(), 0, 0));
                }

                // todo: this is retarded..
                {
                    var index = Array.IndexOf(effects.Values.ToArray(), effect);
                    owner.charaWork.status[index] = 0;
                    owner.charaWork.statusShownTime[index] = uint.MaxValue;
                    this.owner.zone.BroadcastPacketAroundActor(this.owner, SetActorStatusPacket.BuildPacket(owner.actorId, (ushort)index, (ushort)0));
                }
                // function onLose(actor, effect)
                LuaEngine.CallLuaStatusEffectFunction(this.owner, effect, "onLose", this.owner, effect);
                effects.Remove(effect.GetStatusEffectId());
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
                    RemoveStatusEffect(effect, silent);
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

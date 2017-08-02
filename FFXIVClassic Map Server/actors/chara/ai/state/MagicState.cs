using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class MagicState : State
    {

        private Ability spell;

        public MagicState(Character owner, Character target, ushort spellId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            // todo: lookup spell from global table
            this.spell = Server.GetWorldManager().GetAbility(spellId);

            if (spell != null)
            {
                if (spell.CanPlayerUse(owner, target))
                    OnStart();
            }
        }

        public override void OnStart()
        {
            // todo: check within attack range

            owner.LookAt(target);
        }

        public override bool Update(DateTime tick)
        {
            TryInterrupt();

            if (interrupt)
            {
                OnInterrupt();
                return true;
            }

            // todo: check weapon delay/haste etc and use that
            if ((tick - startTime).TotalMilliseconds >= 0)
            {
                OnComplete();
                return true;
            }
            return false;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
        }

        public override void OnComplete()
        {
            //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
            isCompleted = true;
        }

        public override void TryInterrupt()
        {
            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction);
                uint effectId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    effectId = list[0].GetStatusEffectId();
                }
                // todo: which is actually the swing packet
                //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
                //owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
                //errorPacket = null;
                interrupt = true;
                return;
            }

            interrupt = !CanAttack();
        }

        private bool CanAttack()
        {
            if (target == null)
            {
                return false;
            }
            // todo: shouldnt need to check if owner is dead since all states would be cleared
            if (owner.aiContainer.IsDead() || target.aiContainer.IsDead())
            {
                return false;
            }
            else if (!owner.aiContainer.GetTargetFind().CanTarget(target, false, true))
            {
                return false;
            }
            else if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) >= 7.5f)
            {
                owner.aiContainer.pathFind?.PreparePath(target.positionX, target.positionY, target.positionZ, 2.5f, 4);
                return false;
            }
            return true;
        }
    }
}

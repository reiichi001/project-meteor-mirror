using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class AttackState : State
    {
        public AttackState(Character owner, Character target) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            // todo: should handle everything here instead of on next tick..
        }

        public override void OnStart()
        {

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
            var damage = utils.AttackUtils.CalculateDamage(owner, target);

            // onAttack(actor, target, damage)
            lua.LuaEngine.CallLuaBattleAction(owner, "onAttack", false, owner, target, damage);

            //var packet = BattleAction1Packet.BuildPacket(owner.actorId, target.actorId);

            // todo: find a better place to put this?
            if (owner.GetState() != SetActorStatePacket.MAIN_STATE_ACTIVE)
                owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);

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
                //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, 0, 0);
                //owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
                //errorPacket = null;
                interrupt = true;
                return;
            }

            interrupt = !CanAttack();
        }

        private bool CanAttack()
        {
            // todo: shouldnt need to check if owner is dead since all states would be cleared
            if (owner.aiContainer.IsDead() || target.aiContainer.IsDead())
            {
                return false;
            }
            else if (target.zone != owner.zone)
            {
                return false;
            }
            else if (target is Player && ((Player)target).playerSession.isUpdatesLocked)
            {
                return false;
            }
            return true;
        }
    }
}

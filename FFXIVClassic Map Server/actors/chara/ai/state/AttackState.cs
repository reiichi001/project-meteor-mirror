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
    class AttackState : State
    {
        private DateTime attackTime;

        public AttackState(Character owner, Character target) :
            base(owner, target)
        {
            this.canInterrupt = false;
            this.startTime = DateTime.Now;

            owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);
            ChangeTarget(target);
            attackTime = startTime;
            owner.aiContainer.pathFind?.Clear();
            // todo: should handle everything here instead of on next tick..
        }

        public override void OnStart()
        {
            // todo: check within attack range
        }

        public override bool Update(DateTime tick)
        {
            /*
            TryInterrupt();

            if (interrupt)
            {
                OnInterrupt();
                return true;
            }
            */

            if ((target == null || owner.target != target || owner.target?.actorId != owner.currentLockedTarget) && owner.isAutoAttackEnabled)
                owner.aiContainer.ChangeTarget(target = owner.zone.FindActorInArea<Character>(owner.currentLockedTarget));

            if (target == null || target.IsDead())
            {
                if (owner is BattleNpc)
                    target = ((BattleNpc)owner).hateContainer.GetMostHatedTarget();
            }
            else
            {
                if (IsAttackReady())
                {
                    if (CanAttack())
                    {
                        TryInterrupt();

                        // todo: check weapon delay/haste etc and use that
                        if (!interrupt)
                        {
                            OnComplete();
                        }
                        else
                        {

                        }
                        SetInterrupted(false);
                    }
                    else
                    {
                        // todo: handle interrupt/paralyze etc
                    }
                    attackTime = DateTime.Now.AddMilliseconds(owner.GetAttackDelayMs());
                }
            }
            return false;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
        }

        public override void OnComplete()
        {
            // todo: possible underflow
            BattleAction action = new BattleAction(target.actorId, 0x765D, (uint) HitEffect.Hit, 0, (byte) HitDirection.None);
            errorResult = null;

            //var packet = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, target.actorId, (uint)0x19001000, (uint)0x8000604, (ushort)0x765D, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, (byte)0x1);
            
            // HitDirection (auto attack shouldnt need this afaik)

            // todo: implement auto attack damage bonus in Character.OnAttack
            /*
              ≪Auto-attack Damage Bonus≫
              Class        Bonus 1       Bonus 2
              Pugilist     Intelligence  Strength
              Gladiator    Mind          Strength
              Marauder     Vitality      Strength
              Archer       Dexterity     Piety
              Lancer       Piety         Strength
              Conjurer     Mind          Piety
              Thaumaturge  Mind          Piety
              * The above damage bonus also applies to “Shot” attacks by archers.
             */

            owner.OnAttack(this, action, ref errorResult);
            // handle paralyze/intimidate/sleep/whatever in character thing
            owner.DoBattleAction((ushort)BattleActionX01PacketCommand.Attack, action.animation, errorResult == null ? action : errorResult);            
           
            //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
        }

        public override void TryInterrupt()
        {
            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction);
                uint statusId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    statusId = list[0].GetStatusId();
                }
                // todo: which is actually the swing packet
                //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, statusId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
                //owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
                //errorPacket = null;
                interrupt = true;
                return;
            }

            interrupt = !CanAttack();
        }

        private bool IsAttackReady()
        {
            // todo: this enforced delay should really be changed if it's not retail..
            return Program.Tick >= attackTime && Program.Tick >= owner.aiContainer.GetLastActionTime().AddSeconds(1);
        }

        private bool CanAttack()
        {
            if (!owner.isAutoAttackEnabled)
            {
                return false;
            }

            if (target == null)
            {
                return false;
            }
            // todo: shouldnt need to check if owner is dead since all states would be cleared
            if (owner.aiContainer.IsDead() || target.aiContainer.IsDead())
            {
                target = null;
                return false;
            }
            else if (!owner.aiContainer.GetTargetFind().CanTarget(target, false, true))
            {
                return false;
            }
            // todo: use a mod for melee range
            else if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) > owner.GetAttackRange())
            {
                if (owner is Player)
                {
                    ((Player)owner).SendGameMessage(Server.GetWorldManager().GetActor(), 32539, 0x20);
                }
                return false;
            }
            return true;
        }

        public override void Cleanup()
        {
            if (owner.IsDead())
                owner.Disengage();
        }

        public override bool CanChangeState()
        {
            return true;
        }
    }
}

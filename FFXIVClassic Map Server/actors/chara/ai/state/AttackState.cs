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
            this.canInterrupt = true;
            this.startTime = DateTime.Now;

            owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);
            owner.aiContainer.ChangeTarget(target);
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
            if (target == null || owner.target != target || owner.target?.actorId != owner.currentLockedTarget)
                owner.aiContainer.ChangeTarget(target = Server.GetWorldManager().GetActorInWorld(owner.currentLockedTarget == 0xC0000000 ? owner.currentTarget : owner.currentLockedTarget) as Character);

            if (target == null || target.IsDead())
            {
                //if (owner.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER)
                //    target = ((BattleNpc)owner).hateContainer.GetMostHatedTarget();
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
            BattleAction action = new BattleAction();
            errorPacket = null;

            //var packet = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, target.actorId, (uint)0x19001000, (uint)0x8000604, (ushort)0x765D, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, (byte)0x1);
            action.animation = 0x19001000;
            action.targetId = target.actorId;
            action.effectId = (uint)HitEffect.Hit;
            action.worldMasterTextId = 0x765D;
            action.param = (byte)HitDirection.None; // HitDirection (auto attack shouldnt need this afaik)

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

            owner.OnAttack(this, action, ref errorPacket);
            // handle paralyze/intimidate/sleep/whatever in character thing
            if (errorPacket == null)
                owner.zone.BroadcastPacketAroundActor(owner, BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, action.targetId, action.animation,
                0x8000000 | action.effectId, action.worldMasterTextId, (ushort)BattleActionX01PacketCommand.Attack, action.amount, action.param)
                );
            else
                owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
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
            return Program.Tick >= attackTime;
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
                if (owner.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
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

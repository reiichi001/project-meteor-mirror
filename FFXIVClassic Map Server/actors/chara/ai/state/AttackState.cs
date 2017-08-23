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
        private int damage = 0;
        private bool tooFar = false;
        private DateTime attackTime;

        public AttackState(Character owner, Character target) :
            base(owner, target)
        {
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);
            owner.aiContainer.ChangeTarget(target);
            this.startTime = DateTime.Now;
            attackTime = startTime;
            owner.aiContainer.pathFind?.Clear();
            // todo: should handle everything here instead of on next tick..
        }

        public override void OnStart()
        {
            // todo: check within attack range
            
            owner.LookAt(target);
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
            if (owner.target == null || target.IsDead())
            {
                return true;
            }
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
            }
            return false;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
        }

        public override void OnComplete()
        {
            damage = utils.AttackUtils.CalculateDamage(owner, target);

            // onAttack(actor, target, damage)
            utils.BattleUtils.DamageTarget(owner, target, damage);
            lua.LuaEngine.CallLuaBattleAction(owner, "onAttack", false, owner, target, damage);

            {
                foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                {
                    var packet = BattleActionX01Packet.BuildPacket(player.actorId, owner.actorId, target.actorId, 0, 537094006, 0, 27260, (ushort)damage, 0);
                    player.QueuePacket(packet);
                    Program.Log.Error("asudyaisydaisydaioysdaisydaiosdyaiosuydaisydiaosydioasydaiusdyaisduy");
                    packet.DebugPrintSubPacket();
                }
                if (target is Player)
                    ((Player)target).SendPacket("139_attack");
            }
            target.DelHP((short)damage);
            attackTime = attackTime.AddMilliseconds(owner.GetAttackDelayMs());
            owner.LookAt(target);
           //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
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
                return false;
            }
            else if (!owner.aiContainer.GetTargetFind().CanTarget(target, false, true))
            {
                return false;
            }
            else if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) >= 7.5f)
            {
                //owner.aiContainer.GetpathFind?.PreparePath(target.positionX, target.positionY, target.positionZ, 2.5f, 4);
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

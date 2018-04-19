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
        }

        public override void OnStart()
        {

        }

        public override bool Update(DateTime tick)
        {
            if ((target == null || owner.target != target || owner.target?.actorId != owner.currentLockedTarget) && owner.isAutoAttackEnabled)
                owner.aiContainer.ChangeTarget(target = owner.zone.FindActorInArea<Character>(owner.currentTarget));

            if (target == null || target.IsDead())
            {
                if (owner.IsMonster() || owner.IsAlly())
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
            if (errorResult != null)
            {
                owner.zone.BroadcastPacketAroundActor(owner, BattleActionX01Packet.BuildPacket(errorResult.targetId, errorResult.animation, 0x765D, errorResult));
                errorResult = null;
            }
        }

        public override void OnComplete()
        {
            //BattleAction action = new BattleAction(target.actorId, 0x765D, (uint) HitEffect.Hit, 0, (byte) HitDirection.None);
            errorResult = null;

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
            // handle paralyze/intimidate/sleep/whatever in Character.OnAttack


            // todo: Change this to use a BattleCommand like the other states

            //List<BattleAction> actions = new List<BattleAction>();
            BattleActionContainer actions = new BattleActionContainer();
            target.SetMod((uint) Modifier.MinimumHpLock, 0);
            
            var i = 0;
            for (int hitNum = 0; hitNum < 1 /* owner.GetMod((uint) Modifier.HitCount)*/; hitNum++)
            {
                BattleAction action = new BattleAction(target.actorId, 0x765D, (uint)HitEffect.Hit, 100, (byte)HitDirection.None, (byte) hitNum);
                action.commandType = CommandType.AutoAttack;
                action.actionType = ActionType.Physical;
                action.actionProperty = (ActionProperty) owner.GetMod(Modifier.AttackType);
                // evasion, miss, dodge, etc to be handled in script, calling helpers from scripts/weaponskills.lua
                // temporary evade/miss/etc function to test hit effects
                action.DoAction(owner, target, null, actions);
            }

            // todo: this is fuckin stupid, probably only need *one* error packet, not an error for each action
            BattleAction[] errors = (BattleAction[])actions.GetList().ToArray().Clone();
            BattleAction error = null;// new BattleAction(0, null, 0, 0);
            //owner.DoActions(null, actions.GetList(), ref error);
            //owner.OnAttack(this, actions[0], ref errorResult);
            owner.DoBattleAction(22104, 0x19001000, actions.GetList());
        }

        public override void TryInterrupt()
        {
            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAttack))
            {
                // todo: sometimes paralyze can let you attack, calculate proc rate
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAttack);
                uint statusId = 0;
                if (list.Count > 0)
                {
                    statusId = list[0].GetStatusId();
                }
                interrupt = true;
                return;
            }

            interrupt = !CanAttack();
        }

        private bool IsAttackReady()
        {
            // todo: this enforced delay should really be changed if it's not retail..
            return Program.Tick >= attackTime && Program.Tick >= owner.aiContainer.GetLastActionTime();
        }

        private bool CanAttack()
        {
            if (!owner.isAutoAttackEnabled || target.allegiance == owner.allegiance)
            {
                return false;
            }

            if (target == null)
            {
                return false;
            }
            // todo: shouldnt need to check if owner is dead since all states would be cleared
            if (owner.IsDead() || target.IsDead())
            {
                if (owner.IsMonster() || owner.IsAlly())
                    ((BattleNpc)owner).hateContainer.ClearHate(target);

                owner.aiContainer.ChangeTarget(null);
                return false;
            }
            else if (!owner.IsValidTarget(target, ValidTarget.Enemy) || !owner.aiContainer.GetTargetFind().CanTarget(target, false, true))
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

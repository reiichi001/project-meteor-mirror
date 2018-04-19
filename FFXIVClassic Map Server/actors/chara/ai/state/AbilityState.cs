using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.packets.send;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class AbilityState : State
    {

        private BattleCommand skill;

        public AbilityState(Character owner, Character target, ushort skillId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            this.skill = Server.GetWorldManager().GetBattleCommand(skillId);
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "ability", "onAbilityPrepare", owner, target, skill);

            this.target = skill.GetMainTarget(owner, target);

            if (returnCode == 0)
            {
                OnStart();
            }
            else
            {
                errorResult = null;
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "ability", "onAbilityStart", owner, target, skill);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new BattleAction(owner.actorId, (ushort)(returnCode == -1 ? 32558 : returnCode), 0);
            }
            else
            {
                //owner.LookAt(target);
            }
        }

        public override bool Update(DateTime tick)
        {
            if (skill != null)
            {
                TryInterrupt();

                if (interrupt)
                {
                    OnInterrupt();
                    return true;
                }

                // todo: check weapon delay/haste etc and use that
                var actualCastTime = skill.castTimeMs;

                if ((tick - startTime).Milliseconds >= skill.castTimeMs)
                {
                    OnComplete();
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
            if (errorResult != null)
            {
                owner.DoBattleAction(skill.id, errorResult.animation, errorResult);
                errorResult = null;
            }
        }

        public override void OnComplete()
        {
            bool hitTarget = false;

            skill.targetFind.FindWithinArea(target, skill.validTarget, skill.aoeTarget);
            isCompleted = true;

            owner.DoBattleCommand(skill, "ability");
        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAbility))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAbility);
                uint effectId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    effectId = list[0].GetStatusEffectId();
                }
                interrupt = true;
                return;
            }

            interrupt = !CanUse();
        }

        private bool CanUse()
        {
            return skill.IsValidMainTarget(owner, target);
        }

        public BattleCommand GetWeaponSkill()
        {
            return skill;
        }

        public override void Cleanup()
        {
            owner.aiContainer.UpdateLastActionTime(skill.animationDurationSeconds);
        }
    }
}

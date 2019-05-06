using System;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor.battle;

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

            this.target = target != null ? target : owner;

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
                errorResult = new CommandResult(owner.actorId, (ushort)(returnCode == -1 ? 32558 : returnCode), 0);
            }
            else
            {
                if (!skill.IsInstantCast())
                {
                    float castTime = skill.castTimeMs;

                    // command casting duration
                    if (owner is Player)
                    {
                        // todo: modify spellSpeed based on modifiers and stuff
                        ((Player)owner).SendStartCastbar(skill.id, Utils.UnixTimeStampUTC(DateTime.Now.AddMilliseconds(castTime)));
                    }
                    owner.GetSubState().chantId = 0xf0;
                    owner.SubstateModified();
                    //You ready [skill] (6F000002: BLM, 6F000003: WHM, 0x6F000008: BRD)
                    owner.DoBattleAction(skill.id, (uint)0x6F000000 | skill.castType, new CommandResult(target.actorId, 30126, 1, 0, 1));
                }
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

                if ((tick - startTime).TotalMilliseconds >= skill.castTimeMs)
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
            owner.LookAt(target);
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
            owner.GetSubState().chantId = 0x0;
            owner.SubstateModified();
            owner.aiContainer.UpdateLastActionTime(skill.animationDurationSeconds);
        }
    }
}

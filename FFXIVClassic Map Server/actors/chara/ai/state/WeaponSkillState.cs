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
    class WeaponSkillState : State
    {

        private BattleCommand skill;

        public WeaponSkillState(Character owner, Character target, ushort skillId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            this.skill = Server.GetWorldManager().GetBattleCommand(skillId);
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onSkillPrepare", owner, target, skill);

            if (returnCode == 0 && owner.CanWeaponSkill(target, skill))
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
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onSkillStart", owner, target, skill);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new BattleAction(owner.actorId, (ushort)(returnCode == -1 ? 32558 : returnCode), 0);
            }
            else
            {
                owner.LookAt(target);
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
                var actualCastTime = skill.castTimeSeconds;

                if ((tick - startTime).TotalSeconds >= skill.castTimeSeconds)
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
            skill.targetFind.FindWithinArea(target, skill.validTarget, skill.aoeTarget);
            isCompleted = true;
            var targets = skill.targetFind.GetTargets();

            BattleAction[] actions = new BattleAction[targets.Count];

            var i = 0;
            foreach (var chara in targets)
            {
                var action = new BattleAction(chara.actorId, skill.worldMasterTextId, (uint)HitEffect.Hit, 0, 1, 1);                              
                // evasion, miss, dodge, etc to be handled in script, calling helpers from scripts/weaponskills.lua
                action.amount = (ushort)lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onSkillFinish", owner, target, skill, action);
                actions[i++] = action;
                chara.Engage(chara.actorId, 1);
            }

            // todo: this is fuckin stupid, probably only need *one* error packet, not an error for each action
            var errors = (BattleAction[])actions.Clone();
            owner.OnWeaponSkill(this, actions, ref errors);            
            owner.DoBattleAction(skill.id, skill.battleAnimation, actions);
        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

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
                interrupt = true;
                return;
            }

            interrupt = !CanUse();
        }

        private bool CanUse()
        {
            return owner.CanWeaponSkill(target, skill) && skill.IsValidTarget(owner, target);
        }

        public BattleCommand GetWeaponSkill()
        {
            return skill;
        }

        public override void Cleanup()
        {
            owner.aiContainer.UpdateLastActionTime();
        }
    }
}

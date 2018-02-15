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
        private HitDirection hitDirection;
        public WeaponSkillState(Character owner, Character target, ushort skillId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            //this.target = skill.targetFind.
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
                hitDirection = owner.GetHitDirection(target);

                //Do positionals and combo effects first because these can influence accuracy and amount of targets/numhits, which influence the rest of the steps
                //If there is no positon required or if the position bonus should be activated
                if ((skill.positionBonus & utils.BattleUtils.ConvertHitDirToPosition(hitDirection)) == skill.positionBonus)
                {
                    //If there is a position bonus
                    if (skill.positionBonus != BattleCommandPositionBonus.None)
                        //lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onPositional", owner, target, skill);
                        skill.CallLuaFunction(owner, "onPositional", owner, target, skill);

                    //Combo stuff
                    if (owner is Player p)
                    {
                        //If skill is part of owner's class/job, it can be used in a combo
                        if (skill.job == p.GetClass() || skill.job == p.GetCurrentClassOrJob())
                        {
                            //If owner is a player and the skill being used is part of the current combo
                            if (p.playerWork.comboNextCommandId[0] == skill.id || p.playerWork.comboNextCommandId[1] == skill.id)
                            {
                                lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onCombo", owner, target, skill);
                                skill.CallLuaFunction(owner, "onCombo", owner, target, skill);
                                skill.isCombo = true;
                            }
                            //or if this just the start of a combo
                            else if (skill.comboStep == 1)
                                skill.isCombo = true;
                        }
                    }
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
            var targets = skill.targetFind.GetTargets();

            //Need a variable size list of actions because status effects may add extra actions
            //BattleAction[] actions = new BattleAction[targets.Count * skill.numHits];
            List<BattleAction> actions = new List<BattleAction>();
            List<StatusEffect> effects = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.ActivateOnAttack);

            //modify skill based on status effects
            foreach (var effect in effects)
                effect.CallLuaFunction(owner, "onWeaponSkill", skill);

            //Now that combos and positionals bonuses are done, we can calculate hits/crits/etc and damage for each action
            foreach (var chara in targets)
            {
                for (int hitNum = 0; hitNum < skill.numHits; hitNum++)
                {
                    var action = new BattleAction(chara.actorId, skill.worldMasterTextId, 0, 0, (byte) hitDirection, 1);
                    //For older versions this will need to be in the database for magic weaponskills
                    action.battleActionType = BattleActionType.AttackPhysical;
                    //uncached script
                    lua.LuaEngine.CallLuaBattleCommandFunction(owner, skill, "weaponskill", "onSkillFinish", owner, target, skill, action);

                    //cached script
                    //skill.CallLuaFunction(owner, "onSkillFinish", owner, target, skill, action);
                    action.hitNum = (byte)hitNum;

                    if (action.hitType > HitType.Evade)
                        hitTarget = true;

                    actions.AddRange(action.GetAllActions());
                }
            }

            // todo: this is fuckin stupid, probably only need *one* error packet, not an error for each action
            BattleAction[] errors = (BattleAction[]) actions.ToArray().Clone();
            owner.OnWeaponSkill(this, actions.ToArray(), skill, ref errors);
            owner.DoBattleAction(skill.id, skill.battleAnimation, actions);
            owner.statusEffects.RemoveStatusEffectsByFlags((uint) StatusEffectFlags.LoseOnAttacking);

            //Now that we know if we hit the target we can check if the combo continues
            if (owner is Player player)
                if (skill.isCombo && hitTarget)
                    player.SetCombos(skill.comboNextCommandId);
                else
                    player.SetCombos();
        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventWeaponSkill))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventWeaponSkill);
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
            return owner.CanWeaponSkill(target, skill) && skill.IsValidMainTarget(owner, target);
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

/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using Meteor.Common;
using Meteor.Map.Actors;
using Meteor.Map.packets.send.actor.battle;

namespace Meteor.Map.actors.chara.ai.state
{
    class WeaponSkillState : State
    {

        private BattleCommand skill;
        private HitDirection hitDirection;
        public WeaponSkillState(Character owner, Character target, ushort skillId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            this.skill = Server.GetWorldManager().GetBattleCommand(skillId);

            var returnCode = skill.CallLuaFunction(owner, "onSkillPrepare", owner, target, skill);

            this.target = (skill.mainTarget & ValidTarget.SelfOnly) != 0 ? owner : target;

            errorResult = new CommandResult(owner.actorId, 32553, 0);
            if (returnCode == 0 && owner.CanUse(this.target, skill, errorResult))
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
            var returnCode = skill.CallLuaFunction(owner, "onSkillStart", owner, target, skill);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new CommandResult(owner.actorId, (ushort)(returnCode == -1 ? 32558 : returnCode), 0);
            }
            else
            {
                hitDirection = owner.GetHitDirection(target);

                //Do positionals and combo effects first because these can influence accuracy and amount of targets/numhits, which influence the rest of the steps
                //If there is no positon required or if the position bonus should be activated
                if ((skill.positionBonus & utils.BattleUtils.ConvertHitDirToPosition(hitDirection)) == skill.positionBonus)
                {
                    //If there is a position bonus
                    if (skill.positionBonus != BattleCommandPositionBonus.None)
                        skill.CallLuaFunction(owner, "weaponskill", "onPositional", owner, target, skill);

                    //Combo stuff
                    if (owner is Player)
                    {
                        Player p = (Player)owner;
                        //If skill is part of owner's class/job, it can be used in a combo
                        if (skill.job == p.GetClass() || skill.job == p.GetCurrentClassOrJob())
                        {
                            //If owner is a player and the skill being used is part of the current combo
                            if (p.playerWork.comboNextCommandId[0] == skill.id || p.playerWork.comboNextCommandId[1] == skill.id)
                            {
                                skill.CallLuaFunction(owner, "onCombo", owner, target, skill);
                                skill.isCombo = true;
                            }
                            //or if this just the start of a combo
                            else if (skill.comboStep == 1)
                                skill.isCombo = true;
                        }
                    }

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
            skill.targetFind.FindWithinArea(target, skill.validTarget, skill.aoeTarget);
            isCompleted = true;

            owner.DoBattleCommand(skill, "weaponskill");
            owner.statusEffects.RemoveStatusEffectsByFlags((uint) StatusEffectFlags.LoseOnAttacking);

            lua.LuaEngine.GetInstance().OnSignal("weaponskillUsed");
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
            return owner.CanUse(target, skill);
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

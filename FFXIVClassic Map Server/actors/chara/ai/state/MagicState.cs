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
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class MagicState : State
    {

        private BattleCommand spell;
        private Vector3 startPos;

        public MagicState(Character owner, Character target, ushort spellId) :
            base(owner, target)
        {
            this.startPos = owner.GetPosAsVector3();
            this.startTime = DateTime.Now;
            this.spell = Server.GetWorldManager().GetBattleCommand(spellId);
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicPrepare", owner, target, spell);

            //Modify spell based on status effects. Need to do it here because they can modify cast times
            List<StatusEffect> effects = owner.statusEffects.GetStatusEffectsByFlag((uint)(StatusEffectFlags.ActivateOnCastStart));

            //modify skill based on status effects
            //Do this here to allow buffs like Resonance to increase range before checking CanCast()
            foreach (var effect in effects)
                lua.LuaEngine.CallLuaStatusEffectFunction(owner, effect, "onMagicCast", owner, effect, spell);

            this.target = target != null ? target : owner;

            if (returnCode == 0 && owner.CanCast(this.target, spell))
            {
                OnStart();
            }
            else
            {
                errorResult = new CommandResult(owner.actorId, 32553, 0);
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicStart", owner, target, spell);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new CommandResult(target.actorId, (ushort)(returnCode == -1 ? 32553 : returnCode), 0, 0, 0, 1);
            }
            else
            {
                // todo: check within attack range
                float[] baseCastDuration = { 1.0f, 0.25f };

                //There are no positional spells, so just check onCombo, need to check first because certain spells change aoe type/accuracy
                //If owner is a player and the spell being used is part of the current combo
                if (owner is Player && ((Player)owner).GetClass() == spell.job)
                {
                    Player p = (Player)owner;
                    if (spell.comboStep == 1 || ((p.playerWork.comboNextCommandId[0] == spell.id || p.playerWork.comboNextCommandId[1] == spell.id)))
                    {
                        lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onCombo", owner, target, spell);
                        spell.isCombo = true;
                    }
                }

                //Check combo stuff here because combos can impact spell cast times

                float spellSpeed = spell.castTimeMs;

                if (!spell.IsInstantCast())
                {
                // command casting duration
                    if (owner is Player)
                    {
                        // todo: modify spellSpeed based on modifiers and stuff
                        ((Player)owner).SendStartCastbar(spell.id, Utils.UnixTimeStampUTC(DateTime.Now.AddMilliseconds(spellSpeed)));
                    }
                    owner.GetSubState().chantId = 0xf0;
                    owner.SubstateModified();
                    owner.DoBattleAction(spell.id, (uint) 0x6F000000 | spell.castType, new CommandResult(target.actorId, 30128, 1, 0, 1)); //You begin casting (6F000002: BLM, 6F000003: WHM, 0x6F000008: BRD)
                }
            }
        }

        public override bool Update(DateTime tick)
        {
            if (spell != null)
            {
                TryInterrupt();

                if (interrupt)
                {
                    OnInterrupt();
                    return true;
                }

                // todo: check weapon delay/haste etc and use that
                var actualCastTime = spell.castTimeMs;

                if ((tick - startTime).TotalMilliseconds >= spell.castTimeMs)
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
                owner.GetSubState().chantId = 0x0;
                owner.SubstateModified();
                owner.DoBattleAction(spell.id, errorResult.animation, errorResult);
                errorResult = null;
            }
        }

        public override void OnComplete()
        {
            //How do combos/hitdirs work for aoe abilities or does that not matter for aoe?
            HitDirection hitDir = owner.GetHitDirection(target);
            bool hitTarget = false;

            spell.targetFind.FindWithinArea(target, spell.validTarget, spell.aoeTarget);
            isCompleted = true;
            var targets = spell.targetFind.GetTargets();

            owner.DoBattleCommand(spell, "magic");
        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventSpell))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventSpell);
                uint effectId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    effectId = list[0].GetStatusEffectId();
                }
                interrupt = true;
                return;
            }

            if (HasMoved())
            {
                errorResult = new CommandResult(owner.actorId, 30211, 0);
                errorResult.animation = 0x7F000002;
                interrupt = true;
                return;
            }

            interrupt = !CanCast();
        }

        private bool CanCast()
        {
            return owner.CanCast(target, spell) && spell.IsValidMainTarget(owner, target) && !HasMoved();
        }

        private bool HasMoved()
        {
            return (owner.GetPosAsVector3() != startPos);
        }

        public override void Cleanup()
        {
            owner.GetSubState().chantId = 0x0;
            owner.SubstateModified();

            if (owner is Player)
            {
                ((Player)owner).SendEndCastbar();
            }
            owner.aiContainer.UpdateLastActionTime(spell.animationDurationSeconds);
        }

        public BattleCommand GetSpell()
        {
            return spell;
        }
    }
}

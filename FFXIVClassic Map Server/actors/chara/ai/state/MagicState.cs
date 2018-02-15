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

            this.target = spell.GetMainTarget(owner, target);

            if (returnCode == 0 && owner.CanCast(this.target, spell))
            {
                OnStart();
            }
            else
            {
                errorResult = new BattleAction(owner.actorId, 32553, 0);
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicStart", owner, target, spell);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new BattleAction(target.actorId, (ushort)(returnCode == -1 ? 32553 : returnCode), 0, 0, 0, 1);
            }
            else
            {
                // todo: check within attack range
                float[] baseCastDuration = { 1.0f, 0.25f };

                //Check combo stuff here because combos can impact spell cast times

                float spellSpeed = spell.castTimeMs;

                //There are no positional spells, so just check onCombo, need to check first because certain spells change aoe type/accuracy
                //If owner is a player and the spell being used is part of the current combo
                if (spell.comboStep == 1 || ((owner is Player p) && (p.playerWork.comboNextCommandId[0] == spell.id || p.playerWork.comboNextCommandId[1] == spell.id)))
                {
                    lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onCombo", owner, target, spell);
                    spell.isCombo = true;
                }
                if (!spell.IsInstantCast())
                {
                // command casting duration
                    if (owner is Player)
                    {
                        // todo: modify spellSpeed based on modifiers and stuff
                        ((Player)owner).SendStartCastbar(spell.id, Utils.UnixTimeStampUTC(DateTime.Now.AddMilliseconds(spellSpeed)));
                    }
                    owner.SendChant(0xf, 0x0);
                    owner.DoBattleAction(spell.id, (uint) 0x6F000000 | spell.castType, new BattleAction(target.actorId, 30128, 1, 0, 1)); //You begin casting (6F000002: BLM, 6F000003: WHM, 0x6F000008: BRD)
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
                owner.SendChant(0, 0);
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


            List<BattleAction> actions = new List<BattleAction>();
            if (targets.Count > 0)
            {
                List<StatusEffect> effects = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.ActivateOnSpell);

                //modify skill based on status effects
                foreach (var effect in effects)
                    lua.LuaEngine.CallLuaStatusEffectFunction(owner, effect, "onWeaponSkill", owner, effect, spell);

                //Now that combos and positionals bonuses are done, we can calculate hits/crits/etc and damage
                foreach (var chara in targets)
                {
                    for (int hitNum = 0; hitNum < spell.numHits; hitNum++)
                    {
                        var action = new BattleAction(chara.actorId, spell.worldMasterTextId, 0, 0, (byte) hitDir, (byte) hitNum);
                        lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicFinish", owner, chara, spell, action);

                        //if hit type isn't evade or miss
                        if (action.hitType > HitType.Evade)
                            hitTarget = true;
                        
                        actions.AddRange(action.GetAllActions());
                    }
                }
            }
            else
            {
                //No targets hit, cast failed
                actions.Add(new BattleAction(target.actorId, 30202, (uint) (0)));
            }


            // todo: this is fuckin stupid, probably only need *one* error packet, not an error for each action
            BattleAction[] errors = (BattleAction[])actions.ToArray().Clone();
            owner.OnCast(this, actions.ToArray(), spell, ref errors);
            owner.DoBattleAction(spell.id, spell.battleAnimation, actions);
            owner.statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnCasting);
            //Now that we know if we hit the target we can check if the combo continues
            if (owner is Player player)
                if (spell.isCombo && hitTarget)
                    player.SetCombos(spell.comboNextCommandId);
                else
                    player.SetCombos();
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
                errorResult = new BattleAction(owner.actorId, 30211, 0);
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
            owner.SendChant(0, 0);

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

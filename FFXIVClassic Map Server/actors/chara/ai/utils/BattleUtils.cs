using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai.utils
{
    static class BattleUtils
    {
        public static bool TryAttack(Character attacker, Character defender, BattleAction action, ref SubPacket errorPacket)
        {
            // todo: get hit rate, hit count, set hit effect
            action.effectId |= (uint)(HitEffect.RecoilLv2 | HitEffect.Hit | HitEffect.HitVisual1);
            return true;
        }

        public static ushort CalculateAttackDamage(Character attacker, Character defender, BattleAction action)
        {
            ushort damage = (ushort)(Program.Random.Next(10) * 10);

            // todo: handle all other crap before protect/stoneskin

            // todo: handle crit etc
            if (defender.statusEffects.HasStatusEffect(StatusEffectId.Protect) || defender.statusEffects.HasStatusEffect(StatusEffectId.Protect2))
            {
                if (action != null)
                    action.effectId |= (uint)HitEffect.Protect;
            }

            if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
            {
                if (action != null)
                    action.effectId |= (uint)HitEffect.Stoneskin;
            }
            return damage;
        }

        public static ushort GetCriticalHitDamage(Character attacker, Character defender, BattleAction action)
        {
            ushort damage = action.amount;

            // todo:
            // 
            // action.effectId |= (uint)HitEffect.Critical;
            //
            return damage;
        }

        public static ushort CalculateSpellDamage(Character attacker, Character defender, BattleAction action)
        {
            ushort damage = 0;

            // todo: handle all other crap before shell/stoneskin

            if (defender.statusEffects.HasStatusEffect(StatusEffectId.Shell))
            {
                // todo: shell probably only shows on magic..
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Shell))
                {
                    if (action != null)
                        action.effectId |= (uint)HitEffect.Shell;
                }
            }

            if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
            {
                if (action != null)
                    action.effectId |= (uint)HitEffect.Stoneskin;
            }
            return damage;
        }

        public static void DamageTarget(Character attacker, Character defender, BattleAction action)
        {
            // todo: other stuff too
            if (defender is BattleNpc)
            {
                if (!((BattleNpc)defender).hateContainer.HasHateForTarget(attacker))
                {
                    ((BattleNpc)defender).hateContainer.AddBaseHate(attacker);
                }
                ((BattleNpc)defender).hateContainer.UpdateHate(attacker, action.amount);
            }
            defender.DelHP((short)action.amount);
        }

        public static int CalculateSpellDamage(Character attacker, Character defender, BattleCommand spell)
        {
            // todo: spell formulas and stuff (stoneskin, mods, stats, etc)
            return 69;
        }

        public static uint CalculateSpellCost(Character caster, Character target, BattleCommand spell)
        {
            var scaledCost = spell.CalculateCost((uint)caster.charaWork.parameterSave.state_mainSkillLevel);

            // todo: calculate cost for mob/player
            if (caster.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER)
            {
                
            }
            else
            {

            }
            return scaledCost;
        }
    }
}

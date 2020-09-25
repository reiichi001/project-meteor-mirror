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
using System.Collections.Generic;
using System.Linq;

using Meteor.Map.Actors;
using Meteor.Map.packets.send.actor.battle;
using Meteor.Map.actors.chara.npc;
using Meteor.Common;

namespace Meteor.Map.actors.chara.ai.utils
{
    static class BattleUtils
    {

        public static Dictionary<HitType, ushort> PhysicalHitTypeTextIds = new Dictionary<HitType, ushort>()
        {
            { HitType.Miss,     30311 },
            { HitType.Evade,    30310 },
            { HitType.Parry,    30308 },
            { HitType.Block,    30306 },
            { HitType.Hit,      30301 },
            { HitType.Crit,     30302 }
        };

        public static Dictionary<HitType, ushort> MagicalHitTypeTextIds = new Dictionary<HitType, ushort>()
        {
            { HitType.SingleResist,30318 },
            { HitType.DoubleResist,30317 },
            { HitType.TripleResist, 30316 },//Triple Resists seem to use the same text ID as full resists
            { HitType.FullResist,30316 },
            { HitType.Hit,      30319 },
            { HitType.Crit,     30392 }     //Unsure why crit is separated from the rest of the ids
        };

        public static Dictionary<HitType, ushort> MultiHitTypeTextIds = new Dictionary<HitType, ushort>()
        {
            { HitType.Miss,     30449 },    //The attack misses.
            { HitType.Evade,    0 },        //Evades were removed before multi hit skills got their own messages, so this doesnt exist
            { HitType.Parry,    30448 },    //[Target] parries, taking x points of damage.
            { HitType.Block,    30447 },    //[Target] blocks, taking x points of damage.
            { HitType.Hit,      30443 },    //[Target] tales x points of damage
            { HitType.Crit,     30444 }     //Critical! [Target] takes x points of damage.
        };

        public static Dictionary<HitType, HitEffect> HitTypeEffectsPhysical = new Dictionary<HitType, HitEffect>()
        {
            { HitType.Miss,     0 },
            { HitType.Evade,    HitEffect.Evade },
            { HitType.Parry,    HitEffect.Parry },
            { HitType.Block,    HitEffect.Block },
            { HitType.Hit,      HitEffect.Hit },
            { HitType.Crit,     HitEffect.Crit | HitEffect.CriticalHit }
        };

        //Magic attacks can't miss, be blocked, or parried. Resists are technically evades
        public static Dictionary<HitType, HitEffect> HitTypeEffectsMagical = new Dictionary<HitType, HitEffect>()
        {
            { HitType.SingleResist,     HitEffect.WeakResist },
            { HitType.DoubleResist,     HitEffect.WeakResist },
            { HitType.TripleResist,     HitEffect.WeakResist },
            { HitType.FullResist,       HitEffect.FullResist },
            { HitType.Hit,              HitEffect.NoResist },
            { HitType.Crit,             HitEffect.Crit }
        };

        public static Dictionary<KnockbackType, HitEffect> KnockbackEffects = new Dictionary<KnockbackType, HitEffect>()
        {
            { KnockbackType.None, 0 },
            { KnockbackType.Level1, HitEffect.KnockbackLv1 },
            { KnockbackType.Level2, HitEffect.KnockbackLv2 },
            { KnockbackType.Level3, HitEffect.KnockbackLv3 },
            { KnockbackType.Level4, HitEffect.KnockbackLv4 },
            { KnockbackType.Level5, HitEffect.KnockbackLv5 },
            { KnockbackType.Clockwise1, HitEffect.KnockbackClockwiseLv1 },
            { KnockbackType.Clockwise2, HitEffect.KnockbackClockwiseLv2 },
            { KnockbackType.CounterClockwise1, HitEffect.KnockbackCounterClockwiseLv1 },
            { KnockbackType.CounterClockwise2, HitEffect.KnockbackCounterClockwiseLv2 },
            { KnockbackType.DrawIn, HitEffect.DrawIn }
        };

        public static Dictionary<byte, ushort> ClassExperienceTextIds = new Dictionary<byte, ushort>()
        {
            { 2, 33934 },   //Pugilist
            { 3, 33935 },   //Gladiator
            { 4, 33936 },   //Marauder
            { 7, 33937 },   //Archer
            { 8, 33938 },   //Lancer
            { 10, 33939 },  //Sentinel, this doesn't exist anymore but it's still in the files so may as well put it here just in case
            { 22, 33940 },  //Thaumaturge
            { 23, 33941 },  //Conjurer
            { 29, 33945 },  //Carpenter, for some reason there's a a few different messages between 33941 and 33945
            { 30, 33946 },  //Blacksmith
            { 31, 33947 },  //Armorer
            { 32, 33948 },  //Goldsmith
            { 33, 33949 },  //Leatherworker
            { 34, 33950 },  //Weaver
            { 35, 33951 },  //Alchemist
            { 36, 33952 },  //Culinarian
            { 39, 33953 },  //Miner
            { 40, 33954 },  //Botanist
            { 41, 33955 }   //Fisher
        };

        //Most of these numbers I'm fairly certain are correct. The repeated numbers at levels 23 and 48 I'm less sure about but they do match some weird spots in the EXP graph

        public static ushort[] BASEEXP =   {150, 150, 150, 150, 150, 150, 150, 150, 150, 150,  //Level <= 10
                                        150, 150, 150, 150, 150, 150, 150, 150, 160, 170,   //Level <= 20
                                        180, 190, 190, 200, 210, 220, 230, 240, 250, 260,   //Level <= 30
                                        270, 280, 290, 300, 310, 320, 330, 340, 350, 360,   //Level <= 40
                                        370, 380, 380, 390, 400, 410, 420, 430, 430, 440};  //Level <= 50

        public static bool TryAttack(Character attacker, Character defender, CommandResult action, ref CommandResult error)
        {
            // todo: get hit rate, hit count, set hit effect
            //action.effectId |= (uint)(HitEffect.RecoilLv2 | HitEffect.Hit | HitEffect.HitVisual1);
            return true;
        }

        private static double CalculateDlvlModifier(short dlvl)
        {
            //this is just a really, really simplified version of the graph from http://kanican.livejournal.com/55915.html
            //actual formula is definitely more complicated
            //I'm going to assum these formulas are linear, and they're clamped so the modifier never goes below 0.
            double modifier = 0;


            if (dlvl >= 0)
                modifier = (.35 * dlvl) + .225;
            else
                modifier = (.01 * dlvl) + .25;

            return modifier.Clamp(0, 1);
        }

        //Damage calculations
        //Calculate damage of action
        //We could probably just do this when determining the action's hit type
        public static void CalculatePhysicalDamageTaken(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());

            // todo: physical resistances

            //dlvl, Defense, and Vitality all effect how much damage is taken after hittype takes effect
            //player attacks cannot do more than 9999 damage.
            //VIT is turned into Defense at a 3:2 ratio in calculatestats, so don't need to do that here
            double damageTakenPercent = 1 - (defender.GetMod(Modifier.DamageTakenDown) / 100.0);
            action.amount = (ushort)(action.amount - CalculateDlvlModifier(dlvl) * (defender.GetMod((uint)Modifier.Defense))).Clamp(0, 9999);
            action.amount = (ushort)(action.amount * damageTakenPercent).Clamp(0, 9999);
        }


        public static void CalculateSpellDamageTaken(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());

            // todo: elemental resistances
            //Patch 1.19:
            //Magic Defense has been abolished and no longer appears in equipment attributes.
            //The effect of elemental attributes has been changed to that of reducing damage from element-based attacks.

            //http://kanican.livejournal.com/55370.html:
            //elemental resistance stats are not actually related to resists (except for status effects), instead they impact damage taken


            //dlvl, Defense, and Vitality all effect how much damage is taken after hittype takes effect
            //player attacks cannot do more than 9999 damage.
            double damageTakenPercent = 1 - (defender.GetMod(Modifier.DamageTakenDown) / 100.0);
            action.amount = (ushort)(action.amount - CalculateDlvlModifier(dlvl) * (defender.GetMod((uint)Modifier.Defense) + 0.67 * defender.GetMod((uint)Modifier.Vitality))).Clamp(0, 9999);
            action.amount = (ushort)(action.amount * damageTakenPercent).Clamp(0, 9999);
        }

        
        public static void CalculateBlockDamage(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            double percentBlocked;

            //Aegis boon forces a full block
            if (defender.statusEffects.HasStatusEffect(StatusEffectId.AegisBoon))
                percentBlocked = 1.0;
            else
            {
                //Is this a case where VIT gives Block?
                percentBlocked = defender.GetMod((uint)Modifier.Block) * 0.002;//Every point of Block adds .2% to how much is blocked
                percentBlocked += defender.GetMod((uint)Modifier.Vitality) * 0.001;//Every point of vitality adds .1% to how much is blocked
            }

            action.amountMitigated = (ushort)(action.amount * percentBlocked);
            action.amount = (ushort)(action.amount * (1.0 - percentBlocked));
        }

        //don't know exact crit bonus formula
        public static void CalculateCritDamage(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());
            double bonus = (.04 * (dlvl * dlvl)) - 2 * dlvl;
            bonus += 1.20;
            double potencyModifier = (-.075 * dlvl) + 1.73;

            // + potency bonus
            //bonus += attacker.GetMod((uint) Modifier.CriticalPotency) * potencyModifier;
            // - Crit resilience
            //bonus -= attacker.GetMod((uint)Modifier.CriticalResilience) * potencyModifier;

            //need to add something for bonus potency as a part of skill (ie thundara, which breaks the cap)
            action.amount = (ushort)(action.amount * bonus.Clamp(1.15, 1.75));//min bonus of 115, max bonus of 175
        }

        public static void CalculateParryDamage(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            double percentParry = 0.75;

            action.amountMitigated = (ushort)(action.amount * (1 - percentParry));
            action.amount = (ushort)(action.amount * percentParry);
        }

        //There are 3 or 4 tiers of resist that are flat 25% decreases in damage. 
        //It's possible we could just calculate the damage at the same time as we determine the hit type (the same goes for the rest of the hit types)
        //Or we could have HitTypes for DoubleResist, TripleResist, and FullResist that get used here.
        public static void CalculateResistDamage(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            //Every tier of resist is a 25% reduction in damage. ie SingleResist is 25% damage taken down, Double is 50% damage taken down, etc
            double percentResist = 0.25 * (action.hitType - HitType.SingleResist + 1);

            action.amountMitigated = (ushort)(action.amount * (1 - percentResist));
            action.amount = (ushort)(action.amount * percentResist);
        }

        //It's weird that stoneskin is handled in C# and all other buffs are in scripts right now
        //But it's because stoneskin acts like both a preaction and postaction buff in that it falls off after damage is dealt but impacts how much damage is dealt
        public static void HandleStoneskin(Character defender, CommandResult action)
        {
            var mitigation = Math.Min(action.amount, defender.GetMod(Modifier.Stoneskin));

            action.amount = (ushort) (action.amount - mitigation).Clamp(0, 9999);
            defender.SubtractMod((uint)Modifier.Stoneskin, mitigation);
        }

        public static void DamageTarget(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer= null)
        {
            if (defender != null)
            {

                //Bugfix, mobs that instantly died were insta disappearing due to lastAttacker == null.
                if (defender is BattleNpc)
                {
                    var bnpc = defender as BattleNpc;
                    if (bnpc.lastAttacker == null)
                        bnpc.lastAttacker = attacker;
                }

                defender.DelHP((short)action.amount, actionContainer);
                attacker.OnDamageDealt(defender, skill, action, actionContainer);
                defender.OnDamageTaken(attacker, skill, action, actionContainer);

                // todo: other stuff too
                if (defender is BattleNpc)
                {
                    var bnpc = defender as BattleNpc;
                    if (!bnpc.hateContainer.HasHateForTarget(attacker))
                    {
                        bnpc.hateContainer.AddBaseHate(attacker);
                    }
                    bnpc.hateContainer.UpdateHate(attacker, action.enmity);
                    bnpc.lastAttacker = attacker;
                }
            }
        }

        public static void HealTarget(Character caster, Character target, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            if (target != null)
            {
                target.AddHP(action.amount, actionContainer);

                target.statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnHealed, "onHealed", caster, target, skill, action, actionContainer);
            }
        }


        #region Rate Functions

        //How is accuracy actually calculated?
        public static double GetHitRate(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            double hitRate = 80.0;

            //Add raw hit rate buffs, subtract raw evade buffs, take into account skill's accuracy modifier.
            double hitBuff = attacker.GetMod(Modifier.RawHitRate);
            double evadeBuff = defender.GetMod(Modifier.RawEvadeRate);
            float modifier = skill != null ? skill.accuracyModifier : 0;
            hitRate += (hitBuff + modifier).Clamp(0, 100.0);
            hitRate -= evadeBuff;
            return hitRate.Clamp(0, 100.0);
        }

        //Whats the parry formula?
        public static double GetParryRate(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            //Can't parry with shield, can't parry rear attacks
            if (defender.GetMod((uint)Modifier.CanBlock) != 0 || action.param == (byte) HitDirection.Rear)
                return 0;

            double parryRate = 10.0;

            parryRate += defender.GetMod(Modifier.Parry) * 0.1;//.1% rate for every point of Parry

            return parryRate + (defender.GetMod(Modifier.RawParryRate));
        }

        public static double GetCritRate(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            if (action.actionType == ActionType.Status)
                return 0.0;
            
            //using 10.0 for now since gear isn't working
            double critRate = 10.0;// 0.16 * attacker.GetMod((uint)Modifier.CritRating);//Crit rating adds .16% per point

            //Add additional crit rate from skill
            //Should this be a raw percent or a flat crit raitng? the wording on skills/buffs isn't clear.
            critRate += 0.16 * (skill != null ? skill.bonusCritRate : 0);

            return critRate + attacker.GetMod(Modifier.RawCritRate);
        }

        //http://kanican.livejournal.com/55370.html
        // todo: figure that out
        public static double GetResistRate(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            // todo: add elemental stuff
            //Can only resist spells?
            if (action.commandType != CommandType.Spell && action.actionProperty <= ActionProperty.Projectile)
                return 0.0;

            return 15.0 + defender.GetMod(Modifier.RawResistRate);
        }

        //Block Rate follows 4 simple rules:
        //(1) Every point in DEX gives +0.1% rate
        //(2) Every point in "Block Rate" gives +0.2% rate
        //(3) True block proc rate is capped at 75%. No clue on a possible floor.
        //(4) The baseline rate is based on dLVL only(mob stats play no role). The baseline rate is summarized in this raw data sheet: https://imgbox.com/aasLyaJz
        public static double GetBlockRate(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            //Shields are required to block and can't block from rear.
            if (defender.GetMod((uint)Modifier.CanBlock) == 0 || action.param == (byte)HitDirection.Rear)
                return 0;

            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());
            double blockRate = (2.5 * dlvl) + 5; // Base block rate

            //Is this one of those thing where DEX gives block rate and this would be taking DEX into account twice?
            blockRate += defender.GetMod((uint)Modifier.Dexterity) * 0.1;// .1% for every dex
            blockRate += defender.GetMod((uint)Modifier.BlockRate) * 0.2;// .2% for every block rate

            return Math.Min(blockRate, 25.0) + defender.GetMod((uint)Modifier.RawBlockRate);
        }

        #endregion

        public static bool TryCrit(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            if ((Program.Random.NextDouble() * 100) <= action.critRate)
            {
                action.hitType = HitType.Crit;
                CalculateCritDamage(attacker, defender, skill, action);

                if(skill != null)
                    skill.actionCrit = true;

                return true;
            }

            return false;
        }

        //This probably isn't totally correct but it's close enough for now. 
        //Full Resists seem to be calculated in a different way because the resist rates don't seem to line up with kanikan's testing (their tests didn't show any full resists)
        //Non-spells with elemental damage can be resisted, it just doesnt say in the chat that they were. As far as I can tell, all mob-specific attacks are considered not to be spells
        public static bool TryResist(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            //The rate degrades for each check. Meaning with 100% resist, the attack will always be resisted, but it won't necessarily be a triple or full resist
            //Rates beyond 100 still increase the chance for higher resist tiers though
            double rate = action.resistRate;

            int i = -1;

            while ((Program.Random.NextDouble() * 100) <= rate && i < 4)
            {
                rate /= 2;
                i++;
            }

            if (i != -1)
            {
                action.hitType = (HitType) ((int) HitType.SingleResist + i);
                CalculateResistDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        public static bool TryBlock(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            if ((Program.Random.NextDouble() * 100) <= action.blockRate)
            {
                action.hitType = HitType.Block;
                CalculateBlockDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        public static bool TryParry(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            if ((Program.Random.NextDouble() * 100) <= action.parryRate)
            {
                action.hitType = HitType.Parry;
                CalculateParryDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        //TryMiss instead of tryHit because hits are the default and don't change damage
        public static bool TryMiss(Character attacker, Character defender, BattleCommand skill, CommandResult action)
        {
            if ((Program.Random.NextDouble() * 100) >= GetHitRate(attacker, defender, skill, action))
            {
                action.hitType = (ushort)HitType.Miss;
                //On misses, the entire amount is considered mitigated
                action.amountMitigated = action.amount;
                action.amount = 0;
                return true;
            }
            return false;
        }

        /*
         * Hit Effecthelpers. Different types of hit effects hits use some flags for different things, so they're split into physical, magical, heal, and status
         */
        public static void DoAction(Character caster, Character target, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            switch (action.actionType)
            {
                case (ActionType.Physical):
                    FinishActionPhysical(caster, target, skill, action, actionContainer);
                    break;
                case (ActionType.Magic):
                    FinishActionSpell(caster, target, skill, action, actionContainer);
                    break;
                case (ActionType.Heal):
                    FinishActionHeal(caster, target, skill, action, actionContainer);
                    break;
                case (ActionType.Status):
                    FinishActionStatus(caster, target, skill, action, actionContainer);
                    break;
                default:
                    action.effectId = (uint) HitEffect.AnimationEffectType;
                    actionContainer.AddAction(action);
                    break;
            }
        }

        //Determine the hit type, set the hit effect, modify damage based on stoneskin and hit type, hit target
        public static void FinishActionPhysical(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            //Figure out the hit type and change damage depending on hit type
            if (!TryMiss(attacker, defender, skill, action))
            {
                //Handle Stoneskin here because it seems like stoneskin mitigates damage done before taking into consideration crit/block/parry damage reductions.
                //This is based on the fact that a 0 damage attack due to stoneskin will heal for 0 with Aegis Boon, meaning Aegis Boon didn't mitigate any damage
                HandleStoneskin(defender, action);

                //Crits can't be blocked (is this true for Aegis Boon and Divine Veil?) or parried so they are checked first.
                if (!TryCrit(attacker, defender, skill, action))
                    //Block and parry order don't really matter because if you can block you can't parry and vice versa
                    if (!TryBlock(attacker, defender, skill, action))
                        if(!TryParry(attacker, defender, skill, action))
                            //Finally if it's none of these, the attack was a hit
                            action.hitType = HitType.Hit;
            }

            //Actions have different text ids depending on whether they're a part of a multi-hit ws or not.
            Dictionary<HitType, ushort> textIds = PhysicalHitTypeTextIds;

            //If this is the first hit of a multi hit command, add the "You use [command] on [target]" action
            //Needs to be done here because certain buff messages appear before it.
            if (skill != null && skill.numHits > 1)
            {
                if (action.hitNum == 1)
                    actionContainer?.AddAction(new CommandResult(attacker.actorId, 30441, 0));

                textIds = MultiHitTypeTextIds;
            }

            //Set the correct textId
            action.worldMasterTextId = textIds[action.hitType];

            //Set the hit effect
            SetHitEffectPhysical(attacker, defender, skill, action, actionContainer);

            //Modify damage based on defender's stats
            CalculatePhysicalDamageTaken(attacker, defender, skill, action);

            actionContainer.AddAction(action);
            action.enmity = (ushort) (action.enmity * (skill != null ? skill.enmityModifier : 1));

            //Damage the target
            DamageTarget(attacker, defender, skill, action, actionContainer);
        }

        public static void FinishActionSpell(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            //I'm assuming that like physical attacks stoneskin is taken into account before mitigation
            HandleStoneskin(defender, action);

            //Determine the hit type of the action
            //Spells don't seem to be able to miss, instead magic acc/eva is used for resists (which are generally called evades in game)
            //Unlike blocks and parries, crits do not go through resists.
            if (!TryResist(attacker, defender, skill, action))
            {
                if (!TryCrit(attacker, defender, skill, action))
                    action.hitType = HitType.Hit;
            }

            //There are no multi-hit spells, so we don't need to take that into account
            action.worldMasterTextId = MagicalHitTypeTextIds[action.hitType];

            //Set the hit effect
            SetHitEffectSpell(attacker, defender, skill, action);

            HandleStoneskin(defender, action);

            CalculateSpellDamageTaken(attacker, defender, skill, action);

            actionContainer.AddAction(action);

            DamageTarget(attacker, defender, skill, action, actionContainer);
        }

        public static void FinishActionHeal(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            //Set the hit effect
            SetHitEffectHeal(attacker, defender, skill, action);

            actionContainer.AddAction(action);

            HealTarget(attacker, defender, skill, action, actionContainer);
        }

        public static void FinishActionStatus(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            //Set the hit effect
            SetHitEffectStatus(attacker, defender, skill, action);

            TryStatus(attacker, defender, skill, action, actionContainer, false);

            actionContainer.AddAction(action);
        }

        public static void SetHitEffectPhysical(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer)
        {
            var hitEffect = HitEffect.HitEffectType;
            HitType hitType = action.hitType;

            //Don't know what recoil is actually based on, just guessing
            //Crit is 2 and 3 together
            if (hitType == HitType.Crit)
                hitEffect |= HitEffect.CriticalHit;
            else
            {
                //It's not clear what recoil level is based on for physical attacks
                double percentDealt = (100.0 * (action.amount / defender.GetMaxHP()));
                if (percentDealt > 5.0)
                    hitEffect |= HitEffect.RecoilLv2;
                else if (percentDealt > 10)
                    hitEffect |= HitEffect.RecoilLv3;
            }

            hitEffect |= HitTypeEffectsPhysical[hitType];

            //For combos that land, add the combo effect
            if (skill != null && skill.isCombo && action.ActionLanded() && !skill.comboEffectAdded)
            {
                hitEffect |= (HitEffect)(skill.comboStep << 15);
                skill.comboEffectAdded = true;
            }

            //if attack hit the target, take into account protective status effects
            if (hitType >= HitType.Parry)
            {
                //Protect / Shell only show on physical/ magical attacks respectively.
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Protect) || defender.statusEffects.HasStatusEffect(StatusEffectId.Protect2))
                    if (action != null)
                        hitEffect |= HitEffect.Protect;

                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
                    if (action != null)
                        hitEffect |= HitEffect.Stoneskin;
            }

            action.effectId = (uint)hitEffect;
        }

        public static void SetHitEffectSpell(Character attacker, Character defender, BattleCommand skill, CommandResult action, CommandResultContainer actionContainer = null)
        {
            var hitEffect = HitEffect.MagicEffectType;
            HitType hitType = action.hitType;


            hitEffect |= HitTypeEffectsMagical[hitType];

            if (skill != null && skill.isCombo && !skill.comboEffectAdded)
            {
                hitEffect |= (HitEffect)(skill.comboStep << 15);
                skill.comboEffectAdded = true;
            }

            //if attack hit the target, take into account protective status effects
            if (action.ActionLanded())
            {
                //Protect / Shell only show on physical/ magical attacks respectively.
                //The magic hit effect category only has a flag for shell (and another shield effect that seems unused)
                //Even though traited protect gives magic defense, the shell effect doesn't play on attacks
                //This also means stoneskin doesnt show, but it does reduce damage
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Shell))
                    if (action != null)
                        hitEffect |= HitEffect.MagicShell;
            }
            action.effectId = (uint)hitEffect;
        }


        public static void SetHitEffectHeal(Character caster, Character receiver, BattleCommand skill, CommandResult action)
        {
            var hitEffect = HitEffect.MagicEffectType | HitEffect.Heal;
            //Heals use recoil levels in some way as well. Possibly for very low health clutch heals or based on percentage of current health healed (not max health).
            // todo: figure recoil levels out for heals
            hitEffect |= HitEffect.RecoilLv3;
            //do heals crit?

            action.effectId = (uint)hitEffect;
        }

        public static void SetHitEffectStatus(Character caster, Character receiver, BattleCommand skill, CommandResult action)
        {
            var hitEffect = (uint)HitEffect.StatusEffectType | skill.statusId;
            action.effectId = hitEffect;

            action.hitType = HitType.Hit;
        }

        public static uint CalculateSpellCost(Character caster, Character target, BattleCommand spell)
        {
            var scaledCost = spell.CalculateMpCost(caster);

            // todo: calculate cost for mob/player
            if (caster is BattleNpc)
            {

            }
            else
            {

            }
            return scaledCost;
        }


        //IsAdditional is needed because additional actions may be required for some actions' effects
        //For instance, Goring Blade's bleed effect requires another action so the first action can still show damage numbers
        //Sentinel doesn't require an additional action because it doesn't need to show those numbers
        //this is stupid
        public static void TryStatus(Character caster, Character target, BattleCommand skill, CommandResult action, CommandResultContainer results, bool isAdditional = true)
        {
            double rand = Program.Random.NextDouble();

            //Statuses only land for non-resisted attacks and attacks that hit
            if (skill != null && skill.statusId != 0 && (action.ActionLanded()) && rand < skill.statusChance)
            {
                StatusEffect effect = Server.GetWorldManager().GetStatusEffect(skill.statusId);
                //Because combos might change duration or tier
                if (effect != null)
                {
                    effect.SetDuration(skill.statusDuration);
                    effect.SetTier(skill.statusTier);
                    effect.SetMagnitude(skill.statusMagnitude);
                    effect.SetOwner(target);
                    effect.SetSource(caster);

                    if (target.statusEffects.AddStatusEffect(effect, caster))
                    {
                        //If we need an extra action to show the status text
                        if (isAdditional)
                            results.AddAction(target.actorId, effect.GetStatusGainTextId(), skill.statusId | (uint) HitEffect.StatusEffectType);
                    }
                    else
                        action.worldMasterTextId = 32002;//Is this right?
                }
                else
                {
                    //until all effects are scripted and added to db just doing this
                    if (target.statusEffects.AddStatusEffect(skill.statusId, skill.statusTier, skill.statusMagnitude, skill.statusDuration, 3000))
                    {
                        //If we need an extra action to show the status text
                        if (isAdditional)
                            results.AddAction(target.actorId, 30328, skill.statusId | (uint) HitEffect.StatusEffectType);
                    }
                    else
                        action.worldMasterTextId = 32002;//Is this right?
                }
            }
        }

        //Convert a HitDirection to a BattleCommandPositionBonus. Basically just combining left/right into flank
        public static BattleCommandPositionBonus ConvertHitDirToPosition(HitDirection hitDir)
        {
            BattleCommandPositionBonus position = BattleCommandPositionBonus.None;

            switch (hitDir)
            {
                case (HitDirection.Front):
                    position = BattleCommandPositionBonus.Front;
                    break;
                case (HitDirection.Right):
                case (HitDirection.Left):
                    position = BattleCommandPositionBonus.Flank;
                    break;
                case (HitDirection.Rear):
                    position = BattleCommandPositionBonus.Rear;
                    break;
            }
            return position;
        }


        #region experience helpers
        //See 1.19 patch notes for exp info.
        public static ushort GetBaseEXP(Player player, BattleNpc mob)
        {
            //The way EXP seems to work for most enemies is that it gets the lower character's level, gets the base exp for that level, then uses dlvl to modify that exp
            //Less than -19 dlvl gives 0 exp and no message is sent.
            //This equation doesn't seem to work for certain bosses or NMs.
            //Some enemies might give less EXP? Unsure on this. It seems like there might have been a change in base exp amounts after 1.19

            //Example:
            //Level 50 in a party kills a level 45 enemy
            //Base exp is 400, as that's the base EXP for level 45
            //That's multiplied by the dlvl modifier for -5, which is 0.5625, which gives 225
            //That's then multiplied by the party modifier, which seems to be 0.667 regardless of party size, which gives 150
            //150 is then modified by bonus experience from food, rested exp, links, and chains

            int dlvl = mob.GetLevel() - player.GetLevel();
            if (dlvl <= -20)
                return 0;

            int baseLevel = Math.Min(player.GetLevel(), mob.GetLevel());
            ushort baseEXP = BASEEXP[baseLevel - 1];

            double dlvlModifier = 1.0;

            //There's 2 functions depending on if the dlvl is positive or negative. 
            if (dlvl >= 0)
                //I'm not sure if this caps out at some point. This is correct up to at least +9 dlvl though.
                dlvlModifier += 0.2 * dlvl;
            else
                //0.1x + 0.0025x^2
                dlvlModifier += 0.1 * dlvl + 0.0025 * (dlvl * dlvl);

            //The party modifier isn't clear yet. It seems like it might just be 0.667 for any number of members in a group, but the 1.19 notes say it's variable
            //There also seem to be some cases where it simply doesn't apply but it isn't obvious if that's correct or when it applies if it is correct
            double partyModifier = player.currentParty.GetMemberCount() == 1 ? 1.0 : 0.667;

            baseEXP = (ushort) (baseEXP * dlvlModifier * partyModifier);

            return baseEXP;
        }

        //Gets the EXP bonus when enemies link
        public static byte GetLinkBonus(ushort linkCount)
        {
            byte bonus = 0;

            switch (linkCount)
            {
                case (0):
                    break;
                case (1):
                    bonus = 25;
                    break;
                case (2):
                    bonus = 50;
                    break;
                case (3):
                    bonus = 75;
                    break;
                case (4):
                default:
                    bonus = 100;
                    break;
            }

            return bonus;
        }

        //Gets EXP chain bonus for Attacker fighting Defender
        //Official text on EXP Chains:     An EXP Chain occurs when players consecutively defeat enemies of equal or higher level than themselves within a specific amount of time.
        //Assuming this means that there is no bonus for enemies below player's level and EXP chains are specific to the person, not party
        public static byte GetChainBonus(ushort tier)
        {
            byte bonus = 0;

            switch (tier)
            {
                case (0):
                    break;
                case (1):
                    bonus = 20;
                    break;
                case (2):
                    bonus = 25;
                    break;
                case (3):
                    bonus = 30;
                    break;
                case (4):
                    bonus = 40;
                    break;
                default:
                    bonus = 50;
                    break;
            }
            return bonus;
        }

        public static byte GetChainTimeLimit(ushort tier)
        {
            byte timeLimit = 0;

            switch (tier)
            {
                case (0):
                    timeLimit = 100;
                    break;
                case (1):
                    timeLimit = 80;
                    break;
                case (2):
                    timeLimit = 60;
                    break;
                case (3):
                    timeLimit = 20;
                    break;
                default:
                    timeLimit = 10;
                    break;
            }

            return timeLimit;
        }

        //Calculates bonus EXP for Links and Chains
        public static void AddBattleBonusEXP(Player attacker, BattleNpc defender, CommandResultContainer actionContainer)
        {
            ushort baseExp = GetBaseEXP(attacker, defender);

            //Only bother calculating the rest if there's actually exp to be gained.
            //0 exp sends no message
            if (baseExp > 0)
            {
                int totalBonus = 0;//GetMod(Modifier.bonusEXP)

                var linkCount = defender.GetMobMod(MobModifier.LinkCount);
                totalBonus += GetLinkBonus((byte)Math.Min(linkCount, 255));

                StatusEffect effect = attacker.statusEffects.GetStatusEffectById((uint)StatusEffectId.EXPChain);
                ushort expChainNumber = 0;
                uint timeLimit = 100;
                if (effect != null)
                {
                    expChainNumber = effect.GetTier();
                    timeLimit = (uint)(GetChainTimeLimit(expChainNumber));
                    actionContainer?.AddEXPAction(new CommandResult(attacker.actorId, 33919, 0, expChainNumber, (byte)timeLimit));
                }

                totalBonus += GetChainBonus(expChainNumber);

                StatusEffect newChain = Server.GetWorldManager().GetStatusEffect((uint)StatusEffectId.EXPChain);
                newChain.SetDuration(timeLimit);
                newChain.SetTier((byte)(Math.Min(expChainNumber + 1, 255)));
                attacker.statusEffects.AddStatusEffect(newChain, attacker);

                actionContainer?.AddEXPActions(attacker.AddExp(baseExp, (byte)attacker.GetClass(), (byte)(totalBonus.Min(255))));
            }
        }

        #endregion
    }
}

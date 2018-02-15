using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai.utils
{
    static class BattleUtils
    {

        public static Dictionary<HitType, ushort> HitTypeTextIds = new Dictionary<HitType, ushort>()
        {
            { HitType.Miss,     30311 },
            { HitType.Evade,    30310 },
            { HitType.Parry,    30308 },
            { HitType.Block,    30306 },
            { HitType.Resist,   30306 },//I can't find what the actual textid for resists is
            { HitType.Hit,      30301 },
            { HitType.Crit,     30302 }
        };

        public static Dictionary<HitType, HitEffect> HitTypeEffects = new Dictionary<HitType, HitEffect>()
        {
            { HitType.Miss,     0 },
            { HitType.Evade,    HitEffect.Evade },
            { HitType.Parry,    HitEffect.Parry },
            { HitType.Block,    HitEffect.Block },
            { HitType.Resist,   HitEffect.RecoilLv1 },//Probably don't need this, resists are handled differently to the rest
            { HitType.Hit,      HitEffect.Hit },
            { HitType.Crit,     HitEffect.Crit }
        };


        public static bool TryAttack(Character attacker, Character defender, BattleAction action, ref BattleAction error)
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
        public static void CalculateDamage(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {

            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());

            switch (action.hitType)
            {
                //Misses and evades deal no damage.
                case (HitType.Miss):
                case (HitType.Evade):
                    action.amount = 0;
                    break;
                // todo: figure out parry damage reduction. For now assume 25% reduction
                case (HitType.Parry):
                    CalculateParryDamage(attacker, defender, skill, action);
                    break;
                case (HitType.Block):
                    CalculateBlockDamage(attacker, defender, skill, action);
                    break;
                //There are 3 (or 4?) tiers of resists, each decreasing damage dealt by 25%. For now just assume level 2 resist (50% reduction)
                // todo: figure out resist tiers
                case (HitType.Resist):
                    CalculateResistDamage(attacker, defender, skill, action);
                    break;
                case (HitType.Crit):
                    CalculateCritDamage(attacker, defender, skill, action);
                    break;
            }
            ushort finalAmount = action.amount;

            //dlvl, Defense, and Vitality all effect how much damage is taken after hittype takes effect
            //player attacks cannot do more than 9999 damage.
            action.amount = (ushort) (finalAmount - CalculateDlvlModifier(dlvl) * (defender.GetMod((uint)Modifier.Defense) + 0.67 * defender.GetMod((uint)Modifier.Vitality))).Clamp(0, 9999);


        }

        public static void CalculateBlockDamage(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            double percentBlocked = defender.GetMod((uint)Modifier.Block) * .2;//Every point of Block adds .2% to how much is blocked
            percentBlocked += defender.GetMod((uint)Modifier.Vitality) * .1;//Every point of vitality adds .1% to how much is blocked

            percentBlocked = 1 - percentBlocked;
            action.amount = (ushort)(action.amount * percentBlocked);
        }

        //don't know crit formula
        public static void CalculateCritDamage(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            short dlvl = (short)(defender.GetLevel() - attacker.GetLevel());
            double bonus = (.04 * (dlvl * dlvl)) - 2 * dlvl;
            bonus += 1.20;
            double potencyModifier = (-.075 * dlvl) + 1.73;

            // + potency bonus
            //bonus += attacker.GetMod((uint) Modifier.CriticalPotency) * potencyModifier;
            // - Crit resilience
            //bonus -= attacker.GetMod((uint)Modifier.CriticalResilience) * potencyModifier;

            //need to add something for bonus potency as a part of skill (ie thundara)
            action.amount = (ushort)(action.amount * bonus.Clamp(1.15, 1.75));//min bonus of 115, max bonus of 175
        }

        public static void CalculateParryDamage(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            double percentParry = .75;

            action.amount = (ushort)(action.amount * percentParry);
        }

        //There are 3 or 4 tiers of resist that are flat 25% decreases in damage. 
        //It's possible we could just calculate the damage at the same time as we determine the hit type (the same goes for the rest of the hit types)
        //Or we could have HitTypes for DoubleResist, TripleResist, and FullResist that get used here.
        public static void CalculateResistDamage(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            double percentResist = .5;

            action.amount = (ushort)(action.amount * percentResist);
        }

        //Used for attacks and abilities like Jump that deal damage
        public static ushort CalculateAttackDamage(Character attacker, Character defender, BattleAction action)
        {
            ushort damage = (ushort)100;

            if (attacker is Player p)
            {
                var weapon = p.GetEquipment().GetItemAtSlot(Equipment.SLOT_MAINHAND);
                if (weapon != null)
                {
                    var weaponData = Server.GetItemGamedata(weapon.itemId);

                    //just some numbers from https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page24
                    damage += (ushort) (2.225 * (weaponData as WeaponItem).damagePower + (attacker.GetMod((uint) Modifier.Attack) * .38));

                }
            }

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
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Shell))
                {
                }
            }

            if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
            {
            }
            return damage;
        }

        public static void DamageTarget(Character attacker, Character defender, BattleAction action, DamageTakenType type, bool sendBattleAction = false)
        {
            if (defender != null)
            {
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
                defender.DelHP((short) action.amount);
                defender.OnDamageTaken(attacker, action, type);
            }
        }

        public static void DoAction(Character user, Character receiver, BattleAction action, DamageTakenType type = DamageTakenType.None)
        {
            switch(action.battleActionType)
            {
                //split attack into phys/mag?
                case (BattleActionType.AttackMagic)://not sure if these use different damage taken formulas
                case (BattleActionType.AttackPhysical):
                    DamageTarget(user, receiver, action, type, false);
                    break;
                case (BattleActionType.Heal):
                    receiver.AddHP(action.amount);
                    break;
            }


            if ((type == DamageTakenType.Ability || type == DamageTakenType.Attack) && action.amount != 0)
            {
                receiver.AddTP(150);
                user.AddTP(200);
            }
        }

        /*
         * Rate functions
         */
        //How is accuracy actually calculated?
        public static double GetHitRate(Character attacker, Character defender, BattleCommand skill)
        {
            double hitRate = .80;
            //Certain skills have lower or higher accuracy rates depending on position/combo
            return hitRate * (skill != null ? skill.accuracyModifier : 1);
        }

        //Whats the parry formula?
        public static double GetParryRate(Character attacker, Character defender, BattleCommand skill)
        {
            //Can't parry with shield, must be facing attacker
            if (defender.GetMod((uint)Modifier.HasShield) > 0 || !defender.IsFacing(attacker))
                return 0;

            return .10;
        }

        public static double GetCritRate(Character attacker, Character defender, BattleCommand skill)
        {
            double critRate = 10;// .0016 * attacker.GetMod((uint)Modifier.CritRating);//Crit rating adds .16% per point
            return Math.Min(critRate, .20);//Crit rate is capped at 20%
        }

        //http://kanican.livejournal.com/55370.html
        // todo: figure that out
        public static double GetResistRate(Character attacker, Character defender, BattleCommand skill)
        {

            return .95;
        }

        //Block Rate follows 4 simple rules:
        //(1) Every point in DEX gives +0.1% rate
        //(2) Every point in "Block Rate" gives +0.2% rate
        //(3) True block proc rate is capped at 75%. No clue on a possible floor.
        //(4) The baseline rate is based on dLVL only(mob stats play no role). The baseline rate is summarized in this raw data sheet: https://imgbox.com/aasLyaJz
        public static double GetBlockRate(Character attacker, Character defender, BattleCommand skill)
        {
            //Shields are required to block.
            if (defender.GetMod((uint)Modifier.HasShield) == 0)//|| !defender.IsFacing(attacker))
                return 0;

            short dlvl = (short) (attacker.GetLevel() - defender.GetLevel());
            double blockRate =  (-2.5 * dlvl) - 5; // Base block rate
            blockRate += attacker.GetMod((uint) Modifier.Dexterity) * .1;// .1% for every dex
            blockRate += attacker.GetMod((uint) Modifier.BlockRate) * .2;// .2% for every block rate
            return Math.Min(blockRate, 25);
        }


        /*
         * HitType helpers. Used for determining if attacks are hits, crits, blocks, etc. and changing their damage based on that
         */

        public static bool TryCrit(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            if (Program.Random.NextDouble() < GetCritRate(attacker, defender, skill))
            {
                action.hitType = HitType.Crit;
                CalculateCritDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        public static bool TryResist(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            if (Program.Random.NextDouble() < GetResistRate(attacker, defender, skill))
            {
                action.hitType = HitType.Resist;
                CalculateResistDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        public static bool TryBlock(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            if (Program.Random.NextDouble() < GetBlockRate(attacker, defender, skill))
            {
                action.hitType = HitType.Block;
                defender.SetProc((int)HitType.Block);
                CalculateBlockDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        public static bool TryParry(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            if (Program.Random.NextDouble() < GetParryRate(attacker, defender, skill))
            {
                action.hitType = HitType.Parry;
                defender.SetProc((int)HitType.Parry);
                CalculateParryDamage(attacker, defender, skill, action);
                return true;
            }

            return false;
        }

        //TryMiss instead of tryHit because hits are the default and don't change damage
        public static bool TryMiss(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            if (Program.Random.NextDouble() > GetHitRate(attacker, defender, skill))
            {
                action.hitType = HitType.Miss;
                action.amount = 0;
                defender.SetProc((int)HitType.Evade);
                attacker.SetProc((int)HitType.Miss);
                return true;
            }
            return false;
        }

        /*
         * Hit Effecthelpers. Different types of hit effects hits use some flags for different things, so they're split into physical, magical, heal, and status
         */
        public static void CalcHitType(Character caster, Character target, BattleCommand skill, BattleAction action)
        {
            //Might be a simpler way to do this?
            switch(action.battleActionType)
            {
                case (BattleActionType.AttackPhysical):
                    SetHitEffectPhysical(caster, target, skill, action);
                    break;
                case (BattleActionType.AttackMagic):
                    SetHitEffectMagical(caster, target, skill, action);
                    break;
                case (BattleActionType.Heal):
                    SetHitEffectHeal(caster, target, skill, action);
                    break;
                case (BattleActionType.Status):
                    SetHitEffectStatus(caster, target, skill, action);
                    break;
            }
        }

        public static void SetHitEffectPhysical(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            //Determine the hittype of the action and change amount of damage it does based on that
            if (!TryMiss(attacker, defender, skill, action))
                if (!TryCrit(attacker, defender, skill, action))
                    if (!TryBlock(attacker, defender, skill, action))
                        TryParry(attacker, defender, skill, action);

            var hitEffect = HitEffect.HitEffectType;

            //Don't know what recoil is actually based on, just guessing
            //Crit is 2 and 3 together
            if (action.hitType == HitType.Crit)
                hitEffect |= HitEffect.CriticalHit;
            else
            {
                double percentDealt = (100.0 * (action.amount / defender.GetMaxHP()));
                if (percentDealt > 5.0)
                    hitEffect |= HitEffect.RecoilLv2;
                else if(percentDealt > 10)
                    hitEffect |= HitEffect.RecoilLv3;
            }
            action.worldMasterTextId = HitTypeTextIds[action.hitType];
            hitEffect |= HitTypeEffects[action.hitType];

            if (skill != null && skill.isCombo && action.hitType > HitType.Evade)
                hitEffect |= (HitEffect)(skill.comboStep << 15);

            //if attack hit the target, take into account protective status effects
            if (action.hitType >= HitType.Parry)
            {
                //Protect / Shell only show on physical/ magical attacks respectively.
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Protect))
                    if (action != null)
                        hitEffect |= HitEffect.Protect;


                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
                    if (action != null)
                        hitEffect |= HitEffect.Stoneskin;
            }
            action.effectId = (uint) hitEffect;
        }

        public static void SetHitEffectMagical(Character attacker, Character defender, BattleCommand skill, BattleAction action)
        {
            //Determine the hit type of the action
            if (!TryMiss(attacker, defender, skill, action))
                if (!TryCrit(attacker, defender, skill, action))
                    TryResist(attacker, defender, skill, action);

            var hitEffect =  HitEffect.MagicEffectType;

            //Recoil levels for spells are a bit different than physical. Recoil levels are used for resists. 
            //Lv1 is for larger resists, Lv2 is for smaller resists and Lv3 is for no resists. Crit is still used for crits
            if (action.hitType == HitType.Resist)
            {
                //todo: calculate resist levels and figure out what the difference between Lv1 and 2 in retail was. For now assuming a full resist with 0 damage dealt is Lv1, all other resists Lv2
                if (action.amount == 0)
                    hitEffect |= HitEffect.RecoilLv1;
                else
                    hitEffect |= HitEffect.RecoilLv2;
            }
            else if (action.hitType == HitType.Crit)
                hitEffect |= HitEffect.Crit;
            else
                hitEffect |= HitEffect.RecoilLv3;

            action.worldMasterTextId = HitTypeTextIds[action.hitType];
            hitEffect |= HitTypeEffects[action.hitType];

            if (skill != null && skill.isCombo)
                hitEffect |= (HitEffect)(skill.comboStep << 15);

            //if attack hit the target, take into account protective status effects
            if (action.hitType >= HitType.Block)
            {
                //Protect / Shell only show on physical/ magical attacks respectively.
                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Shell))
                    if (action != null)
                        hitEffect |= HitEffect.Shell;


                if (defender.statusEffects.HasStatusEffect(StatusEffectId.Stoneskin))
                    if (action != null)
                        hitEffect |= HitEffect.Stoneskin;
            }
            action.effectId = (uint)hitEffect;
        }


        public static void SetHitEffectHeal(Character caster, Character receiver, BattleCommand skill, BattleAction action)
        {
            var hitEffect = HitEffect.MagicEffectType | HitEffect.Heal;
            //Heals use recoil levels in some way as well. Possibly for very low health clutch heals or based on percentage of current health healed (not max health).
            // todo: figure recoil levels out for heals
            hitEffect |= HitEffect.RecoilLv3;
            //do heals crit?

            action.effectId = (uint)hitEffect;
        }

        public static void SetHitEffectStatus(Character caster, Character receiver, BattleCommand skill, BattleAction action)
        {
            var hitEffect = (uint) HitEffect.StatusEffectType | skill.statusId;
            action.effectId = hitEffect;
        }

        public static int CalculateSpellDamage(Character attacker, Character defender, BattleCommand spell)
        {
            // todo: spell formulas and stuff (stoneskin, mods, stats, etc)
            return 69;
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

        //Convert a HitDirection to a BattleCommandPositionBonus. Basically just combining left/right into flank
        public static BattleCommandPositionBonus ConvertHitDirToPosition(HitDirection hitDir)
        {
            BattleCommandPositionBonus position = BattleCommandPositionBonus.None;

            switch(hitDir)
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

        //IsAdditional is needed because additional actions may be required for some actions' effects
        //For instance, Goring Blade's bleed effect requires another action so the first action can still show damage numbers
        //Sentinel doesn't require an additional action because it doesn't need to show those numbers
        public static void TryStatus(Character caster, Character target, BattleCommand skill, BattleAction action, bool isAdditional = true)
        {
            double rand = Program.Random.NextDouble();
            (caster as Player).SendMessage(0x20, "", rand.ToString());
            if (skill != null && action.amount < target.GetHP() && skill.statusId != 0 && action.hitType > HitType.Evade &&  rand < skill.statusChance)
            {
                StatusEffect effect = Server.GetWorldManager().GetStatusEffect(skill.statusId);
                //Because combos might change duration or tier
                if (effect != null)
                {
                    effect.SetDuration(skill.statusDuration);
                    effect.SetTier(skill.statusTier);
                    effect.SetOwner(target);
                    if (target.statusEffects.AddStatusEffect(effect, caster))
                    {
                        //If we need an extra action to show the status text
                        if (isAdditional)
                            action.AddStatusAction(target.actorId, skill.statusId);
                    }
                    else
                        action.worldMasterTextId = 32002;//Is this right?
                }
                else
                {
                    if (target.statusEffects.AddStatusEffect(skill.statusId, 1, 3000, skill.statusDuration, skill.statusTier))
                    {
                        //If we need an extra action to show the status text
                        if (isAdditional)
                            action.AddStatusAction(target.actorId, skill.statusId);
                    }
                    else
                        action.worldMasterTextId = 32002;//Is this right?
                }

            }
        }
    }
}

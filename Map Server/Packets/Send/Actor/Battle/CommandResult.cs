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
using Meteor.Map.actors.chara.ai;
using Meteor.Map.actors.chara.ai.utils;
using Meteor.Map.Actors;

namespace Meteor.Map.packets.send.actor.battle
{
    //These flags can be stacked and mixed, but the client will prioritize certain flags over others.
    [Flags]
    public enum HitEffect : uint
    {
        //This is used for physical attacks
        HitEffectType = 8 << 24,
        //This is used for additioanl effect hits. Only difference from HitEffectType is that it does not play audio.
        AdditionalEffectType = 24 << 24,
        //Status effects use 32 << 24
        StatusEffectType = 32 << 24,
        //When losing a status effect while using a skill, this prevents the hit effect from playing on the actor playing the animation
        StatusLossType = 40 << 24,
        //Magic effects use 48 << 24, this is also used for when statuses are lost on attack
        MagicEffectType = 48 << 24,
        //This places the number on the user regardless of the target this hit effect is for, used for things like bloodbath
        SelfHealType = 72 << 24,
        //Plays the effect animation with no text or additional effects. Unsure if there are any flags. Used for things like Convert
        AnimationEffectType = 96 << 24,

        //Each Type has it's own set of flags. These should be split into their own enums,
        //but for now just keep them all under HitEffect so we don't have to change anything.

        //HitEffectType flags

        //Not setting RecoilLv2 or RecoilLv3 results in the weaker RecoilLv1.
        //These are the recoil animations that play on the target, ranging from weak to strong.
        //The recoil that gets set was likely based on the percentage of HP lost from the attack.
        //These also have a visual effect with heals and spells but in reverse. RecoilLv1 has a large effect, Lv3 has none. Crit is very large
        //For spells they represent resists. Lv0 is a max resist, Lv3 is no resist. Crit is still used for crits.
        //Heals used the same effects sometimes but it isn't clear what for, it seems random? Possibly something like a trait proccing or even just a bug
        RecoilLv1 = 0,
        RecoilLv2 = 1 << 0,
        RecoilLv3 = 1 << 1,

        //Setting both recoil flags triggers the "Critical!" pop-up text and hit visual effect.
        CriticalHit = RecoilLv2 | RecoilLv3,

        //Hit visual and sound effects when connecting with the target.
        //Mixing these flags together will yield different results.
        //Each visual likely relates to a specific weapon.
        //Ex: HitVisual4 flag alone appears to be the visual and sound effect for hand-to-hand attacks.

        //HitVisual is probably based on attack property.
        //HitVisual1 is for slashing attacks
        //HitVisual2 is for piercing attacks
        //HitVisual1 | Hitvisual2 is for blunt attacks
        //HitVisual3 is for projectile attacks
        //Basically take the attack property of a weapon and shift it left 2
        //For auto attacks attack property is weapon's damageAttributeType1
        //Still not totally sure how this works with weaponskills or what hitvisual4 or the other combinations are for
        HitVisual1 = 1 << 2,
        HitVisual2 = 1 << 3,
        HitVisual3 = 1 << 4,
        HitVisual4 = 1 << 5,

        //An additional visual effect that plays on the target when attacked if:
        //The attack is physical and they have the protect buff on.
        //The attack is magical and they have the shell buff on.
        //Special Note: Shell was removed in later versions of the game.
        //Another effect plays when both Protect and Shell flags are activated.
        //Not sure what this effect is.
        //Random guess: if the attack was a hybrid of both physical and magical and the target had both Protect and Shell buffs applied.
        Protect = 1 << 6,
        Shell = 1 << 7,
        ProtectShellSpecial = Protect | Shell,

        //If only HitEffect1 is set out of the hit effects, the "Evade!" pop-up text triggers along with the evade visual.
        //If no hit effects are set, the "Miss!" pop-up is triggered and no hit visual is played.
        HitEffect1 = 1 << 9,
        HitEffect2 = 1 << 10,   //Plays the standard hit visual effect, but with no sound if used alone.
        HitEffect3 = 1 << 11,   //Yellow effect, crit?
        HitEffect4 = 1 << 12,   //Plays the blocking animation
        HitEffect5 = 1 << 13,
        GustyHitEffect = HitEffect3 | HitEffect2,
        GreenTintedHitEffect = HitEffect4 | HitEffect1,

        //For specific animations
        Miss = 0,
        Evade = HitEffect1,
        Hit = HitEffect1 | HitEffect2,
        Crit = HitEffect3,
        Parry = Hit | HitEffect3,
        Block = HitEffect4,

        //Knocks you back away from the attacker.
        KnockbackLv1 = HitEffect4 | HitEffect2 | HitEffect1,
        KnockbackLv2 = HitEffect4 | HitEffect3,
        KnockbackLv3 = HitEffect4 | HitEffect3 | HitEffect1,
        KnockbackLv4 = HitEffect4 | HitEffect3 | HitEffect2,
        KnockbackLv5 = HitEffect4 | HitEffect3 | HitEffect2 | HitEffect1,

        //Knocks you away from the attacker in a counter-clockwise direction.
        KnockbackCounterClockwiseLv1 = HitEffect5,
        KnockbackCounterClockwiseLv2 = HitEffect5 | HitEffect1,

        //Knocks you away from the attacker in a clockwise direction.
        KnockbackClockwiseLv1 = HitEffect5 | HitEffect2,
        KnockbackClockwiseLv2 = HitEffect5 | HitEffect2 | HitEffect1,

        //Completely drags target to the attacker, even across large distances.
        DrawIn = HitEffect5 | HitEffect3,

        //An additional visual effect that plays on the target based on according buff.
        UnknownShieldEffect = HitEffect5 | HitEffect4,
        Stoneskin = HitEffect5 | HitEffect4 | HitEffect1,

        //A special effect when performing appropriate skill combos in succession.
        //Ex: Thunder (SkillCombo1 Effect) -> Thundara (SkillCombo2 Effect) -> Thundaga (SkillCombo3 Effect)
        //Special Note: SkillCombo4 was never actually used in 1.0 since combos only chained up to 3 times maximum.
        SkillCombo1 = 1 << 15,
        SkillCombo2 = 1 << 16,
        SkillCombo3 = SkillCombo1 | SkillCombo2,
        SkillCombo4 = 1 << 17,

        //This is used in the absorb effect for some reason
        Unknown = 1 << 19,

        //AdditionalEffectType flags
        //The AdditionalEffectType is used for the additional effects some weapons have.
        //These effect ids do not repeat the effect of the attack and will not show without a preceding HitEffectType or MagicEffectType

        //It's unclear what this is for. The ifrit fight capture has a BLM using the garuda weapon
        //and this flag is set every time but has no apparent effect.
        UnknownAdditionalFlag = 1,

        //These play effects on the target
        FireEffect = 1 << 10,
        IceEffect = 2 << 10,
        WindEffect = 3 << 10,
        EarthEffect = 4 << 10,
        LightningEffect = 5 << 10,
        WaterEffect = 6 << 10,
        AstralEffect = 7 << 10,         //Possibly for blind?
        UmbralEffect = 8 << 10,         //Posibly for poison?

        //Unknown status effect effects
        StatusEffect1 = 12 << 10,
        StatusEffect2 = 13 << 10,

        HPAbsorbEffect = 14 << 10,
        MPAbsorbEffect = 15 << 10,
        TPAbsorbEffect = 16 << 10,
        TripleAbsorbEffect = 17 << 10,  //Not sure about this
        MoogleEffect = 18 << 10,

        //MagicEffectType Flags
        //THese are used for magic effects that deal or heal damage as well as damage over time effects
        //Crit is the same as HitEffectType
        FullResist = 0,
        WeakResist = 1 << 0,    //Used for level 1, 2, and 3 resists probably
        NoResist = 1 << 1,

        MagicShell = 1 << 4,    //Used when casting on target with shell effects. MagicEffectType doesnt have a flag for protect or stoneskin
        MagicShield = 1 << 5,   //When used with an command that has an animation, this plays a purple shield effect. DoTs also have this flag set (at least on ifrit) but they have no animations so it doesnt show

        // Required for heal text to be blue, not sure if that's all it's used for
        Heal = 1 << 8,
        MP = 1 << 9,            //Causes "MP" text to appear when used with MagicEffectType. | with Heal to make text blue
        TP = 1 << 10,           //Causes "TP" text to appear when used with MagicEffectType. | with Heal to make text blue

        //SelfHealType flags
        //This category causes numbers to appear on the user rather regardless of the target associated with the hit effect and do not play an animation
        //These determine the text that displays (HP has no text)
        SelfHealHP = 0,
        SelfHealMP = 1 << 0,    //Shows MP text on self. | with SelfHeal to make blue
        SelfHealTP = 1 << 1,    //Shows TP text on self. | with SelfHeal to make blue

        //Causes self healing numbers to be blue
        SelfHeal = 1 << 10,
    }

    //Mixing some of these flags will cause the client to crash.
    //Setting a flag higher than Left (0x10-0x80) will cause the client to crash.
    [Flags]
    public enum HitDirection : byte
    {
        None = 0,
        Front = 1 << 0,
        Right = 1 << 1,
        Rear = 1 << 2,
        Left = 1 << 3
    }

    public enum HitType : ushort
    {
        Miss = 0,
        Evade = 1,
        Parry = 2,
        Block = 3,
        SingleResist = 4,
        DoubleResist = 5,
        TripleResist = 6,
        FullResist = 7,
        Hit = 8,
        Crit = 9
    }

    //Type of action
    public enum ActionType : ushort
    {
        None = 0,
        Physical = 1,
        Magic = 2,
        Heal = 3,
        Status = 4
    }

    //There's are two columns in gamecommand that are for action property and action element respectively and both have percentages next to them
    //the percentages are for what percent that property or element factors into the attack. Astral and Umbral are always 33% because they are both 3 elments combined
    //ActionProperty and ActionElement are slightly different. Property defines whta type of attack it is, and 11-13 are used for "sonic, breath, neutral". Neutral is always used for magic
    //For Element 11-13 are used for astral, umbral, and healing magic.
    //Right now we aren't actually using these but when things like resists get better defined we'll have to
    public enum ActionProperty : ushort
    {
        None = 0,
        Slashing = 1,
        Piercing = 2,
        Blunt = 3,
        Projectile = 4,

        Fire = 5,
        Ice = 6,
        Wind = 7,
        Earth = 8,
        Lightning = 9,
        Water = 10,

        //These I'm not sure about. Check gameCommand.csv
        Astral = 11,
        Umbral = 12,
        Heal = 13
    }
    
   
    /*
    public enum ActionProperty : ushort
    {
        None = 0,
        Slashing = 1,
        Piercing = 2,
        Blunt = 3,
        Projectile = 4,

        Fire = 5,
        Ice = 6,
        Wind = 7,
        Earth = 8,
        Lightning = 9,
        Water = 10,
        
        Sonic = 11,
        Breath = 12,
        Neutral = 13,
        Astral = 14,
        Umbral = 15
    }

    public enum ActionElement : ushort
    {
        None = 0,
        Slashing = 1,
        Piercing = 2,
        Blunt = 3,
        Projectile = 4,

        Fire = 5,
        Ice = 6,
        Wind = 7,
        Earth = 8,
        Lightning = 9,
        Water = 10,

        //These I'm not sure about. Check gameCommand.csv
        Astral = 11,
        Umbral = 12,
        Heal = 13
    }*/


    class CommandResult
    {
        public uint targetId;
        public ushort amount;
        public ushort amountMitigated;          //Amount that got blocked/evaded or resisted
        public ushort enmity;                   //Seperate from amount for abilities that cause a different amount of enmity than damage
        public ushort worldMasterTextId;
        public uint effectId;                   //Impact effect, damage/heal/status numbers or name
        public byte param;                      //Which side the battle action is coming from
        public byte hitNum;                     //Which hit in a sequence of hits this is

        /// <summary>
        /// these fields are not actually part of the packet struct
        /// </summary>
        public uint animation;
        public CommandType commandType;         //What type of command was used (ie weaponskill, ability, etc)
        public ActionProperty actionProperty;   //Damage type of the action
        public ActionType actionType;           //Type of this action (ie physical, magic, heal)
        public HitType hitType;

        //Rates, I'm not sure if these need to be stored like this but with the way some buffs work maybe they do?
        //Makes things like Blindside easy at least.
        public double parryRate = 0.0;
        public double blockRate = 0.0;
        public double resistRate = 0.0;
        public double hitRate = 0.0;
        public double critRate = 0.0;

        public CommandResult(uint targetId, ushort worldMasterTextId, uint effectId, ushort amount = 0, byte param = 0, byte hitNum = 1)
        {
            this.targetId = targetId;
            this.worldMasterTextId = worldMasterTextId;
            this.effectId = effectId;
            this.amount = amount;
            this.param = param;
            this.hitNum = hitNum;
            this.hitType = HitType.Hit;
            this.enmity = amount;
            this.commandType = (byte) CommandType.None;
        }

        public CommandResult(uint targetId, BattleCommand command, byte param = 0, byte hitNum = 1)
        {
            this.targetId = targetId;
            this.worldMasterTextId = command.worldMasterTextId;
            this.param = param;
            this.hitNum = hitNum;
            this.commandType = command.commandType;
            this.actionProperty = command.actionProperty;
            this.actionType = command.actionType;
        }

        //Order of what (probably) happens when a skill is used:
        //Buffs that alter things like recast times or that only happen once per skill usage like Power Surge are activated
        //Script calculates damage and handles any special requirements
        //Rates are calculated
        //Buffs that impact indiviudal hits like Blindside or Blood for Blood are activated
        //The final hit type is determined
        //Stoneskin takes damage
        //Final damage amount is calculated using the hit type and defender's stats
        //Buffs that activate or respond to damage like Rampage. Stoneskin gets removed AFTER damage if it falls off.
        //Additional effects that are a part of the skill itself or weapon in case of auto attacks take place like status effects
        //Certain buffs that alter the whole skill fall off (Resonance, Excruciate)
        
        public void DoAction(Character caster, Character target, BattleCommand skill, CommandResultContainer results)
        {
            //First calculate rates for hit/block/etc
            CalcRates(caster, target, skill);

            //Next, modify those rates based on preaction buffs
            //Still not sure how we shouldh andle these
            PreAction(caster, target, skill, results);

            BattleUtils.DoAction(caster, target, skill, this, results);
        }


        //Calculate the chance of hitting/critting/etc
        public void CalcRates(Character caster, Character target, BattleCommand skill)
        {
            hitRate = BattleUtils.GetHitRate(caster, target, skill, this);
            critRate = BattleUtils.GetCritRate(caster, target, skill, this);
            blockRate = BattleUtils.GetBlockRate(caster, target, skill, this);
            parryRate = BattleUtils.GetParryRate(caster, target, skill, this);
            resistRate = BattleUtils.GetResistRate(caster, target, skill, this);
        }

        //These are buffs that activate before the action hits. Usually they change things like hit or crit rates or damage
        public void PreAction(Character caster, Character target, BattleCommand skill, CommandResultContainer results)
        {
            target.statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnPreactionTarget, "onPreAction", caster, target, skill, this, results);

            caster.statusEffects.CallLuaFunctionByFlag((uint)StatusEffectFlags.ActivateOnPreactionCaster, "onPreAction", caster, target, skill, this, results);
        }

        //Try and apply a status effect
        public void TryStatus(Character caster, Character target, BattleCommand skill, CommandResultContainer results, bool isAdditional = true)
        {
            BattleUtils.TryStatus(caster, target, skill, this, results, isAdditional);
        }

        public ushort GetHitType()
        {
            return (ushort)hitType;
        }

        public void SetTextId(ushort id)
        {
            worldMasterTextId = id;
        }

        //Whether this action didn't miss, and wasn't evaded or resisted
        public bool ActionLanded()
        {
            return hitType > HitType.Evade && hitType != HitType.SingleResist && hitType != HitType.DoubleResist && hitType != HitType.FullResist;
        }
    }
}

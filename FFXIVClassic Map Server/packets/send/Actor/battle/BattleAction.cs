using FFXIVClassic.Common;
using System;

namespace  FFXIVClassic_Map_Server.packets.send.actor.battle
{
    //These flags can be stacked and mixed, but the client will prioritize certain flags over others.
    [Flags]
    public enum HitEffect : uint
    {        
        //All HitEffects have the last byte 0x8
        HitEffectType = 8 << 24,

        //Not setting RecoilLv2 or RecoilLv3 results in the weaker RecoilLv1.
        //These are the recoil animations that play on the target, ranging from weak to strong.
        //The recoil that gets set was likely based on the percentage of HP lost from the attack.
        RecoilLv1 = 0 | HitEffectType,
        RecoilLv2 = 1 << 0 | HitEffectType,
        RecoilLv3 = 1 << 1 | HitEffectType,

        //Setting both recoil flags triggers the "Critical!" pop-up text and hit visual effect.
        CriticalHit = RecoilLv2 | RecoilLv3,

        //Hit visual and sound effects when connecting with the target.
        //Mixing these flags together will yield different results.
        //Each visual likely relates to a specific weapon.
        //Ex: HitVisual4 flag alone appears to be the visual and sound effect for hand-to-hand attacks.
        HitVisual1 = 1 << 2 | HitEffectType,
        HitVisual2 = 1 << 3 | HitEffectType,
        HitVisual3 = 1 << 4 | HitEffectType,
        HitVisual4 = 1 << 5 | HitEffectType,

        //An additional visual effect that plays on the target when attacked if:
        //The attack is physical and they have the protect buff on.
        //The attack is magical and they have the shell buff on.
        //Special Note: Shell was removed in later versions of the game.
        //Another effect plays when both Protect and Shell flags are activated.
        //Not sure what this effect is.
        //Random guess: if the attack was a hybrid of both physical and magical and the target had both Protect and Shell buffs applied.
        Protect = 1 << 6 | HitEffectType,
        Shell = 1 << 7 | HitEffectType,
        ProtectShellSpecial = Protect | Shell,

        //Unknown = 1 << 8, -- Not sure what this flag does.

        //If only HitEffect1 is set out of the hit effects, the "Evade!" pop-up text triggers along with the evade visual.
        //If no hit effects are set, the "Miss!" pop-up is triggered and no hit visual is played.
        HitEffect1 = 1 << 9 | HitEffectType,
        HitEffect2 = 1 << 10 | HitEffectType, //Plays the standard hit visual effect, but with no sound if used alone.
        Hit = HitEffect1 | HitEffect2, //A standard hit effect with sound effect.
        HitEffect3 = 1 << 11 | HitEffectType,
        HitEffect4 = 1 << 12 | HitEffectType,
        HitEffect5 = 1 << 13 | HitEffectType,
        GustyHitEffect = HitEffect3 | HitEffect2,
        GreenTintedHitEffect = HitEffect4 | HitEffect1,

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

        //Unknown = 1 << 14, -- Not sure what this flag does; might be another HitEffect.

        //A special effect when performing appropriate skill combos in succession.
        //Ex: Thunder (SkillCombo1 Effect) -> Thundara (SkillCombo2 Effect) -> Thundaga (SkillCombo3 Effect)
        //Special Note: SkillCombo4 was never actually used in 1.0 since combos only chained up to 3 times maximum.
        SkillCombo1 = 1 << 15 | HitEffectType,
        SkillCombo2 = 1 << 16 | HitEffectType,
        SkillCombo3 = SkillCombo1 | SkillCombo2,
        SkillCombo4 = 1 << 17 | HitEffectType

        //Flags beyond here are unknown/untested.
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

    class BattleAction
    {
        public uint targetId;
        public ushort amount;
        public ushort worldMasterTextId;
        public uint effectId;
        public byte param;
        public byte unknown;

        /// <summary>
        /// this field is not actually part of the packet struct
        /// </summary>
        //public uint animation;

        public BattleAction(uint targetId, ushort worldMasterTextId, uint effectId, ushort amount = 0, byte param = 0, byte unknown = 0)
        {
            this.targetId = targetId;            
            this.worldMasterTextId = worldMasterTextId;
            this.effectId = effectId;
            this.amount = amount;
            this.param = param;
            this.unknown = unknown;        
        }
    }
}

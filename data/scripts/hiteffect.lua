HitEffect =
{        
    --All HitEffects have the last byte 0x8
    HitEffectType = 134217728, --8 << 24
    --Status effects use 32 <<,24
    StatusEffectType = 536870912,--32 << 24,
    --Heal effects use 48 << 24
    MagicEffectType = 805306368,--48 << 24

    --Not setting RecoilLv2 or RecoilLv3 results in the weaker RecoilLv1.
    --These are the recoil animations that play on the target, ranging from weak to strong.
    --The recoil that gets set was likely based on the percentage of HP lost from the attack.
    --These are used for resists for spells. RecoilLV1 is a full resist, RecoilLv2 is a partial resist, RecoilLv3 is no resist, CriticalHit is a crit
    RecoilLv1 = 0,
    RecoilLv2 = 1,
    RecoilLv3 = 2,

    --Setting both recoil flags triggers the "Critical!" pop-up text and hit visual effect.
    CriticalHit = 3,

    --Hit visual and sound effects when connecting with the target.
    --Mixing these flags together will yield different results.
    --Each visual likely relates to a specific weapon.
    --Ex: HitVisual4 flag alone appears to be the visual and sound effect for hand-to-hand attacks.

    --HitVisual is probably based on attack property.
    --HitVisual1 is for slashing attacks
    --HitVisual2 is for piercing attacks
    --HitVisual1 | Hitvisual2 is for blunt attacks
    --HitVisual3 is for projectile attacks
    --Basically, takes the attack property as defined by the weapon and shifts it left 2
    --For auto attacks attack property is weapon's damageAttributeType1
    --Still not totally sure how this works with weaponskills or what hitvisual4 or the other combinations are for
    HitVisual1 = 4,
    HitVisual2 = 8,
    HitVisual3 = 16,
    HitVisual4 = 32,
    

    --An additional visual effect that plays on the target when attacked if:
    --The attack is physical and they have the protect buff on.
    --The attack is magical and they have the shell buff on.
    --Special Note: Shell was removed in later versions of the game.
    --Another effect plays when both Protect and Shell flags are activated.
    --Not sure what this effect is.
    --Random guess: if the attack was a hybrid of both physical and magical and the target had both Protect and Shell buffs applied.
    Protect = 64,
    Shell = 128,
    ProtectShellSpecial = 192,-- Protect | Shell,

    Heal = 256,-- Required for heal text to be blue along with HealEffectType, not sure if that's all it's used for
    MP = 512,
    
    --If only HitEffect1 is set out of the hit effects, the "Evade!" pop-up text triggers along with the evade visual.
    --If no hit effects are set, the "Miss!" pop-up is triggered and no hit visual is played.
    HitEffect1 = 512,
    HitEffect2 = 1024,   --Plays the standard hit visual effect, but with no sound if used alone.
    HitEffect3 = 2048,   --Yellow effect, crit?
    HitEffect4 = 4096,   --Plays the blocking animation
    HitEffect5 = 8192,
    GustyHitEffect = 3072,--HitEffect3 | HitEffect2,
    GreenTintedHitEffect = 4608,-- HitEffect4 | HitEffect1,

    --For specific animations
    Miss = 0,
    Evade = 512,
    Hit = 1536, --HitEffect1 | HitEffect2,
    Parry = 3584, --Hit | HitEffect3,
    Block = 4096,
    Crit = 2048,

    --Knocks you back away from the attacker.
    KnockbackLv1 = 5632,-- HitEffect4 | HitEffect2 | HitEffect1,
    KnockbackLv2 = 6144,-- HitEffect4 | HitEffect3,
    KnockbackLv3 = 6656,-- HitEffect4 | HitEffect3 | HitEffect1,
    KnockbackLv4 = 7168,-- HitEffect4 | HitEffect3 | HitEffect2,
    KnockbackLv5 = 7680,-- HitEffect4 | HitEffect3 | HitEffect2 | HitEffect1,

    --Knocks you away from the attacker in a counter-clockwise direction.
    KnockbackCounterClockwiseLv1 = 8192,
    KnockbackCounterClockwiseLv2 = 8704,-- HitEffect5 | HitEffect1,

    --Knocks you away from the attacker in a clockwise direction.
    KnockbackClockwiseLv1 = 9216,-- HitEffect5 | HitEffect2,
    KnockbackClockwiseLv2 = 9728,-- HitEffect5 | HitEffect2 | HitEffect1,

    --Completely drags target to the attacker, even across large distances.
    DrawIn = 10240,-- HitEffect5 | HitEffect3,

    --An additional visual effect that plays on the target based on according buff.
    UnknownShieldEffect = 12288,-- HitEffect5 | HitEffect4,
    Stoneskin = 12800,-- HitEffect5 | HitEffect4 | HitEffect1,

    --Unknown = 1 << 14, -- Not sure what this flag does; might be another HitEffect.

    --A special effect when performing appropriate skill combos in succession.
    --Ex: Thunder (SkillCombo1 Effect) -> Thundara (SkillCombo2 Effect) -> Thundaga (SkillCombo3 Effect)
    --Special Note: SkillCombo4 was never actually used in 1.0 since combos only chained up to 3 times maximum.
    SkillCombo1 = 32768,
    SkillCombo2 = 65536,
    SkillCombo3 = 98304,-- SkillCombo1 | SkillCombo2,
    SkillCombo4 = 131072

    --Flags beyond here are unknown/untested.
}
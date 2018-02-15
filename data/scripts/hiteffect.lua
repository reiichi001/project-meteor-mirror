HitEffect =
{        
    --All HitEffects have the last byte 0x8
    HitEffectType = 8 << 24,
    --Status effects use 32 << 24
    StatusEffectType = 32 << 24,
    --Heal effects use 48 << 24
    MagicEffectType = 48 << 24,

    --Not setting RecoilLv2 or RecoilLv3 results in the weaker RecoilLv1.
    --These are the recoil animations that play on the target, ranging from weak to strong.
    --The recoil that gets set was likely based on the percentage of HP lost from the attack.
    --These are used for resists for spells. RecoilLV1 is a resist, RecoilLv2 is a partial resist. 
    --Don't know what CriticalHit is for but it has a larger effect than Lv1
    RecoilLv1 = 0,
    RecoilLv2 = 1 << 0,
    RecoilLv3 = 1 << 1,

    --Setting both recoil flags triggers the "Critical!" pop-up text and hit visual effect.
    CriticalHit = RecoilLv2 | RecoilLv3,

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
    HitVisual1 = 1 << 2,
    HitVisual2 = 1 << 3,
    HitVisual3 = 1 << 4,
    HitVisual4 = 1 << 5,
    

    --An additional visual effect that plays on the target when attacked if:
    --The attack is physical and they have the protect buff on.
    --The attack is magical and they have the shell buff on.
    --Special Note: Shell was removed in later versions of the game.
    --Another effect plays when both Protect and Shell flags are activated.
    --Not sure what this effect is.
    --Random guess: if the attack was a hybrid of both physical and magical and the target had both Protect and Shell buffs applied.
    Protect = 1 << 6 | HitEffectType,
    Shell = 1 << 7 | HitEffectType,
    ProtectShellSpecial = Protect | Shell,

    Heal = 1 << 8,-- Required for heal text to be blue along with HealEffectType, not sure if that's all it's used for

    --If only HitEffect1 is set out of the hit effects, the "Evade!" pop-up text triggers along with the evade visual.
    --If no hit effects are set, the "Miss!" pop-up is triggered and no hit visual is played.
    HitEffect1 = 1 << 9,
    HitEffect2 = 1 << 10,   --Plays the standard hit visual effect, but with no sound if used alone.
    HitEffect3 = 1 << 11,   --Yellow effect, crit?
    HitEffect4 = 1 << 12,   --Plays the blocking animation
    HitEffect5 = 1 << 13,
    GustyHitEffect = HitEffect3 | HitEffect2,
    GreenTintedHitEffect = HitEffect4 | HitEffect1,

    --For specific animations
    Miss = 0,
    Evade = HitEffect1,
    Hit = HitEffect1 | HitEffect2,
    Parry = Hit | HitEffect3,
    Block = HitEffect4,
    Crit = HitEffect3,

    --Knocks you back away from the attacker.
    KnockbackLv1 = HitEffect4 | HitEffect2 | HitEffect1,
    KnockbackLv2 = HitEffect4 | HitEffect3,
    KnockbackLv3 = HitEffect4 | HitEffect3 | HitEffect1,
    KnockbackLv4 = HitEffect4 | HitEffect3 | HitEffect2,
    KnockbackLv5 = HitEffect4 | HitEffect3 | HitEffect2 | HitEffect1,

    --Knocks you away from the attacker in a counter-clockwise direction.
    KnockbackCounterClockwiseLv1 = HitEffect5,
    KnockbackCounterClockwiseLv2 = HitEffect5 | HitEffect1,

    --Knocks you away from the attacker in a clockwise direction.
    KnockbackClockwiseLv1 = HitEffect5 | HitEffect2,
    KnockbackClockwiseLv2 = HitEffect5 | HitEffect2 | HitEffect1,

    --Completely drags target to the attacker, even across large distances.
    DrawIn = HitEffect5 | HitEffect3,

    --An additional visual effect that plays on the target based on according buff.
    UnknownShieldEffect = HitEffect5 | HitEffect4,
    Stoneskin = HitEffect5 | HitEffect4 | HitEffect1,

    --Unknown = 1 << 14, -- Not sure what this flag does; might be another HitEffect.

    --A special effect when performing appropriate skill combos in succession.
    --Ex: Thunder (SkillCombo1 Effect) -> Thundara (SkillCombo2 Effect) -> Thundaga (SkillCombo3 Effect)
    --Special Note: SkillCombo4 was never actually used in 1.0 since combos only chained up to 3 times maximum.
    SkillCombo1 = 1 << 15,
    SkillCombo2 = 1 << 16,
    SkillCombo3 = SkillCombo1 | SkillCombo2,
    SkillCombo4 = 1 << 17

    --Flags beyond here are unknown/untested.
}
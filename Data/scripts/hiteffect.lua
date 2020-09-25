HitEffect =
{        
    --This is used for physical attacks
    HitEffectType = bit32.lshift(8,24),
    --This is used for additioanl effect hits. Only difference from HitEffectType is that it does not play audio.
    AdditionalEffectType = bit32.lshift(24, 24),
    --Status effects use 32 << 24
    StatusEffectType = bit32.lshift(32, 24),
    --When losing a status effect while using a skill, this prevents the hit effect from playing on the actor playing the animation
    StatusLossType = bit32.lshift(40, 24),
    --Magic effects use 48 << 24, this is also used for when statuses are lost on attack
    MagicEffectType = bit32.lshift(48, 24),
    --This places the number on the user regardless of the target this hit effect is for, used for things like bloodbath
    SelfHealType = bit32.lshift(72, 24),
    --Plays the effect animation with no text or additional effects. Unsure if there are any flags. Used for things like Convert
    AnimationEffectType = bit32.lshift(96, 24),

    --Each Type has it's own set of flags. These should be split into their own enums,
    --but for now just keep them all under HitEffect so we don't have to change anything.

    --HitEffectType flags

    --Not setting RecoilLv2 or RecoilLv3 results in the weaker RecoilLv1.
    --These are the recoil animations that play on the target, ranging from weak to strong.
    --The recoil that gets set was likely based on the percentage of HP lost from the attack.
    --These also have a visual effect with heals and spells but in reverse. RecoilLv1 has a large effect, Lv3 has none. Crit is very large
    --For spells they represent resists. Lv0 is a max resist, Lv3 is no resist. Crit is still used for crits.
    --Heals used the same effects sometimes but it isn't clear what for, it seems random? Possibly something like a trait proccing or even just a bug
    RecoilLv1 = 0,
    RecoilLv2 = bit32.lshift(1, 0),
    RecoilLv3 = bit32.lshift(1, 1),

    --Setting both recoil flags triggers the "Critical!" pop-up text and hit visual effect.
    CriticalHit = 3,                        --RecoilLv2 | RecoilLv3

    --Hit visual and sound effects when connecting with the target.
    --Mixing these flags together will yield different results.
    --Each visual likely relates to a specific weapon.
    --Ex: HitVisual4 flag alone appears to be the visual and sound effect for hand-to-hand attacks.

    --HitVisual is probably based on attack property.
    --HitVisual1 is for slashing attacks
    --HitVisual2 is for piercing attacks
    --HitVisual1 | Hitvisual2 is for blunt attacks
    --HitVisual3 is for projectile attacks
    --Basically take the attack property of a weapon and shift it left 2
    --For auto attacks attack property is weapon's damageAttributeType1
    --Still not totally sure how this works with weaponskills or what hitvisual4 or the other combinations are for
    HitVisual1 = bit32.lshift(1, 2),
    HitVisual2 = bit32.lshift(1, 3),
    HitVisual3 = bit32.lshift(1, 4),
    HitVisual4 = bit32.lshift(1, 5),

    --An additional visual effect that plays on the target when attacked if:
    --The attack is physical and they have the protect buff on.
    --The attack is magical and they have the shell buff on.
    --Special Note: Shell was removed in later versions of the game.
    --Another effect plays when both Protect and Shell flags are activated.
    --Not sure what this effect is.
    --Random guess: if the attack was a hybrid of both physical and magical and the target had both Protect and Shell buffs applied.
    Protect = bit32.lshift(1, 6),
    Shell = bit32.lshift(1, 7),
    ProtectShellSpecial = 192,              --Protect | Shell

    --If only HitEffect1 is set out of the hit effects, the "Evade!" pop-up text triggers along with the evade visual.
    --If no hit effects are set, the "Miss!" pop-up is triggered and no hit visual is played.
    HitEffect1 = bit32.lshift(1, 9),
    HitEffect2 = bit32.lshift(1, 10),       --Plays the standard hit visual effect, but with no sound if used alone.
    HitEffect3 = bit32.lshift(1, 11),       --Yellow effect, crit?
    HitEffect4 = bit32.lshift(1, 12),       --Plays the blocking animation
    HitEffect5 = bit32.lshift(1, 13),
    GustyHitEffect = 3072,                  --HitEffect3 | HitEffect2
    GreenTintedHitEffect = 4608,            --HitEffect4 | HitEffect1

    --For specific animations
    Miss = 0,
    Evade = HitEffect1,
    Hit = 1536,                             --HitEffect1 | HitEffect2,
    Crit = HitEffect3,
    Parry = 3584,                           --Hit | HitEffect3,
    Block = HitEffect4,

    --Knocks you back away from the attacker.
    KnockbackLv1 = 5632,                    --HitEffect4 | HitEffect2 | HitEffect1
    KnockbackLv2 = 6144,                    --HitEffect4 | HitEffect3
    KnockbackLv3 = 6656,                    --HitEffect4 | HitEffect3 | HitEffect1
    KnockbackLv4 = 7168,                    --HitEffect4 | HitEffect3 | HitEffect2
    KnockbackLv5 = 7680,                    --HitEffect4 | HitEffect3 | HitEffect2 | HitEffect1

    --Knocks you away from the attacker in a counter-clockwise direction.
    KnockbackCounterClockwiseLv1 = HitEffect5,
    KnockbackCounterClockwiseLv2 = 8704,    --HitEffect5 | HitEffect1

    --Knocks you away from the attacker in a clockwise direction.
    KnockbackClockwiseLv1 = 9216,           --HitEffect5 | HitEffect2
    KnockbackClockwiseLv2 = 9728,           --HitEffect5 | HitEffect2 | HitEffect1

    --Completely drags target to the attacker, even across large distances.
    DrawIn = 10240,                         --HitEffect5 | HitEffect3

    --An additional visual effect that plays on the target based on according buff.
    UnknownShieldEffect = 12288,            --HitEffect5 | HitEffect4
    Stoneskin = 12800,                      --HitEffect5 | HitEffect4 | HitEffect1

    --A special effect when performing appropriate skill combos in succession.
    --Ex: Thunder (SkillCombo1 Effect) -> Thundara (SkillCombo2 Effect) -> Thundaga (SkillCombo3 Effect)
    --Special Note: SkillCombo4 was never actually used in 1.0 since combos only chained up to 3 times maximum.
    SkillCombo1 = bit32.lshift(1, 15),
    SkillCombo2 = bit32.lshift(1, 16),
    SkillCombo3 = 98304,                    --SkillCombo1 | SkillCombo2
    SkillCombo4 = bit32.lshift(1, 17),

    --This is used in the absorb effect for some reason
    Unknown = bit32.lshift(1, 19),

    --AdditionalEffectType flags
    --The AdditionalEffectType is used for the additional effects some weapons have.
    --These effect ids do not repeat the effect of the attack and will not show without a preceding HitEffectType or MagicEffectType

    --It's unclear what this is for. The ifrit fight capture has a BLM using the garuda weapon
    --and this flag is set every time but has no apparent effect.
    UnknownAdditionalFlag = 1,

    --These play effects on the target
    FireEffect = bit32.lshift(1, 10),
    IceEffect = bit32.lshift(2, 10),
    WindEffect = bit32.lshift(3, 10),
    EarthEffect = bit32.lshift(4, 10),
    LightningEffect = bit32.lshift(5, 10),
    WaterEffect = bit32.lshift(6, 10),
    AstralEffect = bit32.lshift(7, 10),         --Possibly for blind?
    UmbralEffect = bit32.lshift(8, 10),         --Posibly for poison?

    --Unknown status effect effects
    StatusEffect1 = bit32.lshift(12, 10),
    StatusEffect2 = bit32.lshift(13, 10),

    HPAbsorbEffect = bit32.lshift(14, 10),
    MPAbsorbEffect = bit32.lshift(15, 10),
    TPAbsorbEffect = bit32.lshift(16, 10),
    TripleAbsorbEffect = bit32.lshift(17, 10),  --Not sure about this
    MoogleEffect = bit32.lshift(18, 10),

    --MagicEffectType Flags
    --THese are used for magic effects that deal or heal damage as well as damage over time effects
    --Crit is the same as HitEffectType
    FullResist = 0,
    WeakResist = bit32.lshift(1, 0),    --Used for level 1, 2, and 3 resists probably
    NoResist = bit32.lshift(1, 1),

    MagicShell = bit32.lshift(1, 4),    --Used when casting on target with shell effects. MagicEffectType doesnt have a flag for protect or stoneskin
    MagicShield = bit32.lshift(1, 5),   --When used with an command that has an animation, this plays a purple shield effect. DoTs also have this flag set (at least on ifrit) but they have no animations so it doesnt show

    -- Required for heal text to be blue, not sure if that's all it's used for
    Heal = bit32.lshift(1, 8),
    MP = bit32.lshift(1, 9),            --Causes "MP" text to appear when used with MagicEffectType. | with Heal to make text blue
    TP = bit32.lshift(1, 10),           --Causes "TP" text to appear when used with MagicEffectType. | with Heal to make text blue

    --SelfHealType flags
    --This category causes numbers to appear on the user rather regardless of the target associated with the hit effect and do not play an animation
    --These determine the text that displays (HP has no text)
    SelfHealHP = 0,
    SelfHealMP = bit32.lshift(1, 0),    --Shows MP text on self. | with SelfHeal to make blue
    SelfHealTP = bit32.lshift(1, 1),    --Shows TP text on self. | with SelfHeal to make blue

    --Causes self healing numbers to be blue
    SelfHeal = bit32.lshift(1, 10),
}
CommandType = 
{
    None = 0,
    AutoAttack = 1,
    Weaponskill = 2,
    Ability = 3,
    Spell = 4
}

ActionType =
{
    None = 0,
    Physical = 1,
    Magic = 2,
    Heal = 3,
    Status = 4
}

ActionProperty = 
{
    None = 0,
    Physical = 1,
    Magic = 2,
    Heal = 4,
    Status = 8,
    Ranged = 16
}

DamageTakenType =
{
    None,
    Attack,
    Magic,
    Weaponskill,
    Ability
}

HitDirection =
{
    None = 0,
    Front = 1,
    Right = 2,
    Rear = 4,
    Left = 8
}

HitType =
{
    Miss = 0,
    Evade = 1,
    Parry = 2,
    Block = 3,
    Resist = 4,
    Hit = 5,
    Crit = 6
}

TargetFindAOEType =
{
    None = 0,
    Circle = 1,
    Cone = 2,
    Box = 3
}

StatusEffectFlags =
{
    None = 0,

    --Loss flags - Do we need loseonattacking/caststart? Could just be done with activate flags
    LoseOnDeath =                   bit32.lshift(1, 0),     -- effects removed on death
    LoseOnZoning =                  bit32.lshift(1, 1),     -- effects removed on zoning
    LoseOnEsuna =                   bit32.lshift(1, 2),     -- effects which can be removed with esuna (debuffs)
    LoseOnDispel =                  bit32.lshift(1, 3),     -- some buffs which player might be able to dispel from mob
    LoseOnLogout =                  bit32.lshift(1, 4),     -- effects removed on logging out
    LoseOnAttacking =               bit32.lshift(1, 5),     -- effects removed when owner attacks another entity
    LoseOnCastStart =               bit32.lshift(1, 6),     -- effects removed when owner starts casting
    LoseOnAggro =                   bit32.lshift(1, 7),     -- effects removed when owner gains enmity (swiftsong)
    LoseOnClassChange =             bit32.lshift(1, 8),     --Effect falls off whhen changing class

    --Activate flags
    ActivateOnCastStart =           bit32.lshift(1, 9),     --Activates when a cast starts.
    ActivateOnCommandStart =        bit32.lshift(1, 10),    --Activates when a command is used, before iterating over targets. Used for things like power surge, excruciate.
    ActivateOnCommandFinish =       bit32.lshift(1, 11),    --Activates when the command is finished, after all targets have been iterated over. Used for things like Excruciate and Resonance falling off.
    ActivateOnPreactionTarget =     bit32.lshift(1, 12),    --Activates after initial rates are calculated for an action against owner
    ActivateOnPreactionCaster =     bit32.lshift(1, 13),    --Activates after initial rates are calculated for an action by owner
    ActivateOnDamageTaken =         bit32.lshift(1, 14),
    ActivateOnHealed =              bit32.lshift(1, 15),

    --Should these be rolled into DamageTaken?
    ActivateOnMiss =                bit32.lshift(1, 16),    --Activates when owner misses
    ActivateOnEvade =               bit32.lshift(1, 17),    --Activates when owner evades
    ActivateOnParry =               bit32.lshift(1, 18),    --Activates when owner parries
    ActivateOnBlock =               bit32.lshift(1, 19),    --Activates when owner evades
    ActivateOnHit =                 bit32.lshift(1, 20),    --Activates when owner hits
    ActivateOnCrit =                bit32.lshift(1, 21),    --Activates when owner crits

    --Prevent flags. Sleep/stun/petrify/etc combine these
    PreventSpell =                  bit32.lshift(1, 22),    -- effects which prevent using spells, such as silence
    PreventWeaponSkill =            bit32.lshift(1, 23),    -- effects which prevent using weaponskills, such as pacification
    PreventAbility =                bit32.lshift(1, 24),    -- effects which prevent using abilities, such as amnesia
    PreventAttack =                 bit32.lshift(1, 25),    -- effects which prevent basic attacks
    PreventMovement =               bit32.lshift(1, 26),    -- effects which prevent movement such as bind, still allows turning in place
    PreventTurn =                   bit32.lshift(1, 27),    -- effects which prevent turning, such as stun
    PreventUntarget =               bit32.lshift(1, 28),    -- effects which prevent changing targets, such as fixation
    Stance =                        bit32.lshift(1, 29)     -- effects that do not have a timer
}
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
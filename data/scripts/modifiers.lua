modifiersGlobal = 
{
        None                   = 0,
        Hp                     = 1,
        HpPercent              = 2,
        Mp                     = 3,
        MpPercent              = 4,
        Tp                     = 5,
        TpPercent              = 6,
        Regen                  = 7,
        Refresh                = 8,
        Strength               = 9,
        Vitality               = 10,
        Dexterity              = 11,
        Intelligence           = 12,
        Mind                   = 13,
        Piety                  = 14,
        Attack                 = 15,
        Accuracy               = 16,
        Defense                = 17,
        Evasion                = 18,
        MagicAttack            = 19,
        MagicHeal              = 20, -- is this needed? shouldnt it just be calc'd from mind
        MagicAccuracy          = 21,
        MagicEvasion           = 22,
        MagicDefense           = 23,
        MagicEnhancePotency    = 24,
        MagicEnfeeblingPotency = 25,
        ResistFire             = 26,
        ResistIce              = 27,
        ResistWind             = 28,
        ResistLightning        = 29,
        ResistEarth            = 30,
        ResistWater            = 31, -- <3 u jorge
        AttackRange            = 32,
        Speed                  = 33,
        AttackDelay            = 34,
        
        CraftProcessing        = 35,
        CraftMagicProcessing   = 36,
        CraftProcessControl    = 37,

        HarvestPotency         = 38,
        HarvestLimit           = 39,
        HarvestRate            = 40,

        Raise                  = 41,
        MinimumHpLock          = 42, -- hp cannot fall below this value
}

mobModifiersGlobal = 
{
        None              = 0,
        SpawnLeash        = 1, -- how far can i move before i deaggro target
        SightRange        = 2, -- how close does target need to be for me to detect by sight
        SoundRange        = 3, -- how close does target need to be for me to detect by sound
        BuffChance        = 4, 
        HealChance        = 5,
        SkillUseChance    = 6,
        LinkRadius        = 7,
        MagicDelay        = 8,
        SpecialDelay      = 9,
        ExpBonus          = 10, -- 
        IgnoreSpawnLeash  = 11, -- pursue target forever
        DrawIn            = 12, -- do i suck people in around me
        HpScale           = 13, --
        Assist            = 14, -- gotta call the bois 
        NoMove            = 15, -- cant move
        ShareTarget       = 16, -- use this actor's id as target id
        AttackScript      = 17, -- call my script's onAttack whenever i attack
        DefendScript      = 18, -- call my script's onDamageTaken whenever i take damage
        SpellScript       = 19, -- call my script's onSpellCast whenever i finish casting
        WeaponskillScript = 20, -- call my script's onWeaponSkill whenever i finish using a weaponskill
        AbilityScript     = 21, -- call my script's onAbility whenever i finish using an ability
        CallForHelp       = 22, -- actor with this id outside of target's party with this can attack me
        FreeForAll        = 23, -- any actor can attack me
        Roams             = 24, -- Do I walk around?
        RoamDelay         = 25  -- What is the delay between roam ticks
}
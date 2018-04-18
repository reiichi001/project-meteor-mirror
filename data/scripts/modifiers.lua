modifiersGlobal =       
{       
        NAMEPLATE_SHOWN         = 0,
        TARGETABLE              = 1,
        NAMEPLATE_SHOWN2        = 2,
        --NAMEPLATE_SHOWN2 = 3,

        Strength                = 3,
        Vitality                = 4,
        Dexterity               = 5,
        Intelligence            = 6,
        Mind                    = 7,
        Piety                   = 8,

        ResistFire              = 9,
        ResistIce               = 10,
        ResistWind              = 11,
        ResistLightning         = 12,
        ResistEarth             = 13,
        ResistWater             = 14,

        Accuracy                = 15,
        Evasion                 = 16,
        Attack                  = 17,
        Defense                 = 18,   --Is there a magic defense stat? 19 maybe?
        MagicAttack             = 23,
        MagicHeal               = 24,
        MagicEnhancePotency     = 25,
        MagicEnfeeblingPotency  = 26,

        MagicAccuracy           = 27,
        MagicEvasion            = 28,

        CraftProcessing         = 30,
        CraftMagicProcessing    = 31,
        CraftProcessControl     = 32,

        HarvestPotency          = 33,
        HarvestLimit            = 34,
        HarvestRate             = 35,

        None                    = 36,
        Hp                      = 37,
        HpPercent               = 38,
        Mp                      = 39,
        MpPercent               = 40,
        Tp                      = 41,
        TpPercent               = 42,
        Regen                   = 43,
        Refresh                 = 44,

        AttackRange             = 45,
        Speed                   = 46,
        AttackDelay             = 47,

        Raise                   = 48,
        MinimumHpLock           = 49,   -- hp cannot fall below this value
        AttackType              = 50,   -- slashing, piercing, etc
        BlockRate               = 51,
        Block                   = 52,
        CritRating              = 53,
        HasShield               = 54,   -- Need this because shields are required for blocks. Could have used BlockRate or Block but BlockRate is provided by Gallant Sollerets and Block is provided by some buffs.
        HitCount                = 55,   -- Amount of hits in an auto attack. Usually 1, 2 for h2h, 3 with spinning heel

        --Flat percent increases to these rates. Probably a better way to do this
        RawEvadeRate            = 56,   
        RawParryRate            = 57,
        RawBlockRate            = 58,
        RawResistRate           = 59,
        RawHitRate              = 60,
        RawCritRate             = 61,

        DamageTakenDown         = 62,   -- Percent damage taken down
        StoreTP                 = 63,   --.1% extra tp per point. Lancer trait is 50 StoreTP
        PhysicalCritRate        = 64,   --CritRating but only for physical attacks. Increases chance of critting.
        PhysicalCritEvasion     = 65,   --Opposite of CritRating. Reduces chance of being crit by phyiscal attacks
        PhysicalCritAttack      = 66,   --Increases damage done by Physical Critical hits
        PhysicalCritResilience  = 67,   --Decreases damage taken by Physical Critical hits
        Parry                   = 68,   --Increases chance to parry
        MagicCritPotency        = 69,   --Increases 
        Regain                  = 70,   --TP regen, should be -90 out of combat, Invigorate sets to 100+ depending on traits
        RegenDown               = 71,   --Damage over time effects. Separate from normal Regen because of how they are displayed in game
        Stoneskin               = 72,   --Nullifies damage
        MinimumTpLock           = 73
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
using System;

namespace FFXIVClassic_Map_Server.actors.chara
{
    //These will need to be redone at some point. remember to update tables in db.
    //Consider using text_paramname sheet. that matches up with the stats on armor, but some things will need special handling
    //Also, 0-35 should probably match with up BattleTemp
    enum Modifier : UInt32
    {
        NAMEPLATE_SHOWN         = 0,
        TARGETABLE              = 1,
        NAMEPLATE_SHOWN2        = 2,
        //NAMEPLATE_SHOWN2 = 3,

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
        Defense                 = 18,   //Is there a magic defense stat? 19 maybe?
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
        MinimumHpLock           = 49,   // hp cannot fall below this value
        AttackType              = 50,   // slashing, piercing, etc
        BlockRate               = 51,
        Block                   = 52,
        CritRating              = 53,
        HasShield               = 54,   // Need this because shields are required for blocks. Could have used BlockRate or Block but BlockRate is provided by Gallant Sollerets and Block is provided by some buffs.
        HitCount                = 55,   // Amount of hits in an auto attack. Usually 1, 2 for h2h, 3 with spinning heel

        //Flat percent increases to these rates. Probably a better way to do this
        RawEvadeRate            = 56,   
        RawParryRate            = 57,
        RawBlockRate            = 58,
        RawResistRate           = 59,
        RawHitRate              = 60,
        RawCritRate             = 61,

        DamageTakenDown         = 62,   // Percent damage taken down
        StoreTP                 = 63,   //.1% extra tp per point. Lancer trait is 50 StoreTP
        PhysicalCritRate        = 64,   //CritRating but only for physical attacks. Increases chance of critting.
        PhysicalCritEvasion     = 65,   //Opposite of CritRating. Reduces chance of being crit by phyiscal attacks
        PhysicalCritAttack      = 66,   //Increases damage done by Physical Critical hits
        PhysicalCritResilience  = 67,   //Decreases damage taken by Physical Critical hits
        Parry                   = 68,   //Increases chance to parry
        MagicCritPotency        = 69,   //Increases 
        Regain                  = 70,   //TP regen, should be -90 out of combat, Invigorate sets to 100+ depending on traits
        RegenDown               = 71,   //Damage over time effects. Separate from normal Regen because of how they are displayed in game
        Stoneskin               = 72,   //Nullifies damage
        MinimumTpLock           = 73,   //Don't let TP fall below this, used in openings
        KnockbackImmune         = 74    //Immune to knockback effects when above 0
    }
}

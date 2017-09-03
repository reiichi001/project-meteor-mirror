using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara
{
    enum Modifier : UInt32
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
        MagicHeal              = 20, // is this needed? shouldnt it just be calc'd from mind
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
        ResistWater            = 31, // <3 u jorge
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
    }
}

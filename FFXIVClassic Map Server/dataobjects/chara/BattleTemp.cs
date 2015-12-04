using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class BattleTemp
    {
        //These are known property IDs
        public const uint NAMEPLATE_SHOWN = 0xFBFBCFB1;
        public const uint TARGETABLE = 0x2138FD71;

        public const uint STAT_STRENGTH = 0x647A29A8;
        public const uint STAT_VITALITY = 0x939E884A;
        public const uint STAT_DEXTERITY = 0x416571AC;
        public const uint STAT_INTELLIGENCE = 0x2DFBC13A;
        public const uint STAT_MIND = 0x0E704141;
        public const uint STAT_PIETY = 0x6CCAF8B3;

        public const uint STAT_ACCURACY = 0x91CD44E7;
        public const uint STAT_EVASION = 0x11B1B22D;
        public const uint STAT_ATTACK = 0xBA51C4E1;
        public const uint STAT_DEFENSE = 0x8CAE90DB;
        public const uint STAT_ATTACK_MAGIC_POTENCY = 0x1F3DACC5;
        public const uint STAT_HEALING_MAGIC_POTENCY = 0xA329599A;
        public const uint STAT_ENCHANCEMENT_MAGIC_POTENCY = 0xBA51C4E1;
        public const uint STAT_ENFEEBLING_MAGIC_POTENCY = 0xEB90BAAB;
        public const uint STAT_MAGIC_ACCURACY = 0xD57DC284;
        public const uint STAT_MAGIC_EVASION = 0x17AB37EF;

        public const uint RESISTANCE_FIRE = 0x79C7ECFF;
        public const uint RESISTANCE_ICE = 0xE17D8C7A;
        public const uint RESISTANCE_WIND = 0x204CF942;
        public const uint RESISTANCE_LIGHTNING = 0x1C2AEC73;
        public const uint RESISTANCE_EARTH = 0x5FC56D16;
        public const uint RESISTANCE_WATER = 0x64803E98;
        //End of properties

        public int[]    castGauge_speed = new int[2];
        public bool[]   timingCommandFlag = new bool[4];
        public ushort[] generalParameter = new ushort[32];
    }
}

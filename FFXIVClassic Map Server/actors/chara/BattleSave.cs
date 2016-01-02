using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class BattleSave
    {
        public float potencial;
        public int skillLevel;
        public int skillLevelCap;
        public int[] skillPoint;

        public int physicalExp;

        public bool[] negotiationFlag= new bool[2];
    }
}

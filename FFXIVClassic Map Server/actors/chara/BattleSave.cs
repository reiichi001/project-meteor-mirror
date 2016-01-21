using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class BattleSave
    {
        public float potencial = 6.6f;
        public short[] skillLevel = new short[52];
        public short[] skillLevelCap = new short[52];
        public short[] skillPoint = new short[52];

        public short physicalLevel;
        public int physicalExp;

        public bool[] negotiationFlag= new bool[2];
    }
}

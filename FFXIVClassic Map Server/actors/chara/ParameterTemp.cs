using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class ParameterTemp
    {
        public short tp = 0;

        public int targetInformation = 0;

        public int[] maxCommandRecastTime = new int[40];

        public float[] forceControl_float_forClientSelf = { 1.0f, 1.0f};
        public short[] forceControl_int16_forClientSelf = { -1, -1 };

        public int[] otherClassAbilityCount = new int[2];
        public int[] giftCount = new int[2];
    }
}

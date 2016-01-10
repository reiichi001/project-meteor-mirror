using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class ParameterSave
    {
        public short[] hp = new short[1];
        public short[] hpMax = new short[1];
        public short mp;
        public short mpMax;

        public byte[] state_mainSkill = new byte[4];
        public ushort state_mainSkillLevel;

        public int[] state_boostPointForSkill;

        public bool[] commandSlot_compatibility = new bool[40];
        public uint[] commandSlot_recastTime = new uint[40];

        public int[] giftCommandSlot_commandId;
    }
}

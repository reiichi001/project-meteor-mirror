using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class ParameterSave
    {
        public short[] hp = new short[8];
        public short[] hpMax = new short[8];
        public short mp;
        public short mpMax;

        public byte[] state_mainSkill = new byte[4];
        public short state_mainSkillLevel;

        public byte[] state_boostPointForSkill = new byte[4];
         
        public uint[] commandSlot_recastTime = new uint[40];
        public bool[] commandSlot_compatibility = new bool[40];

        public ushort[] giftCommandSlot_commandId = new ushort[10];

        public ushort[] constanceCommandSlot_commandId = new ushort[10];

        public byte abilityCostPoint_used;
        public byte abilityCostPoint_max;

        public byte giftCostPoint_used;
        public byte giftCostPoint_max;

        public byte constanceCostPoint_used;
        public byte constanceCostPoint_max;
    }
}

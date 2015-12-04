using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class ParameterSave
    {
        public int[] hp = new int[1];
        public int[] hpMax = new int[1];
        public int mp;
        public int mpMax;

        public int[] state_mainSkill = new int[4];
        public int state_mainSkillLevel;

        public int[] state_boostPointForSkill;

        public int[] commandSlot_compatibility;
        public int[] commandSlot_recastTime;

        public int[] giftCommandSlot_commandId;
    }
}

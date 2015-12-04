using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.database
{
    class DBStats
    {
        public int hp;
        public int hpMax;
        public int mp;
        public int mpMax;
        public ushort[] state_mainSkill;
        public int state_mainSkillLevel;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    class BattleTrait
    {
        public ushort id;
        public string name;
        public byte job;
        public byte level;
        public uint modifier;
        public int bonus;

        public BattleTrait(ushort id, string name, byte job, byte level, uint modifier, int bonus)
        {
            this.id = id;
            this.name = name;
            this.job = job;
            this.level = level;
            this.modifier = modifier;
            this.bonus = bonus;
        }
    }
}

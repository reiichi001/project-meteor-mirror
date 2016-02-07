using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.battle
{
    class BattleAction
    {
        public uint targetId;
        public ushort amount;
        public ushort worldMasterTextId;
        public uint effectId;
        public byte param;
        public byte unknown;
    }
}

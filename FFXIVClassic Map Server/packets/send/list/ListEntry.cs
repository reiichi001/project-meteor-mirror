using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListEntry
    {
        public uint actorId;
        public uint unknown1;
        public uint unknown2;
        public bool flag1;
        public bool isOnline;
        public string name;

        public ListEntry(uint actorId, uint unknown1, uint unknown2, bool flag1, bool isOnline, string name)
        {
            this.actorId = actorId;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.flag1 = flag1;
            this.isOnline = isOnline;
            this.name = name;
        }
    }
}

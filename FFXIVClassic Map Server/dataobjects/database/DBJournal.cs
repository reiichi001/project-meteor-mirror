using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.database
{
    class DBJournal
    {
        public uint id;
        public uint characterId;
        public uint index;
        public uint questId;
        public uint type;
        public bool abandoned;
        public bool completed;
    }
}
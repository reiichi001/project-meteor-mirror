using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class PlayerSearchResult
    {
        public string name;
        public string comment;

        public byte preferredClass;
        public byte clientLanguage;
        public byte initialTown;
        public byte status;
        public byte currentClass;
        public ushort currentZone;



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.dataobjects
{
    class DBWorld
    {
        public ushort id;
        public string address;
        public ushort port;
        public ushort listPosition;
        public ushort population;
        public string name;
        public bool   isActive;
        public string motd;
    }
}

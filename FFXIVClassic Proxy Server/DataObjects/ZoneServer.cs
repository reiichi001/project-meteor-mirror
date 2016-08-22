using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects
{
    class ZoneServer
    {
        public string zoneServerIp;
        public int zoneServerPort;
        public Socket zoneServerConnection;
    }
}

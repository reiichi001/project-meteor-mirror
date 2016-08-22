using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects
{
    class Session
    {
        public enum Channel {ZONE, CHAT};

        public readonly ulong sessionId;
        public readonly ClientConnection clientSocket;
        public readonly Channel type;
        public ZoneServer routing1, routing2;

        public Session(ulong sessionId, ClientConnection socket, Channel type)
        {
            this.sessionId = sessionId;
            this.clientSocket = socket;
            this.type = type;
        }

    }
}

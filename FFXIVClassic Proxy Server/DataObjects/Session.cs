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

        public readonly uint sessionId;
        public readonly ClientConnection clientConnection;
        public readonly Channel type;
        public ZoneServer routing1, routing2;

        public Session(ulong sessionId, ClientConnection connection, Channel type)
        {
            this.sessionId = sessionId;
            this.clientConnection = connection;
            this.type = type;
            connection.owner = this;
        }

    }
}

using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send
{
    class SessionBeginPacket
    {
        public const ushort OPCODE = 0x1000;
        public const uint PACKET_SIZE = 0x24;

        public static SubPacket BuildPacket(Session session)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            return new SubPacket(true, OPCODE, 0, session.sessionId, data);
        }
    }
}

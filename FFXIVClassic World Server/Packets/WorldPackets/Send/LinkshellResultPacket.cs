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
    class LinkshellResultPacket
    {
        public const ushort OPCODE = 0x1025;
        public const uint PACKET_SIZE = 0x24;

        public static SubPacket BuildPacket(Session session, int result)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)result);
                }
            }            

            return new SubPacket(true, OPCODE, session.sessionId, data);
        }
    }
}

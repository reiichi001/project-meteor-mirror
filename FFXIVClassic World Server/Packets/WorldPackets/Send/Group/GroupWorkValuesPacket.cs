using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send.Group
{
    class GroupWorkValuesPacket
    {
        public const ushort OPCODE = 0x1023;
        public const uint PACKET_SIZE = 0x80;

        public static SubPacket BuildPacket(Session session, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)groupId);
                    //Write data
                }
            }

            return new SubPacket(true, OPCODE, 0, session.sessionId, data);
        }
    }
}

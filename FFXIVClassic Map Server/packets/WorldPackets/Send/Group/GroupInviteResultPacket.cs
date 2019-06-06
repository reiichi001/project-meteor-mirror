using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class GroupInviteResultPacket
    {
        public const ushort OPCODE = 0x1023;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(Session session, uint groupType, uint result)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)groupType);
                    binWriter.Write((UInt32)result);
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }

    }
}

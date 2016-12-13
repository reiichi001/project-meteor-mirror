using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send.Group
{
    class GroupMemberListStartPacket
    {
        public const ushort OPCODE = 0x1020;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(Session session, int resultCode, ulong groupId, int numMembers)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32) resultCode);
                    binWriter.Write((UInt64) groupId);
                    binWriter.Write((UInt32) numMembers);
                }
            }

            return new SubPacket(true, OPCODE, 0, session.sessionId, data);
        }
    }
}

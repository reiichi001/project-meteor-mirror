using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send.Group
{
    class GroupMemberListPacket
    {
        public const ushort OPCODE = 0x1021;
        public const uint PACKET_SIZE = 0x2C;

        public static SubPacket BuildPacket(Session session, ulong groupId, uint numMembersInPacket)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)groupId);
                    binWriter.Write((UInt32)numMembersInPacket);

                    //Members
                }
            }

            return new SubPacket(true, OPCODE, 0, session.sessionId, data);
        }
    }
}

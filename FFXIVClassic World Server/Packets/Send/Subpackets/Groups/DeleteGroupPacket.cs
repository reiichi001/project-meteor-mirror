using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups
{
    class DeleteGroupPacket
    {
        public const ushort OPCODE = 0x0143;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint sessionId, Group group)
        {
            return buildPacket(sessionId, group.groupIndex);
        }

        public static SubPacket buildPacket(uint sessionId, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write control num ????
                    binWriter.Write((UInt64)3);

                    //Write Ids
                    binWriter.Write((UInt64)groupId);
                    binWriter.Write((UInt64)0);
                    binWriter.Write((UInt64)groupId);
                }
            }

            return new SubPacket(OPCODE, sessionId, data);
        }
    }
}

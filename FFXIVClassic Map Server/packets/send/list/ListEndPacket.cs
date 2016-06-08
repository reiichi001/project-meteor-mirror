using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListEndPacket
    {
        public const ushort OPCODE = 0x017E;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, ulong listId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write List Header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);
                    //Write List Info
                    binWriter.Write((UInt64)listId);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }

    }
}

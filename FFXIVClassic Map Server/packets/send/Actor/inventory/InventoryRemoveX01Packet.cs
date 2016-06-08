using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryRemoveX01Packet
    {
        public const ushort OPCODE = 0x0152;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, ushort slot)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)slot);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

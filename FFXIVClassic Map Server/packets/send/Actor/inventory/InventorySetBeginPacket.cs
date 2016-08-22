using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventorySetBeginPacket
    {
        public const ushort OPCODE = 0x0146;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorId, ushort size, ushort code)
        {
            return BuildPacket(playerActorId, playerActorId, size, code);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint targetActorId, ushort size, ushort code)
        {
            byte[] data = new byte[8];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt16)size);
                    binWriter.Write((UInt16)code);
                }                
            }

            return new SubPacket(OPCODE, sourceActorId, targetActorId, data);
        }

    }
}

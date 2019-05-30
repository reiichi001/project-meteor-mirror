using System;
using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetDalamudPacket
    {
        public const ushort OPCODE = 0x0010;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorId, sbyte dalamudLevel)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)dalamudLevel);
                }
            }

            return new SubPacket(OPCODE, playerActorId, data);
        }
    }
}

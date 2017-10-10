using FFXIVClassic.Common;
using System;

using FFXIVClassic.Common;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerItemStoragePacket
    {
        public const ushort OPCODE = 0x01A5;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket BuildPacket(uint sourceActorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x3F}); //All items enabled
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

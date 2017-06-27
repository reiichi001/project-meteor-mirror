using System.IO;

using FFXIVClassic.Common;
using System;
using System.Text;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class _0xD9Packet
    {
        public const ushort OPCODE = 0x00D9;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, string anim)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(anim), 0, Encoding.ASCII.GetByteCount(anim) >= 4 ? 4 : Encoding.ASCII.GetByteCount(anim));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

using FFXIVClassic.Common;
using System;
using System.IO;
using System.Text;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class _0x132Packet
    {
        public const ushort OPCODE = 0x132;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint sourceActorId, ushort number, string function)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)number);
                    binWriter.Write(Encoding.ASCII.GetBytes(function), 0, Encoding.ASCII.GetByteCount(function) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(function));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

using FFXIVClassic.Common;
using System;
using System.IO;
using System.Text;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class StartCountdownPacket
    {
        public const ushort OPCODE = 0xE5;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint sourceActorId, byte countdownLength, uint startTime, string message)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Byte)countdownLength);
                    binWriter.Seek(8, SeekOrigin.Begin);
                    binWriter.Write((UInt32)startTime);
                    binWriter.Seek(18, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(message), 0, Encoding.ASCII.GetByteCount(message) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(message));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

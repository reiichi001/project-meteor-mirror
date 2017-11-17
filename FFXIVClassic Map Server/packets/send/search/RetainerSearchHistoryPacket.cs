using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class RetainerSearchHistoryPacket
    {
        public const ushort OPCODE = 0x01DD;
        public const uint PACKET_SIZE = 0x120;

        public static SubPacket BuildPacket(uint sourceActorId, byte count, bool hasEnded)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(0x12, SeekOrigin.Begin);
                    binWriter.Write((UInt16)count);
                    binWriter.Write((Byte)(hasEnded ? 2 : 0));

                    for (int i = 0; i < count; i++)
                    {
                        binWriter.Seek(0x10 + (0x80 * i), SeekOrigin.Begin);
                        RetainerSearchHistoryResult result = null;
                        binWriter.Write((UInt32)result.timestamp);
                        binWriter.Write((UInt16)0);
                        binWriter.Write((UInt16)result.quanitiy);
                        binWriter.Write((UInt32)result.gilCostPerItem);
                        binWriter.Write((Byte)result.numStack);                       
                    }
                }
            }
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

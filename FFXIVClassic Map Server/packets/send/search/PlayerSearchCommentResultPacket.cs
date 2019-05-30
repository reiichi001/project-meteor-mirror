using System.IO;
using System.Text;

using FFXIVClassic.Common;
using System;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class PlayerSearchCommentResultPacket
    {
        public const ushort OPCODE = 0x01E0;
        public const uint PACKET_SIZE = 0x288;

        public static SubPacket BuildPacket(uint sourceActorId, uint searchSessionId, byte resultCode, PlayerSearchResult[] results, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            byte count = 0;

            for (int i = offset; i < results.Length; i++)
            {
                int size = 3 + (Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment));

                if (size >= 600)
                    break;

                count++;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = offset; i < count; i++)
                    {
                        binWriter.Write((UInt32)searchSessionId);
                        binWriter.Write((Byte)count);
                        binWriter.Seek(1, SeekOrigin.Current);
                        binWriter.Write((Byte)resultCode);
                        binWriter.Seek(4, SeekOrigin.Current);

                        binWriter.Write((Byte)i);
                        binWriter.Write((UInt16)(Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment)));
                        binWriter.Write(Encoding.ASCII.GetBytes(results[i].comment), 0, Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment));
                    }
                }
            }

            offset += count;

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

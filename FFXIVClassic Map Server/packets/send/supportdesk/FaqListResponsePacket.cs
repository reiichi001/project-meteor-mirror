using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class FaqListResponsePacket
    {
        public const ushort OPCODE = 0x01D0;
        public const uint PACKET_SIZE = 0x2B8;

        public static SubPacket buildPacket(uint playerActorID, string[] faqsTitles)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < faqsTitles.Length; i++)
                    {
                        binWriter.Seek(0x80 * i, SeekOrigin.Begin);
                        binWriter.Write(Encoding.ASCII.GetBytes(faqsTitles[i]), 0, Encoding.ASCII.GetByteCount(faqsTitles[i]) >= 0x80 ? 0x80 : Encoding.ASCII.GetByteCount(faqsTitles[i]));
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

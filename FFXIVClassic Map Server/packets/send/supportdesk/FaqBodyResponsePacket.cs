using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class FaqBodyResponsePacket
    {
        public const ushort OPCODE = 0x01D1;
        public const uint PACKET_SIZE = 0x587;

        public static SubPacket buildPacket(uint playerActorID, string faqsBodyText)
        {            
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x04;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                        binWriter.Seek(4, SeekOrigin.Begin);
                        binWriter.Write(Encoding.ASCII.GetBytes(faqsBodyText), 0, Encoding.ASCII.GetByteCount(faqsBodyText) >= maxBodySize ? maxBodySize : Encoding.ASCII.GetByteCount(faqsBodyText));                    
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

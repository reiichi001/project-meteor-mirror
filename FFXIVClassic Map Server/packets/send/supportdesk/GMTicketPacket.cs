using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class GMTicketPacket
    {
        public const ushort OPCODE = 0x01D4;
        public const uint PACKET_SIZE = 0x2B8;

        public static SubPacket buildPacket(uint playerActorID, string titleText, string bodyText)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(0x0, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(titleText), 0, Encoding.ASCII.GetByteCount(titleText) >= 0x80 ? 0x80 : Encoding.ASCII.GetByteCount(titleText));
                    binWriter.Seek(0x80, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(bodyText), 0, Encoding.ASCII.GetByteCount(bodyText) >= maxBodySize ? maxBodySize : Encoding.ASCII.GetByteCount(bodyText));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

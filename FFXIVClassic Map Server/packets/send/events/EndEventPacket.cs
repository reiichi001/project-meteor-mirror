using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class EndEventPacket
    {
        public const ushort OPCODE = 0x0131;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket buildPacket(uint playerActorID, uint eventOwnerActorID, string eventStarter)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)playerActorID);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((Byte)1);
                    binWriter.Write(Encoding.ASCII.GetBytes(eventStarter), 0, Encoding.ASCII.GetByteCount(eventStarter) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(eventStarter));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

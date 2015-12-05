using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class BlacklistAddedPacket
    {
        public const ushort OPCODE = 0x01C9;
        public const uint PACKET_SIZE = 0x048;

        public static SubPacket buildPacket(uint playerActorID, bool isSuccess, string nameToAdd)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((byte)(isSuccess ? 1 : 0));
                    binWriter.Write(Encoding.ASCII.GetBytes(nameToAdd), 0, Encoding.ASCII.GetByteCount(nameToAdd) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(nameToAdd));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

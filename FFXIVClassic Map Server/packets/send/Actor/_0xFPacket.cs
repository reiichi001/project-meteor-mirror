using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class _0xFPacket
    {
        public const ushort OPCODE = 0x000F;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    
                }
            }

            return new SubPacket(OPCODE, playerActorID, targetActorID, data);
        }
    }
}

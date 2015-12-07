using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.recruitment
{
    class StartRecruitingResponse
    {
        public const ushort OPCODE = 0x01C3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, bool success)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            data[0] = (byte)(success ? 0x1 : 0x0);

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

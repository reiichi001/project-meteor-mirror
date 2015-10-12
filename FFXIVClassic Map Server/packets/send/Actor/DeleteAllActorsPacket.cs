using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class DeleteAllActorsPacket
    {
        public const ushort OPCODE = 0x0007;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, new byte[8]);
        }
    }
}

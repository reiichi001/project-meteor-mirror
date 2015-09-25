using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class RemoveActorPacket
    {
        public const ushort OPCODE = 0x00CB;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint actorID)
        {
            return new SubPacket(OPCODE, playerActorID, actorID, new byte[8]);
        }

    }
}

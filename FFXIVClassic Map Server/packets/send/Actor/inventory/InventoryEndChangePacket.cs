using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryEndChangePacket
    {
        public const ushort OPCODE = 0x016E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint sourceActorId, uint targetActorId)
        {
            return new SubPacket(OPCODE, sourceActorId, targetActorId, new byte[8]);
        }

        public static SubPacket buildPacket(uint playerActorID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, new byte[8]);
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryBeginChangePacket
    {
        public const ushort OPCODE = 0x016D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint sourceActorId, uint targetActorId)
        {
            byte[] data = new byte[8];
            data[0] = 2;
            return new SubPacket(OPCODE, sourceActorId, targetActorId, data);
        }

        public static SubPacket buildPacket(uint playerActorID)
        {
            byte[] data = new byte[8];
            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListEndPacket
    {
        public const ushort OPCODE = 0x017E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, ulong listId)
        {
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(listId));
        }

    }
}

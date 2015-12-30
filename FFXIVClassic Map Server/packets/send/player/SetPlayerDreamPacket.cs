using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerDreamPacket
    {
        public const ushort OPCODE = 0x01A7;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint dreamID)
        {
            dreamID += 0x20E;
            return new SubPacket(OPCODE, playerActorID, playerActorID, BitConverter.GetBytes((uint)dreamID));
        }
    }
}

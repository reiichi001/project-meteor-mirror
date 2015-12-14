using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetHasGoobbuePacket
    {
        public const ushort OPCODE = 0x01A1;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, bool hasGoobbue)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(hasGoobbue ? 1 : 0);
            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

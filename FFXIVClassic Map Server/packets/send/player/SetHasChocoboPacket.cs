using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetHasChocoboPacket
    {
        public const ushort OPCODE = 0x0199;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, bool hasChocobo)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(hasChocobo ? 1 : 0);
            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

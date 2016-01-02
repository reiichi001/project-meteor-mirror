using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCurrentMountChocoboPacket
    {
        public const int CHOCOBO_NORMAL = 0;

        public const int CHOCOBO_LIMSA1 = 0x1;
        public const int CHOCOBO_LIMSA2 = 0x2;
        public const int CHOCOBO_LIMSA3 = 0x3;
        public const int CHOCOBO_LIMSA4 = 0x4;

        public const int CHOCOBO_GRIDANIA1 = 0x1F;
        public const int CHOCOBO_GRIDANIA2 = 0x20;
        public const int CHOCOBO_GRIDANIA3 = 0x21;
        public const int CHOCOBO_GRIDANIA4 = 0x22;

        public const int CHOCOBO_ULDAH1 = 0x3D;
        public const int CHOCOBO_ULDAH2 = 0x3E;
        public const int CHOCOBO_ULDAH3 = 0x3F;
        public const int CHOCOBO_ULDAH4 = 0x40;

        public const ushort OPCODE = 0x0197;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, int appearanceId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[5] = (byte)(appearanceId & 0xFF);
            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

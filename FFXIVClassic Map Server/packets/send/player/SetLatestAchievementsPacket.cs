using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetLatestAchievementsPacket
    {
        public const ushort OPCODE = 0x01A3;
        public const uint PACKET_SIZE = 0150;

        private byte[] mainstoryFlags = new byte[7];
        private byte[] classFlags = new byte[2*17];

        public static SubPacket buildPacket(uint playerActorID)
        {
            return null;
        }
    }
}

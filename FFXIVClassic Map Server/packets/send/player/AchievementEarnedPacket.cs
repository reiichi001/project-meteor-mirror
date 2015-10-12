using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class AchievementEarnedPacket
    {
        public const ushort OPCODE = 0x019E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint achievementID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, BitConverter.GetBytes((UInt64)achievementID));
        }
    }
}

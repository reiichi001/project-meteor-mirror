using System;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class AchievementEarnedPacket
    {
        public const ushort OPCODE = 0x019E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint achievementID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, BitConverter.GetBytes((UInt64)achievementID));
        }
    }
}

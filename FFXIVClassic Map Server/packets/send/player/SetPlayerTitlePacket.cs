using System;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerTitlePacket
    {
        public const ushort OPCODE = 0x019D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, uint titleID)
        {
            return new SubPacket(OPCODE, playerActorID, targetActorID, BitConverter.GetBytes((UInt64)titleID));
        }
    }
}

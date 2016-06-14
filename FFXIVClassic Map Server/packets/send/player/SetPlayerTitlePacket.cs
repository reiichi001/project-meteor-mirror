using System;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerTitlePacket
    {
        public const ushort OPCODE = 0x019D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, uint titleID)
        {
            return new SubPacket(OPCODE, playerActorID, tarGetActorID, BitConverter.GetBytes((UInt64)titleID));
        }
    }
}

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class LogoutPacket
    {
        public const ushort OPCODE = 0x000E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID)
        {
            return new SubPacket(OPCODE, playerActorID, new byte[8]);
        }
    }
}

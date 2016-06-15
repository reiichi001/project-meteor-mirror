namespace FFXIVClassic_Map_Server.packets.send
{
    class QuitPacket
    {
        public const ushort OPCODE = 0x0011;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, new byte[8]);
        }
    }
}

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class EndGMTicketPacket
    {
        public const ushort OPCODE = 0x01D6;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = 1;
            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

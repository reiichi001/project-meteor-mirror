namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventorySetEndPacket
    {

        public const ushort OPCODE = 0x0147;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorId)
        {
            return new SubPacket(OPCODE, playerActorId, playerActorId, new byte[8]);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint targetActorID)
        {
            return new SubPacket(OPCODE, sourceActorId, targetActorID, new byte[8]);
        }

    }
}

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryEndChangePacket
    {
        public const ushort OPCODE = 0x016E;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint targetActorId)
        {
            return new SubPacket(OPCODE, sourceActorId, targetActorId, new byte[8]);
        }

        public static SubPacket BuildPacket(uint playerActorID)
        {
            return new SubPacket(OPCODE, playerActorID, playerActorID, new byte[8]);
        }
    }
}

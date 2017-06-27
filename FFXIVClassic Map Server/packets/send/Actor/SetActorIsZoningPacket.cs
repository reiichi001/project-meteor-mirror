using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorIsZoningPacket
    {
        public const ushort OPCODE = 0x017B;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, bool isDimmed)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[0] = (byte)(isDimmed ? 1 : 0);
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

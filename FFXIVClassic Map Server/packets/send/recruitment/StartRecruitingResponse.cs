using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.recruitment
{
    class StartRecruitingResponse
    {
        public const ushort OPCODE = 0x01C3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, bool success)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            data[0] = (byte)(success ? 0x1 : 0x0);

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

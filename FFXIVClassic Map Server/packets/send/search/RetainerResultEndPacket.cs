using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class RetainerResultEndPacket
    {
        public const ushort OPCODE = 0x01DA;
        public const uint PACKET_SIZE = 0x038;

        public static SubPacket BuildPacket(uint sourceActorId, bool isSuccess)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[16] = (byte) (isSuccess ? 1 : 0);
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

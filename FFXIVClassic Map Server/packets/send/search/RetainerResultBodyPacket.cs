using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class RetainerResultBodyPacket
    {
        public const ushort OPCODE = 0x01DB;
        public const uint PACKET_SIZE = 0x028;

        public static SubPacket BuildPacket(uint sourceActorId, bool isSuccess, string nameToAdd)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];           
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

using FFXIVClassic.Common;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class BattleAction1Packet
    {
        public const ushort OPCODE = 0x0139;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket BuildPacket(uint sourceId, uint targetId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                }
            }

            return new SubPacket(OPCODE, sourceId, targetId, data);
        }
    }
}

using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send
{
    class WorldRequestZoneChangePacket
    {
        public const ushort OPCODE = 0x1002;
        public const uint PACKET_SIZE = 0x048;

        public static SubPacket BuildPacket(uint sessionId, uint destinationZoneId, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sessionId);
                    binWriter.Write((UInt32)destinationZoneId);
                    binWriter.Write((UInt16)spawnType);
                    binWriter.Write((Single)spawnX);
                    binWriter.Write((Single)spawnY);
                    binWriter.Write((Single)spawnZ);
                    binWriter.Write((Single)spawnRotation);
                }
            }

            return new SubPacket(OPCODE, sessionId, data);
        }
    }
}

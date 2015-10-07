using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorPositionPacket
    {
        public const ushort OPCODE = 0x00CE;
        public const uint PACKET_SIZE = 0x48;

        public const uint SPAWNTYPE_NORMAL = 1;
        public const uint SPAWNTYPE_WARP1  = 2;
        public const uint SPAWNTYPE_WARP2  = 3;

        public const float INNPOS_X     = 157.550003f;
        public const float INNPOS_Y     = 000.000000f;
        public const float INNPOS_Z     = 165.050003f;
        public const float INNPOS_ROT   =  -1.530000f;

        public static SubPacket buildPacket(uint playerActorID, uint actorID, float x, float y, float z, float rotation)
        {
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)0);
                    binWriter.Write((Int32)0);
                    binWriter.Write((Single)x);
                    binWriter.Write((Single)y);
                    binWriter.Write((Single)z);
                    binWriter.Write((Single)rotation);
                }
            }

            return new SubPacket(OPCODE, playerActorID, actorID, data);
        }

    }
}

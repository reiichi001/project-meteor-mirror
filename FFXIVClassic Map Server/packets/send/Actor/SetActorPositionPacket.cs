using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorPositionPacket
    {
        public const ushort OPCODE = 0x00CE;
        public const uint PACKET_SIZE = 0x48;

        public const ushort SPAWNTYPE_FADEIN = 0;
        public const ushort SPAWNTYPE_PLAYERWAKE = 1;
        public const ushort SPAWNTYPE_WARP_DUTY = 2;
        public const ushort SPAWNTYPE_WARP2 = 3;
        public const ushort SPAWNTYPE_WARP3 = 4;
        public const ushort SPAWNTYPE_WARP_YELLOW = 5;
        public const ushort SPAWNTYPE_WARP_DUTY2 = 6;
        public const ushort SPAWNTYPE_WARP_LIGHT = 7;
        
        public static SubPacket BuildPacket(uint sourceActorID, uint targetActorID, uint actorId, float x, float y, float z, float rotation, ushort spawnType, bool isZoningPlayer)
        {
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)0);                    
                    binWriter.Write((Int32)actorId);
                    binWriter.Write((Single)x);
                    binWriter.Write((Single)y);
                    binWriter.Write((Single)z);
                    binWriter.Write((Single)rotation);

                    binWriter.BaseStream.Seek(0x24, SeekOrigin.Begin);

                    binWriter.Write((UInt16)spawnType);
                    binWriter.Write((UInt16)(isZoningPlayer ? 1 : 0));
                }
            }

            return new SubPacket(OPCODE, sourceActorID, targetActorID, data);
        }

    }
}

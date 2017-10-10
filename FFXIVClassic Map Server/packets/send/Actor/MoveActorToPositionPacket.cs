using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class MoveActorToPositionPacket
    {
        public const ushort OPCODE = 0x00CF;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket BuildPacket(uint sourceActorId, float x, float y, float z, float rot, ushort moveState)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.BaseStream.Seek(0x8, SeekOrigin.Begin);
                    binWriter.Write((Single)x);
                    binWriter.Write((Single)y);
                    binWriter.Write((Single)z);
                    binWriter.Write((Single)rot);
                    binWriter.Write((ushort)moveState);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, data);
            return packet;
        }

    }
}

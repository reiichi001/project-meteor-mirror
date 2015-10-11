using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class MoveActorToPositionPacket
    {
        public const ushort OPCODE = 0x00CF;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, float x, float y, float z, float rot, ushort moveState)
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

            SubPacket packet = new SubPacket(OPCODE, playerActorID, targetActorID, data);
            return packet;
        }

    }
}

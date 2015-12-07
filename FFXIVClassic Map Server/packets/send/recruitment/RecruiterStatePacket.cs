using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.recruitment
{
    class RecruiterStatePacket
    {
        public const ushort OPCODE = 0x01C5;
        public const uint PACKET_SIZE = 0x038;

        public static SubPacket buildPacket(uint playerActorID, bool isRecruiting, bool isRecruiter, long recruitmentId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)recruitmentId);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((byte)(isRecruiter ? 1 : 0));
                    binWriter.Write((byte)(isRecruiting ? 1 : 0));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

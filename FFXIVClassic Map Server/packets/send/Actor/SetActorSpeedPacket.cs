using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorSpeedPacket
    {
        public const ushort OPCODE = 0x00D0;
        public const uint PACKET_SIZE = 0xA8;

        public const float DEFAULT_STOP = 0.0f;
        public const float DEFAULT_WALK = 2.0f;
        public const float DEFAULT_RUN = 5.0f;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)DEFAULT_STOP);
                    binWriter.Write((UInt32)0);

                    binWriter.Write((Single)DEFAULT_WALK);
                    binWriter.Write((UInt32)1);

                    binWriter.Write((Single)DEFAULT_RUN);
                    binWriter.Write((UInt32)2);

                    binWriter.Write((Single)DEFAULT_RUN);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)5);
                }
            }

            return new SubPacket(OPCODE, playerActorID, targetActorID, data);
        }

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, float stopSpeed, float walkSpeed, float runSpeed)
        {               
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)stopSpeed);
                    binWriter.Write((UInt32)0);

                    binWriter.Write((Single)walkSpeed);
                    binWriter.Write((UInt32)1);

                    binWriter.Write((Single)runSpeed);
                    binWriter.Write((UInt32)2);
                    
                    binWriter.Write((Single)runSpeed);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)5);
                }
            }

            return new SubPacket(OPCODE, playerActorID, targetActorID, data);
        }
    }
}

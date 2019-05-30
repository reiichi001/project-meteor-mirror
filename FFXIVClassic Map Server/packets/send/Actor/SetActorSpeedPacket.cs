using FFXIVClassic.Common;
using System;
using System.IO;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorSpeedPacket
    {
        public const ushort OPCODE = 0x00D0;
        public const uint PACKET_SIZE = 0xA8;

        public const float DEFAULT_STOP = 0.0f;
        public const float DEFAULT_WALK = 2.0f;
        public const float DEFAULT_RUN = 5.0f;
        public const float DEFAULT_ACTIVE = 5.0f;

        public static SubPacket BuildPacket(uint sourceActorId)
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

                    binWriter.Write((Single)DEFAULT_ACTIVE);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)4);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, float stopSpeed, float walkSpeed, float runSpeed, float activeSpeed)
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
                    
                    binWriter.Write((Single)activeSpeed);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)4);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

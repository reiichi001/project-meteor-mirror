using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorSubStatPacket
    {
        public const ushort OPCODE = 0x144;
        public const uint PACKET_SIZE = 0x28;

        enum SubStat : int
        {
            Breakage          = 0x00, // (index goes high to low, bitflags)
            Chant             = 0x01, // [Nibbles: left / right hand = value]) (AKA SubStatObject)
            Guard             = 0x02, // [left / right hand = true] 0,1,2,3) ||| High byte also defines how many bools to use as flags for byte 0x4. 
            Waste             = 0x03, // (High Nibble)
            Mode              = 0x04, // ???
            Unknown           = 0x05, // ???
            SubStatMotionPack = 0x06,
            Unknown2          = 0x07,
        }
        public static SubPacket BuildPacket(uint sourceActorId, byte breakage, int leftChant, int rightChant, int guard, int wasteStat, int statMode, uint idleAnimationId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                   binWriter.Write((byte)breakage);
                   binWriter.Write((byte)(((leftChant & 0xF) << 8) | (rightChant & 0xF)));
                   binWriter.Write((byte)(guard & 0xF));
                   binWriter.Write((byte)((wasteStat & 0xF) << 8));
                   binWriter.Write((byte)(statMode & 0xF));
                   binWriter.Write((byte)0);
                   binWriter.Write((UInt16)(idleAnimationId&0xFFFF));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

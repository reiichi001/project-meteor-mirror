using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorSubStatPacket
    {        
        public const ushort OPCODE = 0x144;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint targetID, byte breakage, int leftChant, int rightChant, int guard, int wasteStat, int statMode, uint idleAnimationId)
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

            return new SubPacket(OPCODE, playerActorID, targetID, data);
        }
    }
}

using System;
using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetSpecialEventWorkPacket
    {
        public const ushort OPCODE = 0x0196;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt16)0x00);
                    binWriter.Write((UInt16)18); //Just set it to Bomb Festival to unlock Bombdance
                }
            }

            return new SubPacket(OPCODE, playerActorID, targetActorID, data);
        }
    }
}

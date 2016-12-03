using System;
using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetLatestAchievementsPacket
    {
        public const ushort OPCODE = 0x019B;
        public const uint PACKET_SIZE = 0x40;
       
        public static SubPacket BuildPacket(uint playerActorID, uint[] latestAchievementIDs)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        //Had less than 5
                        if (i > latestAchievementIDs.Length)
                            break;
                        binWriter.Write((UInt32)latestAchievementIDs[i]);
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

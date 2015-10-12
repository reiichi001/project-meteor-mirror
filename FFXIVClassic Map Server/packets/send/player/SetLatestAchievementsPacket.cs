using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetLatestAchievementsPacket
    {
        public const ushort OPCODE = 0x019B;
        public const uint PACKET_SIZE = 0x40;
       
        public static SubPacket buildPacket(uint playerActorID, uint[] latestAchievementIDs)
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

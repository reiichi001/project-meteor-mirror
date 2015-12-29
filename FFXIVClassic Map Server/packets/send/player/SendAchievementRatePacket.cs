using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SendAchievementRatePacket
    {
        public const ushort OPCODE = 0x019F;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket buildPacket(uint playerActorID, uint achievementId, uint progressCount, uint progressFlags)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)achievementId);
                    binWriter.Write((UInt32)progressCount);
                    binWriter.Write((UInt32)progressFlags);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

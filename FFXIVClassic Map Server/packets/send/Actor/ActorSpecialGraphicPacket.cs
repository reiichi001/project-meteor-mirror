using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorQuestGraphicPacket
    {
        public const int NONE            = 0x0;
        public const int QUEST           = 0x2;
        public const int NOGRAPHIC       = 0x3;
        public const int QUEST_IMPORTANT = 0x4;

        public const ushort OPCODE = 0x00E3;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, int iconCode)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)iconCode);
                }
            }

            return new SubPacket(OPCODE, targetActorID, playerActorID, data);
        }
    }
}

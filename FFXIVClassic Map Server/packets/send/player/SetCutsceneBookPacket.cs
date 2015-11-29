using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCutsceneBookPacket
    {
        public const int CATEGORY_BATTLE = 000;
        public const int CATEGORY_CHARACTER = 050;
        public const int CATEGORY_CURRENCY = 200;
        public const int CATEGORY_ITEMS = 250;
        public const int CATEGORY_SYNTHESIS = 300;
        public const int CATEGORY_GATHERING = 400;
        public const int CATEGORY_MATERIA = 550;
        public const int CATEGORY_QUESTS = 600;
        public const int CATEGORY_SEASONAL_EVENTS = 700;
        public const int CATEGORY_DUNGEONS = 750;
        public const int CATEGORY_EXPLORATION = 800;
        public const int CATEGORY_GRAND_COMPANY = 820;

        public const ushort OPCODE = 0x01A3;
        public const uint PACKET_SIZE = 0150;

        public bool[] cutsceneFlags = new bool[2432];

        public SubPacket buildPacket(uint playerActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    byte[] binStream = Utils.ConvertBoolArrayToBinaryStream(cutsceneFlags);
                    if (binStream.Length <= PACKET_SIZE - 0x20)
                        binWriter.Write(binStream);
                    else
                        Log.error("Failed making SetCutsceneBook packet. Bin Stream was too big!");
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }

    }
}

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
    class SetCompletedAchievementsPacket
    {     
        //Achievenments are +1 and up, except for Quests and GCs which is +2
        public const int CATEGORY_BATTLE =             000;
        public const int CATEGORY_CHARACTER =          050;
        public const int CATEGORY_CURRENCY =           200;
        public const int CATEGORY_ITEMS =              250;
        public const int CATEGORY_SYNTHESIS =          300;
        public const int CATEGORY_GATHERING =          400;
        public const int CATEGORY_MATERIA =            550;
        public const int CATEGORY_QUESTS =             600;
        public const int CATEGORY_SEASONAL_EVENTS =    700;
        public const int CATEGORY_DUNGEONS =           750;
        public const int CATEGORY_EXPLORATION =        800;
        public const int CATEGORY_GRAND_COMPANY =      820;
        
        public const ushort OPCODE = 0x019A;
        public const uint PACKET_SIZE = 0xA0;

        public bool[] achievementFlags = new bool[1024];

        public SubPacket buildPacket(uint playerActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    byte[] binStream = Utils.ConvertBoolArrayToBinaryStream(achievementFlags);
                    if (binStream.Length <= PACKET_SIZE - 0x20)
                        binWriter.Write(binStream);
                    else                    
                        Log.error("Failed making SetCompletedAchievements packet. Bin Stream was too big!");                    
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);            
        }
        
    }
}
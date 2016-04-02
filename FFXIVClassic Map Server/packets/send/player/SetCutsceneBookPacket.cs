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
        //Main Story
        public const int CATEGORY_MAINSTORY =  072;
        public const int CATEGORY_SIDEQUESTS1 = 672;
        public const int CATEGORY_SIDEQUESTS2 = 892;
        public const int CATEGORY_INSTANCED = 888;

        //Other        
        public const int OTHER_UGHAMARO =      005;
        public const int OTHER_NATALAN =       006;
        public const int OTHER_Z =             007;
        public const int OTHER_CASTRUMNOVUM =  008;
        public const int OTHER_NIGHTMARE =     014;

        //Class Cutscenes
        public const int CATEGORY_PUG_QUESTS = 128;
        public const int CATEGORY_GLA_QUESTS = 148;
        public const int CATEGORY_MRD_QUESTS = 168;
        public const int CATEGORY_ARC_QUESTS = 188;
        public const int CATEGORY_LNC_QUESTS = 208;
        public const int CATEGORY_THM_QUESTS = 228;
        public const int CATEGORY_CNJ_QUESTS = 248;

        //DoH/DoL Cutscenes
        public const int CATEGORY_CRP_QUESTS = 268;
        public const int CATEGORY_BSM_QUESTS = 288;
        public const int CATEGORY_GSM_QUESTS = 308;
        public const int CATEGORY_LTW_QUESTS = 328;
        public const int CATEGORY_WVR_QUESTS = 348;
        public const int CATEGORY_ALC_QUESTS = 368;
        public const int CATEGORY_CUL_QUESTS = 388;
        public const int CATEGORY_MIN_QUESTS = 408;
        public const int CATEGORY_BTN_QUESTS = 428;
        public const int CATEGORY_FSH_QUESTS = 448;
        
        //Job Cutscenes
        public const int CATEGORY_WAR_QUESTS = 1272;
        public const int CATEGORY_MNK_QUESTS = 1292;
        public const int CATEGORY_WHM_QUESTS = 1312;
        public const int CATEGORY_BLM_QUESTS = 1332;
        public const int CATEGORY_PLD_QUESTS = 1352;
        public const int CATEGORY_BRD_QUESTS = 1372;
        public const int CATEGORY_DRG_QUESTS = 1392;

        //GC Cutscenes
        public const int CATEGORY_MAELSTROM =  1472;
        public const int CATEGORY_ADDERS =     1672;
        public const int CATEGORY_FLAMES =     1872;        

        public const ushort OPCODE = 0x01A3;
        public const uint PACKET_SIZE = 0x150;

        public bool[] cutsceneFlags = new bool[2048];

        public SubPacket buildPacket(uint playerActorID, string sNpcName, short sNpcActorIdOffset, byte sNpcSkin, byte sNpcPersonality)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    byte[] binStream = Utils.ConvertBoolArrayToBinaryStream(cutsceneFlags);
                    
                    //Temp Path Companion SNPC Stuff
                    binWriter.Seek(0x01 ,SeekOrigin.Begin);
                    binWriter.Write((Int16)2);
                    binWriter.Write((Byte)0);
                    binWriter.Write((Int16)sNpcActorIdOffset);
                    binWriter.Write((Byte)sNpcSkin);
                    binWriter.Write((Byte)sNpcPersonality);

                    if (binStream.Length <= PACKET_SIZE - 0x20)
                        binWriter.Write(binStream);
                    else
                        Log.error("Failed making SetCutsceneBook packet. Bin Stream was too big!");

                    binWriter.Seek(0x109, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(sNpcName), 0, Encoding.ASCII.GetByteCount(sNpcName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(sNpcName));

                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }

    }
}

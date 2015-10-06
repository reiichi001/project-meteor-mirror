using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorAppearancePacket
    {
        public const ushort OPCODE = 0x00D6;
        public const uint PACKET_SIZE = 0x128;
        
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int WEAPON1 = 5;
        public const int WEAPON2 = 6;
        public const int WEAPON3 = 7;
        public const int HEADGEAR = 8;
        public const int BODYGEAR = 9;
        public const int LEGSGEAR = 10;
        public const int HANDSGEAR = 11;
        public const int FEETGEAR = 12;
        public const int WAISTGEAR = 13;
        public const int UNKNOWN1 = 14;
        public const int R_EAR = 15;
        public const int L_EAR = 16;
        public const int UNKNOWN2 = 17;
        public const int UNKNOWN3 = 18;
        public const int R_FINGER = 19;
        public const int L_FINGER = 20;

        public uint modelID;
        public uint[] appearanceIDs;

        public SetActorAppearancePacket(uint modelID)
        {
            this.modelID = modelID;
            appearanceIDs = new uint[0x1D];
        }

        public SetActorAppearancePacket(uint modelID, uint[] appearanceTable)
        {
            this.modelID = modelID;
            appearanceIDs = appearanceTable;
        }

        public SubPacket buildPacket(uint playerActorID, uint actorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((uint)modelID); 
                    for (int i = 0; i <= 0x1A; i++)
                    {
                        binWriter.Write((uint)i);
                        binWriter.Write((uint)appearanceIDs[i]);
                    }
                    binWriter.Write((uint) 0x1B);
                    binWriter.Seek(0x20, SeekOrigin.Current);
                    binWriter.Write((uint) 0x1C);
                    binWriter.Write((uint) 0x00);
                }

            }

            SubPacket packet = new SubPacket(OPCODE, playerActorID, actorID, data);
            return packet;
        }       

    }
}

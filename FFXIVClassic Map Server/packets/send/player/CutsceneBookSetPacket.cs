using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class CutsceneBookSetPacket
    {
        public const ushort OPCODE = 0x01A3;
        public const uint PACKET_SIZE = 0150;

        private byte[] mainstoryFlags = new byte[7];
        private byte[] classFlags = new byte[2*17];
      
        public static SubPacket buildPacket(uint playerActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            byte currentOut = 0;
            int byteIndex = 0;
            int currentBit = 0;

            //Main Scenario
            for (int i = 0; i < 60; i++)
            {
                currentOut = (byte) (1|(currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //Classes
            for (int i = 0; i < 340; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //GAP
            for (int i = 0; i < 60; i++)
            {                
                currentBit++;
                if (currentBit >= 8)
                {
                    currentBit = 0;
                    byteIndex++;
                }
            }

            //Side Quests
            for (int i = 0; i < 372; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //GAP
            for (int i = 0; i < 228; i++)
            {
                currentBit++;
                if (currentBit >= 8)
                {
                    currentBit = 0;
                    byteIndex++;
                }
            }

            //Jobs
            for (int i = 0; i < 140; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //GAP
            for (int i = 0; i < 61; i++)
            {
                currentBit++;
                if (currentBit >= 8)
                {
                    currentBit = 0;
                    byteIndex++;
                }
            }

            //Maelstrom
            for (int i = 0; i < 200; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //Adders
            for (int i = 0; i < 200; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            //Flames
            for (int i = 0; i < 200; i++)
            {
                currentOut = (byte)(1 | (currentOut << currentBit));
                currentBit++;

                if (currentBit >= 8)
                {
                    currentBit = 0;
                    data[byteIndex] = currentOut;
                    byteIndex++;
                    currentOut = 0;
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

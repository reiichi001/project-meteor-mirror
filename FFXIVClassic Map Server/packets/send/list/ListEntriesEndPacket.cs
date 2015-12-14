﻿using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListEntriesEndPacket
    {
        public const ushort OPCODE = 0x017F;
        public const uint PACKET_SIZE = 0x1B8;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, uint id, uint numInPacket)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write List Header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)id);
                    //Write Entries
                    uint max = 8;
                    if (numInPacket < 8)
                        max = numInPacket;                    
                    for (int i = 0; i < max; i++)
                    {

                    }
                    //Write Count
                    binWriter.Seek(0x30 * 8, SeekOrigin.Begin);
                    binWriter.Write(max);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

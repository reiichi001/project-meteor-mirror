﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorStatusPacket
    {
        public const ushort OPCODE = 0x0177;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, ushort index, ushort statusCode)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
          
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)index);
                    binWriter.Write((UInt16)statusCode);
                }
            }

            return new SubPacket(OPCODE, playerActorID, tarGetActorID, data);            
        }
    }
}

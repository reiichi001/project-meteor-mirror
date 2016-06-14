﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class _0x196Packet
    {
        public const ushort OPCODE = 0x0196;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(0xE, SeekOrigin.Begin);
                    binWriter.Write((Byte)0x01);
                }
            }

            return new SubPacket(OPCODE, playerActorID, tarGetActorID, data);
        }
    }
}

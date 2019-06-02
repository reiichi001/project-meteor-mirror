﻿using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class LinkedItemListX01Packet
    {
        public const ushort OPCODE = 0x014D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, ushort position, uint linkedItem)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)position);
                    binWriter.Write((UInt32)linkedItem);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }

        public static SubPacket BuildPacket(uint playerActorID, ushort position, ushort itemSlot, ushort itemPackageCode)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)position);
                    binWriter.Write((UInt16)itemSlot);
                    binWriter.Write((UInt16)itemPackageCode);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

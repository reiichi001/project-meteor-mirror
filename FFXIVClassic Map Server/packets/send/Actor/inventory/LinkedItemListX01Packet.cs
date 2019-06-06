using System;
using System.IO;

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class LinkedItemListX01Packet
    {
        public const ushort OPCODE = 0x014D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, ushort position, InventoryItem linkedItem)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)position);
                    binWriter.Write((UInt16)linkedItem.slot);
                    binWriter.Write((UInt16)linkedItem.itemPackage);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }        
    }
}

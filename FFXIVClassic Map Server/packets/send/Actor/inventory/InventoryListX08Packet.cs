using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX08Packet
    {
        public const ushort OPCODE = 0x0149;
        public const uint PACKET_SIZE = 0x3A8;
        
        public static SubPacket BuildPacket(uint sourceActorId, List<InventoryItem> items, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (items.Count - listOffset <= 8)
                        max = items.Count - listOffset;
                    else
                        max = 8;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write(items[listOffset].ToPacketBytes());
                        listOffset++;
                    }

                    binWriter.Seek(0x380, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryRemoveX08Packet
    {
        public const ushort OPCODE = 0x0153;
        public const uint PACKET_SIZE = 0x38;
        public static SubPacket BuildPacket(uint playerActorID, List<ushort> slots, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (slots.Count - listOffset <= 8)
                        max = slots.Count - listOffset;
                    else
                        max = 8;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt16)slots[listOffset]);
                        listOffset++;
                    }

                    binWriter.Seek(0x10, SeekOrigin.Begin);
                    binWriter.Write((Byte)max);
                }
            }
            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

using FFXIVClassic_Map_Server.dataobjects;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryItemEndPacket
    {

        public const ushort OPCODE = 0x0149;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket BuildPacket(uint playerActorID, List<InventoryItem> items, ref int listOffset)
        {
            byte[] data;

            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = listOffset; i < items.Count; i++)
                    {
                        binWriter.Write(items[i].ToPacketBytes());
                        listOffset++;
                    }
                }

                data = mem.GetBuffer();
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }


    }
}

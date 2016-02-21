using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventoryItemPacket
    {

        public const ushort OPCODE = 0x014A;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket buildPacket(uint playerActorID, List<InventoryItem> items, ref int listOffset)
        {
            byte[] data;

            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = listOffset; i < items.Count; i++)
                    {
                        binWriter.Write(items[i].toPacketBytes());
                        listOffset++;
                    }
                }

                data = mem.GetBuffer();
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }


    }
}

using System.IO;
using System.Text;
using System;
using FFXIVClassic.Common;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class ItemSearchResultsBodyPacket
    {
        public const ushort OPCODE = 0x01D8;
        public const uint PACKET_SIZE = 0x228;

        public static SubPacket BuildPacket(uint sourceActorId, List<ItemSearchResult> itemSearchResult, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            int max;
            if (itemSearchResult.Count - listOffset <= 64)
                max = itemSearchResult.Count - listOffset;
            else
                max = 64;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)max);

                    foreach (ItemSearchResult item in itemSearchResult)
                        binWriter.Write((UInt32)item.itemId);

                    binWriter.Seek(0x104, SeekOrigin.Begin);

                    foreach (ItemSearchResult item in itemSearchResult)
                        binWriter.Write((UInt32)item.numItems);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

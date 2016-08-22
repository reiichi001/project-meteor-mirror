using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class EquipmentListX08Packet
    {
        public const ushort OPCODE = 0x14E;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket BuildPacket(uint playerActorId, InventoryItem[] equipment, List<ushort> slotsToUpdate, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (slotsToUpdate.Count - listOffset <= 8)
                        max = slotsToUpdate.Count - listOffset;
                    else
                        max = 8;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt16)slotsToUpdate[i]);
                        binWriter.Write((UInt32)equipment[slotsToUpdate[i]].slot);
                        listOffset++;
                    }

                    binWriter.Seek(0x30, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max);
                }
            }

            return new SubPacket(OPCODE, playerActorId, playerActorId, data);
        }

    }
}

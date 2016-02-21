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
    class EquipmentListX64Packet
    {
        public const ushort OPCODE = 0x151;
        public const uint PACKET_SIZE = 0x194;

        public static SubPacket buildPacket(uint playerActorId, InventoryItem[] equipment, List<ushort> slotsToUpdate, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (slotsToUpdate.Count - listOffset <= 64)
                        max = slotsToUpdate.Count - listOffset;
                    else
                        max = 64;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt16)slotsToUpdate[i]);
                        binWriter.Write((UInt32)equipment[slotsToUpdate[i]].slot);
                        listOffset++;
                    }

                }
            }

            return new SubPacket(OPCODE, playerActorId, playerActorId, data);
        }

    }
}

﻿using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class LinkedItemListX16Packet
    {
        public const ushort OPCODE = 0x14F;
        public const uint PACKET_SIZE = 0x80;

        public static SubPacket BuildPacket(uint playerActorId, uint[] linkedItemList, List<ushort> slotsToUpdate, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (slotsToUpdate.Count - listOffset <= 16)
                        max = slotsToUpdate.Count - listOffset;
                    else
                        max = 16;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt16)slotsToUpdate[i]); //LinkedItemPackageSlot
                        binWriter.Write((UInt32)linkedItemList[slotsToUpdate[i]]); //ItemPackage Slot + ItemPackage Code    
                        listOffset++;
                    }

                }
            }

            return new SubPacket(OPCODE, playerActorId, data);
        }

    }
}
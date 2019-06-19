/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class SetInitialEquipmentPacket
    {
        public const ushort OPCODE = 0x014E;
        public const uint PACKET_SIZE = 0x58;

        public const uint UNEQUIPPED = 0xFFFFFFFF;

        public const int SLOT_MAINHAND			    = 0x00;
		public const int SLOT_OFFHAND			    = 0x01;
		public const int SLOT_THROWINGWEAPON		= 0x04;
		public const int SLOT_PACK				    = 0x05;
		public const int SLOT_POUCH				    = 0x06;
		public const int SLOT_HEAD				    = 0x08;
		public const int SLOT_UNDERSHIRT			= 0x09;
		public const int SLOT_BODY				    = 0x0A;
		public const int SLOT_UNDERGARMENT		    = 0x0B;
		public const int SLOT_LEGS				    = 0x0C;
		public const int SLOT_HANDS				    = 0x0D;
		public const int SLOT_BOOTS				    = 0x0E;
		public const int SLOT_WAIST				    = 0x0F;
		public const int SLOT_NECK				    = 0x10;
		public const int SLOT_EARS				    = 0x11;
		public const int SLOT_WRISTS				= 0x13;
		public const int SLOT_RIGHTFINGER		    = 0x15;
		public const int SLOT_LEFTFINGER			= 0x16;

        private uint[] equipment = new uint[0x17];

        public SetInitialEquipmentPacket()
        {
            for (int i = 0; i < equipment.Length; i++)
                equipment[i] = UNEQUIPPED; //We will use this as "Unequipped"
        }

        public void SetItem(int slot, uint itemSlot)
        {
            if (slot >= equipment.Length)
                return;

            equipment[slot] = itemSlot;
        }

        public List<SubPacket> BuildPackets(uint playerActorID)
        {            
            List<SubPacket> packets = new List<SubPacket>();
            int packetCount = 0;
            int runningCount = 0;
            byte[] data = new byte[PACKET_SIZE - 0x20];
            MemoryStream mem = new MemoryStream(data);
            BinaryWriter binWriter = new BinaryWriter(mem);

            for (int i = 0; i < equipment.Length; i++)
            {
                if (equipment[i] == UNEQUIPPED)
                    continue;

                binWriter.Write((UInt16)i);
                binWriter.Write((UInt32)equipment[i]);

                packetCount++;
                runningCount++;

                //Create another packet
                if (runningCount >= 8)
                {
                    packetCount = 0;
                    binWriter.Write((UInt32)8);
                    binWriter.Dispose();
                    mem.Dispose();
                    packets.Add(new SubPacket(OPCODE, playerActorID, playerActorID, data));
                    data = new byte[PACKET_SIZE - 0x20];
                    mem = new MemoryStream(data);
                    binWriter = new BinaryWriter(mem);
                }
            }

            //Create any leftover subpacket
            binWriter.BaseStream.Seek(0x30, SeekOrigin.Begin);
            binWriter.Write((UInt32)packetCount);
            binWriter.Dispose();
            mem.Dispose();
            packets.Add(new SubPacket(OPCODE, playerActorID, playerActorID, data));
                   
            return packets;
        }

    }
}

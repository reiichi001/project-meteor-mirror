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

using Meteor.Map.dataobjects;
using System.Collections.Generic;
using System.IO;

using Meteor.Common;

namespace  Meteor.Map.packets.send.actor.inventory
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

            return new SubPacket(OPCODE, playerActorID, data);
        }


    }
}

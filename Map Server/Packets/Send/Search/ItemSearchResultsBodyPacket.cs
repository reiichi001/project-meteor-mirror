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

using System.IO;
using System;
using Meteor.Common;
using System.Collections.Generic;

namespace Meteor.Map.packets.send.search
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

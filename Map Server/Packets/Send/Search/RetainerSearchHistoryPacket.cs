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

using Meteor.Common;
using System;
using System.IO;

namespace Meteor.Map.packets.send.search
{
    class RetainerSearchHistoryPacket
    {
        public const ushort OPCODE = 0x01DD;
        public const uint PACKET_SIZE = 0x120;

        public static SubPacket BuildPacket(uint sourceActorId, byte count, bool hasEnded)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(0x12, SeekOrigin.Begin);
                    binWriter.Write((UInt16)count);
                    binWriter.Write((Byte)(hasEnded ? 2 : 0));

                    for (int i = 0; i < count; i++)
                    {
                        binWriter.Seek(0x10 + (0x80 * i), SeekOrigin.Begin);
                        RetainerSearchHistoryResult result = null;
                        binWriter.Write((UInt32)result.timestamp);
                        binWriter.Write((UInt16)0);
                        binWriter.Write((UInt16)result.quanitiy);
                        binWriter.Write((UInt32)result.gilCostPerItem);
                        binWriter.Write((Byte)result.numStack);                       
                    }
                }
            }
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

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
using Meteor.Map.actors.group;
using System;
using System.IO;
using System.Text;

namespace Meteor.Map.packets.send.group
{
    class CreateNamedGroupMultiple
    {
        public const ushort OPCODE = 0x0189;
        public const uint PACKET_SIZE = 0x228;

        public static SubPacket buildPacket(uint playerActorID, Group[] groups, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max = 8;
                    if (groups.Length - offset <= 8)
                        max = groups.Length - offset;

                    for (int i = 0; i < max; i++)
                    {                        
                        binWriter.Seek(i * 0x40, SeekOrigin.Begin);

                        Group group = groups[offset+i];

                        binWriter.Write((UInt64)group.groupIndex);
                        binWriter.Write((UInt32)group.GetTypeId());
                        binWriter.Write((Int32)group.GetGroupLocalizedName());

                        binWriter.Write((UInt16)0x121C);

                        binWriter.Seek(0x20, SeekOrigin.Begin);

                        binWriter.Write(Encoding.ASCII.GetBytes(group.GetGroupName()), 0, Encoding.ASCII.GetByteCount(group.GetGroupName()) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(group.GetGroupName()));                    
                    }

                    binWriter.Seek(0x200, SeekOrigin.Begin);
                    binWriter.Write((Byte)max);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

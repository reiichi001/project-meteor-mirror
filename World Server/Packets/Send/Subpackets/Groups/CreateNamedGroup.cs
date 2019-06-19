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

using System;
using System.IO;
using System.Text;

using Meteor.Common;
using Meteor.World.DataObjects.Group;

namespace Meteor.World.Packets.Send.Subpackets.Groups
{
    class CreateNamedGroup
    {
        public const ushort OPCODE = 0x0188;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket buildPacket(uint sessionId, Group group)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)group.groupIndex);
                    binWriter.Write((UInt32)group.GetTypeId());
                    binWriter.Write((Int32)group.GetGroupLocalizedName());

                    binWriter.Write((UInt16)0x121C);

                    binWriter.Seek(0x20, SeekOrigin.Begin);

                    binWriter.Write(Encoding.ASCII.GetBytes(group.GetGroupName()), 0, Encoding.ASCII.GetByteCount(group.GetGroupName()) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(group.GetGroupName()));                    
                }
            }

            return new SubPacket(OPCODE, sessionId, data);
        }
    }
}

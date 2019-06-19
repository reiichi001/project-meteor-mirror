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

using Meteor.Common;
using Meteor.World.DataObjects.Group;

namespace Meteor.World.Packets.Send.Subpackets.Groups
{
    class DeleteGroupPacket
    {
        public const ushort OPCODE = 0x0143;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint sessionId, Group group)
        {
            return buildPacket(sessionId, group.groupIndex);
        }

        public static SubPacket buildPacket(uint sessionId, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write control num ????
                    binWriter.Write((UInt64)3);

                    //Write Ids
                    binWriter.Write((UInt64)groupId);
                    binWriter.Write((UInt64)0);
                    binWriter.Write((UInt64)groupId);
                }
            }

            return new SubPacket(OPCODE, sessionId, data);
        }
    }
}

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

namespace Meteor.Map.packets.send.recruitment
{
    class RecruiterStatePacket
    {
        public const ushort OPCODE = 0x01C5;
        public const uint PACKET_SIZE = 0x038;

        public static SubPacket BuildPacket(uint sourceActorId, bool isRecruiting, bool isRecruiter, long recruitmentId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)recruitmentId);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((byte)(isRecruiter ? 1 : 0));
                    binWriter.Write((byte)(isRecruiting ? 1 : 0));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

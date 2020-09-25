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

namespace  Meteor.Map.packets.send.actor
{
    class MoveActorToPositionPacket
    {
        public const ushort OPCODE = 0x00CF;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket BuildPacket(uint sourceActorId, float x, float y, float z, float rot, ushort moveState)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.BaseStream.Seek(0x8, SeekOrigin.Begin);
                    binWriter.Write((Single)x);
                    binWriter.Write((Single)y);
                    binWriter.Write((Single)z);
                    binWriter.Write((Single)rot);
                    binWriter.Write((ushort)moveState);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, data);
            return packet;
        }

    }
}

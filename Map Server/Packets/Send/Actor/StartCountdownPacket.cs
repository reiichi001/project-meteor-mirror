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
using System.Text;

namespace  Meteor.Map.packets.send.actor
{
    class StartCountdownPacket
    {
        public const ushort OPCODE = 0xE5;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint sourceActorId, byte countdownLength, ulong syncTime, string message)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Byte)countdownLength);
                    binWriter.Seek(8, SeekOrigin.Begin);
                    binWriter.Write((UInt64)syncTime);
                    binWriter.Seek(18, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(message), 0, Encoding.ASCII.GetByteCount(message) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(message));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

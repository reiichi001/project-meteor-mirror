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
using Meteor.Map.dataobjects;
using System;
using System.IO;
using System.Text;

namespace Meteor.Map.packets.WorldPackets.Send.Group
{
    class LinkshellLeavePacket
    {
        public const ushort OPCODE = 0x1031;
        public const uint PACKET_SIZE = 0x68;

        public static SubPacket BuildPacket(Session session, string lsName, string kickedName, bool isKicked)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)(isKicked ? 1 : 0));
                    if (kickedName != null && isKicked)
                        binWriter.Write(Encoding.ASCII.GetBytes(kickedName), 0, Encoding.ASCII.GetByteCount(kickedName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(kickedName));
                    binWriter.Seek(0x22, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(lsName), 0, Encoding.ASCII.GetByteCount(lsName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(lsName));
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }

    }
}

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
using System.Text;

using Meteor.Common;

namespace Meteor.Map.packets.send.supportdesk
{
    class GMTicketPacket
    {
        public const ushort OPCODE = 0x01D4;
        public const uint PACKET_SIZE = 0x2B8;

        public static SubPacket BuildPacket(uint playerActorID, string titleText, string bodyText)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Seek(0x0, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(titleText), 0, Encoding.ASCII.GetByteCount(titleText) >= 0x80 ? 0x80 : Encoding.ASCII.GetByteCount(titleText));
                    binWriter.Seek(0x80, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(bodyText), 0, Encoding.ASCII.GetByteCount(bodyText) >= maxBodySize ? maxBodySize : Encoding.ASCII.GetByteCount(bodyText));
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

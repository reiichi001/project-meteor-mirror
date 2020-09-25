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

namespace Meteor.Lobby.Packets
{
    class ErrorPacket
    {
        private const ushort OPCODE = 0x02;

        private UInt64 sequence;
        private uint errorCode;
        private uint statusCode;
        private uint textId;
        private string message;

        public ErrorPacket(UInt64 sequence, uint errorCode, uint statusCode, uint textId, string message)
        {
            this.sequence = sequence;
            this.errorCode = errorCode;
            this.statusCode = statusCode;
            this.textId = textId;
            this.message = message;
        }

        public SubPacket BuildPacket()
        {
            MemoryStream memStream = new MemoryStream(0x210);
            BinaryWriter binWriter = new BinaryWriter(memStream);

            binWriter.Write(sequence);
            binWriter.Write(errorCode);
            binWriter.Write(statusCode);
            binWriter.Write(textId);
            binWriter.Write(Encoding.ASCII.GetBytes(message));

            byte[] data = memStream.GetBuffer();
            binWriter.Dispose();
            memStream.Dispose();
            SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
            subpacket.SetTargetId(0xe0006868);
            return subpacket;
        }
    }
}

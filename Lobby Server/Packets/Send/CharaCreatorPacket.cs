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
    class CharaCreatorPacket
    {
        public const ushort OPCODE = 0x0E;

        public const ushort RESERVE = 0x01;
        public const ushort MAKE = 0x02;
        public const ushort RENAME = 0x03;
        public const ushort DELETE = 0x04;
        public const ushort RENAME_RETAINER = 0x06;

        private UInt64 sequence;

        private ushort command;
        private uint pid;
        private uint cid;
        private uint type;
        private uint ticket;
        private string charaName;
        private string worldName;

        public CharaCreatorPacket(UInt64 sequence, ushort command, uint pid, uint cid, uint ticket, string charaName, string worldName)
        {
            this.sequence = sequence;
            this.command = command;
            this.pid = pid;
            this.cid = cid;
            this.type = 0x400017;
            this.ticket = ticket;
            this.charaName = charaName; 
            this.worldName = worldName;
        }        

        public SubPacket BuildPacket()    
        {
            MemoryStream memStream = new MemoryStream(0x1F0);
            BinaryWriter binWriter = new BinaryWriter(memStream);

            binWriter.Write((UInt64)sequence);
            binWriter.Write((byte)1);
            binWriter.Write((byte)1);
            binWriter.Write((UInt16)command);
            binWriter.Write((UInt32)0);

            binWriter.Write((UInt32)pid); //PID
            binWriter.Write((UInt32)cid); //CID
            binWriter.Write((UInt32)type); //Type?
            binWriter.Write((UInt32)ticket); //Ticket
            binWriter.Write(Encoding.ASCII.GetBytes(charaName.PadRight(0x20, '\0'))); //Name
            binWriter.Write(Encoding.ASCII.GetBytes(worldName.PadRight(0x20, '\0'))); //World Name

            byte[] data = memStream.GetBuffer();
            binWriter.Dispose();
            memStream.Dispose();

            return new SubPacket(OPCODE, 0xe0006868, data);
        }
    }
}

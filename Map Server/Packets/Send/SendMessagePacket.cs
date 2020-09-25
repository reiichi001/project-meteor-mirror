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

namespace Meteor.Map.packets.send
{
    class SendMessagePacket
    {       
            public const int MESSAGE_TYPE_NONE       = 0;
            public const int MESSAGE_TYPE_SAY        = 1;
            public const int MESSAGE_TYPE_SHOUT      = 2;
            public const int MESSAGE_TYPE_TELL       = 3;
            public const int MESSAGE_TYPE_PARTY      = 4;
            public const int MESSAGE_TYPE_LINKSHELL1 = 5;
            public const int MESSAGE_TYPE_LINKSHELL2 = 6;
            public const int MESSAGE_TYPE_LINKSHELL3 = 7;
            public const int MESSAGE_TYPE_LINKSHELL4 = 8;
            public const int MESSAGE_TYPE_LINKSHELL5 = 9;
            public const int MESSAGE_TYPE_LINKSHELL6 = 10;
            public const int MESSAGE_TYPE_LINKSHELL7 = 11;
            public const int MESSAGE_TYPE_LINKSHELL8 = 12;

            public const int MESSAGE_TYPE_SAY_SPAM       = 22;
            public const int MESSAGE_TYPE_SHOUT_SPAM     = 23;
            public const int MESSAGE_TYPE_TELL_SPAM      = 24;
            public const int MESSAGE_TYPE_CUSTOM_EMOTE   = 25;
            public const int MESSAGE_TYPE_EMOTE_SPAM     = 26;
            public const int MESSAGE_TYPE_STANDARD_EMOTE = 27;
            public const int MESSAGE_TYPE_URGENT_MESSAGE = 28;
            public const int MESSAGE_TYPE_GENERAL_INFO   = 29;
            public const int MESSAGE_TYPE_SYSTEM         = 32;
            public const int MESSAGE_TYPE_SYSTEM_ERROR   = 33;

            public const ushort OPCODE = 0x0003;
            public const uint PACKET_SIZE = 0x248;

            public static SubPacket BuildPacket(uint sourceActorId, uint messageType, string sender, string message)
            {
                byte[] data = new byte[PACKET_SIZE - 0x20];

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (BinaryWriter binWriter = new BinaryWriter(mem))
                    {
                        binWriter.Write(Encoding.ASCII.GetBytes(sender), 0, Encoding.ASCII.GetByteCount(sender) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(sender));
                        binWriter.BaseStream.Seek(0x20, SeekOrigin.Begin);
                        binWriter.Write((UInt32)messageType);
                        binWriter.Write(Encoding.ASCII.GetBytes(message), 0, Encoding.ASCII.GetByteCount(message) >= 0x200 ? 0x200 : Encoding.ASCII.GetByteCount(message));
                    }
                }

                return new SubPacket(OPCODE, sourceActorId, data);
            }

    }
}

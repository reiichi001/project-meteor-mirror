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

namespace Meteor.World.Packets.Send
{
    class _0x2Packet
    {
        public const ushort OPCODE = 0x0002;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(uint actorID)
        {
            byte[] data =  {                                           
                                            0x6c, 0x00, 0x00, 0x00, 0xC8, 0xD6, 0xAF, 0x2B, 0x38, 0x2B, 0x5F, 0x26, 0xB8, 0x8D, 0xF0, 0x2B,
                                            0xC8, 0xFD, 0x85, 0xFE, 0xA8, 0x7C, 0x5B, 0x09, 0x38, 0x2B, 0x5F, 0x26, 0xC8, 0xD6, 0xAF, 0x2B,
                                            0xB8, 0x8D, 0xF0, 0x2B, 0x88, 0xAF, 0x5E, 0x26
                                        };

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Seek(0, SeekOrigin.Begin);
                        binWriter.Write((UInt32)actorID);                        
                    }
                    catch (Exception)
                    { }
                }
            }

            /*
            0x6c, 0x00, 0x00, 0x00, 0xC8, 0xD6, 0xAF, 0x2B, 0x38, 0x2B, 0x5F, 0x26, 0xB8, 0x8D, 0xF0, 0x2B,
            0xC8, 0xFD, 0x85, 0xFE, 0xA8, 0x7C, 0x5B, 0x09, 0x38, 0x2B, 0x5F, 0x26, 0xC8, 0xD6, 0xAF, 0x2B,
            0xB8, 0x8D, 0xF0, 0x2B, 0x88, 0xAF, 0x5E, 0x26
             */

            return new SubPacket(false, OPCODE, 0, data);
        }
    }
}

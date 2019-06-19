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

namespace  Meteor.Map.packets.send.actor
{
    class SetActorSpeedPacket
    {
        public const ushort OPCODE = 0x00D0;
        public const uint PACKET_SIZE = 0xA8;

        public const float DEFAULT_STOP = 0.0f;
        public const float DEFAULT_WALK = 2.0f;
        public const float DEFAULT_RUN = 5.0f;
        public const float DEFAULT_ACTIVE = 5.0f;

        public static SubPacket BuildPacket(uint sourceActorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)DEFAULT_STOP);
                    binWriter.Write((UInt32)0);

                    binWriter.Write((Single)DEFAULT_WALK);
                    binWriter.Write((UInt32)1);

                    binWriter.Write((Single)DEFAULT_RUN);
                    binWriter.Write((UInt32)2);

                    binWriter.Write((Single)DEFAULT_ACTIVE);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)4);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, float stopSpeed, float walkSpeed, float runSpeed, float activeSpeed)
        {               
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)stopSpeed);
                    binWriter.Write((UInt32)0);

                    binWriter.Write((Single)walkSpeed);
                    binWriter.Write((UInt32)1);

                    binWriter.Write((Single)runSpeed);
                    binWriter.Write((UInt32)2);
                    
                    binWriter.Write((Single)activeSpeed);
                    binWriter.Write((UInt32)3);

                    binWriter.BaseStream.Seek(0x80, SeekOrigin.Begin);

                    binWriter.Write((UInt32)4);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

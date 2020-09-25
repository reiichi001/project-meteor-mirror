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

using Meteor.Map.actors;
using System;
using System.IO;
using System.Text;

using Meteor.Common;

namespace  Meteor.Map.packets.send.actor.events
{
    class SetPushEventConditionWithCircle
    {
        public const ushort OPCODE = 0x016F;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket BuildPacket(uint sourceActorId, EventList.PushCircleEventCondition condition)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)condition.radius);
                    binWriter.Write((UInt32)0x44533088);
                    binWriter.Write((Single)100.0f);
                    binWriter.Seek(4, SeekOrigin.Current);
                    binWriter.Write((Byte)(condition.outwards ? 0x11 : 0x1)); //If == 0x10, Inverted Bounding Box
                    binWriter.Write((Byte)0);
                    binWriter.Write((Byte)(condition.silent ? 0x1 : 0x0)); //Silent Trigger
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.conditionName), 0, Encoding.ASCII.GetByteCount(condition.conditionName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(condition.conditionName));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

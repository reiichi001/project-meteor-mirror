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

namespace Meteor.Map.packets.send.actor.events
{
    class SetPushEventConditionWithTriggerBox
    {
        public const ushort OPCODE = 0x0175;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket BuildPacket(uint sourceActorId, EventList.PushBoxEventCondition condition)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)condition.bgObj);  // bgObj
                    binWriter.Write((UInt32)condition.layout);   // Layout
                    binWriter.Write((UInt32)4);       // Actor?  Always 4 in 1.23
                    binWriter.Seek(8, SeekOrigin.Current); // Unknowns
                    binWriter.Write((Byte)(condition.outwards ? 0x11 : 0x0)); //If == 0x10, Inverted Bounding Box
                    binWriter.Write((Byte)3);
                    binWriter.Write((Byte)(condition.silent ? 0x1 : 0x0)); //Silent Trigger;
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.conditionName), 0, Encoding.ASCII.GetByteCount(condition.conditionName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(condition.conditionName));
                    binWriter.Seek(55, SeekOrigin.Begin);
                    binWriter.Write((Byte)0);       // Unknown
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.reactName), 0, Encoding.ASCII.GetByteCount(condition.reactName) >= 0x04 ? 0x04 : Encoding.ASCII.GetByteCount(condition.reactName));
                }
            }
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
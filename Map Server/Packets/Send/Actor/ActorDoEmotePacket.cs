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

namespace  Meteor.Map.packets.send.actor
{
    class ActorDoEmotePacket
    {
        public const ushort OPCODE = 0x00E1;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint sourceActorId, uint targettedActorId, uint animationId, uint descriptionId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            if (targettedActorId == 0)
            {
                targettedActorId = sourceActorId;
                if (descriptionId != 10105)
                    descriptionId++;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    uint realAnimID = 0x5000000 | (animationId << 12);                    
                    binWriter.Write((UInt32)realAnimID);
                    binWriter.Write((UInt32)targettedActorId);
                    binWriter.Write((UInt32)descriptionId);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, data);
            packet.DebugPrintSubPacket();
            return packet;
        }
    }
}

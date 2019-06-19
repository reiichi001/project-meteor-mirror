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

namespace Meteor.Map.packets.send.player
{
    class SetLatestAchievementsPacket
    {
        public const ushort OPCODE = 0x019B;
        public const uint PACKET_SIZE = 0x40;
       
        public static SubPacket BuildPacket(uint sourceActorId, uint[] latestAchievementIDs)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        //Had less than 5
                        if (i > latestAchievementIDs.Length)
                            break;
                        binWriter.Write((UInt32)latestAchievementIDs[i]);
                    }
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

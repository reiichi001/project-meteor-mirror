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

namespace Meteor.Map.packets.send.social
{
    class FriendStatusPacket
    {
        public const ushort OPCODE = 0x01CF;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket BuildPacket(uint sourceActorId, Tuple<long, bool>[] friendStatus)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)0);
                    int max;

                    if (friendStatus != null)
                    {
                        if (friendStatus.Length <= 200)
                            max = friendStatus.Length;
                        else
                            max = 200;
                    }
                    else
                        max = 0;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt64)friendStatus[i].Item1);
                        binWriter.Write((UInt64)(friendStatus[i].Item2 ? 1 : 0));
                    }

                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

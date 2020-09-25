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

namespace Meteor.Map.packets.send.social
{
    class SendFriendlistPacket
    {
        public const ushort OPCODE = 0x01CE;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket BuildPacket(uint sourceActorId, Tuple<long, string>[] friends, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)0);
                    int max;

                    if (friends.Length - offset <= 0x32)
                        max = friends.Length - offset;
                    else
                        max = 0x32;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write(Encoding.ASCII.GetBytes(friends[i].Item2), 0, Encoding.ASCII.GetByteCount(friends[i].Item2) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(friends[i].Item2));
                        binWriter.Write((UInt64)friends[i].Item1);
                    }

                    offset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

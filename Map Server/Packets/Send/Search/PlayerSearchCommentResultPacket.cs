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

using System.IO;
using System.Text;

using Meteor.Common;
using System;

namespace Meteor.Map.packets.send.search
{
    class PlayerSearchCommentResultPacket
    {
        public const ushort OPCODE = 0x01E0;
        public const uint PACKET_SIZE = 0x288;

        public static SubPacket BuildPacket(uint sourceActorId, uint searchSessionId, byte resultCode, PlayerSearchResult[] results, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            byte count = 0;

            for (int i = offset; i < results.Length; i++)
            {
                int size = 3 + (Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment));

                if (size >= 600)
                    break;

                count++;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = offset; i < count; i++)
                    {
                        binWriter.Write((UInt32)searchSessionId);
                        binWriter.Write((Byte)count);
                        binWriter.Seek(1, SeekOrigin.Current);
                        binWriter.Write((Byte)resultCode);
                        binWriter.Seek(4, SeekOrigin.Current);

                        binWriter.Write((Byte)i);
                        binWriter.Write((UInt16)(Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment)));
                        binWriter.Write(Encoding.ASCII.GetBytes(results[i].comment), 0, Encoding.ASCII.GetByteCount(results[i].comment) >= 597 ? 596 : Encoding.ASCII.GetByteCount(results[i].comment));
                    }
                }
            }

            offset += count;

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

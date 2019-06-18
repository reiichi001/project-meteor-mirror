/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.supportdesk
{
    class IssueListResponsePacket
    {
        public const ushort OPCODE = 0x01D2;
        public const uint PACKET_SIZE = 0x160;

        public static SubPacket BuildPacket(uint playerActorID, string[] issueStrings)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < (issueStrings.Length <= 5 ? issueStrings.Length : 5); i++)
                    {
                        binWriter.Seek(0x40 * i, SeekOrigin.Begin);
                        binWriter.Write(Encoding.ASCII.GetBytes(issueStrings[i]), 0, Encoding.ASCII.GetByteCount(issueStrings[i]) >= 0x40 ? 0x40 : Encoding.ASCII.GetByteCount(issueStrings[i]));
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

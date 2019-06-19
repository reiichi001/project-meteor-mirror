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

using Meteor.Map.dataobjects;
using System;
using System.IO;
using System.Text;

using Meteor.Common;

namespace Meteor.Map.packets.send.recruitment
{
    class CurrentRecruitmentDetailsPacket
    {
        public const ushort OPCODE = 0x01C8;
        public const uint PACKET_SIZE = 0x218;

        public static SubPacket BuildPacket(uint sourceActorId, RecruitmentDetails details)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    if (details == null)
                    {
                        return new SubPacket(OPCODE, sourceActorId, data);
                    }

                    binWriter.Write((UInt32)details.purposeId);
                    binWriter.Write((UInt32)details.locationId);
                    binWriter.Write((UInt32)details.subTaskId);
                    binWriter.Write((UInt32)details.timeSinceStart);

                    for (int i = 0; i < 4; i++)
                    {
                        binWriter.Write((UInt32)details.discipleId[i]);
                        binWriter.Write((UInt32)details.classjobId[i]);
                        binWriter.Write((byte)details.minLvl[i]);
                        binWriter.Write((byte)details.maxLvl[i]);
                        binWriter.Write((byte)details.num[i]);
                        binWriter.Write((byte)0);
                    }

                    binWriter.Write(Encoding.ASCII.GetBytes(details.comment), 0, Encoding.ASCII.GetByteCount(details.comment) >= 0x168 ? 0x168 : Encoding.ASCII.GetByteCount(details.comment));
                    binWriter.Seek(0x1C0, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(details.recruiterName), 0, Encoding.ASCII.GetByteCount(details.recruiterName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(details.recruiterName));
                    binWriter.Seek(0x1E0, SeekOrigin.Begin);
                    binWriter.Write((byte)1);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

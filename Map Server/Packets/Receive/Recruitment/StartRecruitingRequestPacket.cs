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

namespace Meteor.Map.packets.receive.recruitment
{
    class StartRecruitingRequestPacket
    {
        public bool invalidPacket = false;

        public uint purposeId;
        public uint locationId;
        public uint subTaskId;

        public uint[] discipleId = new uint[4];
        public uint[] classjobId = new uint[4];
        public byte[] minLvl = new byte[4];
        public byte[] maxLvl = new byte[4];
        public byte[] num = new byte[4];

        public string comment;

        public StartRecruitingRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        purposeId = binReader.ReadUInt32();
                        locationId = binReader.ReadUInt32();
                        subTaskId = binReader.ReadUInt32();

                        for (int i = 0; i < 4; i++)
                        {
                            discipleId[i] = binReader.ReadUInt32();
                            classjobId[i] = binReader.ReadUInt32();
                            minLvl[i] = binReader.ReadByte();
                            maxLvl[i] = binReader.ReadByte();
                            num[i] = binReader.ReadByte();
                            binReader.ReadByte();
                        }

                        comment = Utils.ReadNullTermString(binReader, 0x168);
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

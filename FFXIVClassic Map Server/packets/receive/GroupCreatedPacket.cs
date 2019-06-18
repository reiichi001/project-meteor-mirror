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

using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class GroupCreatedPacket
    {    
        public ulong groupId;
        public string workString;

        public bool invalidPacket = false;

        public GroupCreatedPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        groupId = binReader.ReadUInt64();
                        workString = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

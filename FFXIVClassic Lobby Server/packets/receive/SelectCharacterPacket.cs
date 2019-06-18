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

namespace FFXIVClassic_Lobby_Server.packets.receive
{
    class SelectCharacterPacket
    {
        public UInt64 sequence;
        public uint characterId;
        public uint unknownId;
        public UInt64 ticket;

        public bool invalidPacket = false;

        public SelectCharacterPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sequence = binReader.ReadUInt64();
                        characterId = binReader.ReadUInt32();
                        unknownId = binReader.ReadUInt32();
                        ticket = binReader.ReadUInt64();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

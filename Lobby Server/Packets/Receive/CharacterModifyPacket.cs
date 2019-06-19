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

namespace Meteor.Lobby.Packets.Receive
{
    class CharacterModifyPacket
    {
        public UInt64 sequence;
        public uint characterId;
        public uint personType;
        public byte slot;
        public byte command;
        public ushort worldId;
        public String characterName;
        public String characterInfoEncoded;

        public bool invalidPacket = false;

        public CharacterModifyPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sequence = binReader.ReadUInt64();
                        characterId = binReader.ReadUInt32();
                        personType = binReader.ReadUInt32();
                        slot = binReader.ReadByte();
                        command = binReader.ReadByte();
                        worldId = binReader.ReadUInt16();

                        characterName = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                        characterInfoEncoded = Encoding.ASCII.GetString(binReader.ReadBytes(0x190)).Trim(new[] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

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
using System.Collections.Generic;
using System.IO;

using Meteor.Map.lua;

namespace Meteor.Map.packets.receive.events
{
    class EventUpdatePacket
    {
        public const ushort OPCODE = 0x012E;
        public const uint PACKET_SIZE = 0x78;

        public bool invalidPacket = false;

        public uint triggerActorID;
        public uint serverCodes;
        public uint unknown1;
        public uint unknown2;
        public byte eventType;
        public List<LuaParam> luaParams;

        public EventUpdatePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        triggerActorID = binReader.ReadUInt32();
                        serverCodes = binReader.ReadUInt32();
                        unknown1 = binReader.ReadUInt32();
                        unknown2 = binReader.ReadUInt32();
                        eventType = binReader.ReadByte();
                        luaParams = LuaUtils.ReadLuaParams(binReader);
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

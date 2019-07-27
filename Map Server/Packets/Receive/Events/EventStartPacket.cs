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
using Meteor.Map.lua;
using System;
using System.Collections.Generic;
using System.IO;

namespace Meteor.Map.packets.receive.events
{
    class EventStartPacket
    {
        public const ushort OPCODE = 0x012D;
        public const uint PACKET_SIZE = 0xD8;

        public bool invalidPacket = false;

        public uint triggerActorID;
        public uint ownerActorID;
        public uint serverCodes;
        public uint unknown;
        public byte eventType;
        public string eventName;
        public List<LuaParam> luaParams;

        public uint errorIndex;
        public uint errorNum;
        public string error = null;
        
        public EventStartPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        triggerActorID = binReader.ReadUInt32();
                        ownerActorID = binReader.ReadUInt32();
                        serverCodes = binReader.ReadUInt32();
                        unknown = binReader.ReadUInt32();
                        eventType = binReader.ReadByte();
                        /*
                        //Lua Error Dump
                        if (val1 == 0x39800010)
                        {
                            errorIndex = actorID;
                            errorNum = scriptOwnerActorID;
                            error = ASCIIEncoding.ASCII.GetString(binReader.ReadBytes(0x80)).Replace("\0", "");

                            if (errorIndex == 0)
                                Program.Log.Error("LUA ERROR:");                            

                            return;
                        }
                        */
                        eventName = Utils.ReadNullTermString(binReader);

                        if (binReader.PeekChar() == 0x1)
                            luaParams = new List<LuaParam>();
                        else
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

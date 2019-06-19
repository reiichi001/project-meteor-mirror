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

using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.receive.events
{
    class EventStartPacket
    {
        public const ushort OPCODE = 0x012D;
        public const uint PACKET_SIZE = 0xD8;

        public bool invalidPacket = false;

        public uint actorID;
        public uint scriptOwnerActorID;
        public uint val1;
        public uint val2;
        public byte val3;

        public uint errorIndex;
        public uint errorNum;
        public string error = null;

        public string triggerName;

        public List<LuaParam> luaParams;

        public EventStartPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        scriptOwnerActorID = binReader.ReadUInt32();
                        val1 = binReader.ReadUInt32();
                        val2 = binReader.ReadUInt32();
                        val3 = binReader.ReadByte();
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
                        List<byte> strList = new List<byte>();
                        byte curByte;
                        while ((curByte = binReader.ReadByte())!=0)
                        {
                            strList.Add(curByte);
                        }
                        triggerName = Encoding.ASCII.GetString(strList.ToArray());

                        binReader.BaseStream.Seek(0x31, SeekOrigin.Begin);

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

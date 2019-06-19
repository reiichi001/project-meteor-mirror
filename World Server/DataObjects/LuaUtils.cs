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
using System.Text;

using Meteor.Common;

namespace Meteor.World.DataObjects
{
    class LuaUtils
    {
    
        public class Type7Param
        {
            public uint actorId;
            public byte unknown;
            public byte slot;
            public byte inventoryType;

            public Type7Param(uint actorId, byte unknown, byte slot, byte inventoryType)
            {
                this.actorId = actorId;
                this.unknown = unknown;
                this.slot = slot;
                this.inventoryType = inventoryType;
            }
        }

        public class Type9Param
        {
            public ulong item1;
            public ulong item2;

            public Type9Param(ulong item1, ulong item2)
            {
                this.item1 = item1;
                this.item2 = item2;
            }
        }

        public static List<LuaParam> ReadLuaParams(BinaryReader reader)
        {
            List<LuaParam> luaParams = new List<LuaParam>();

            bool isDone = false;
            while (true)
            {
                byte code = reader.ReadByte();
                object value = null;
                bool wasNil = false;

                switch (code)
                {
                    case 0x0: //Int32
                        value = Utils.SwapEndian(reader.ReadInt32());                        
                        break;
                    case 0x1: //Int32
                        value = Utils.SwapEndian(reader.ReadUInt32());        
                        break;          
                    case 0x2: //Null Termed String                        
                        List<byte> list = new List<byte>();
                        while(true){
                            byte readByte = reader.ReadByte();
                            if (readByte == 0)
                                break;
                            list.Add(readByte);
                        }
                        value = Encoding.ASCII.GetString(list.ToArray());
                        break;
                    case 0x3: //Boolean True
                        value = true;
                        break;
                    case 0x4: //Boolean False
                        value = false;
                        break;
                    case 0x5: //Nil
                        wasNil = true;
                        break;
                    case 0x6: //Actor (By Id)
                        value = Utils.SwapEndian(reader.ReadUInt32());
                        break;
                    case 0x7: //Weird one used for inventory
                        uint type7ActorId = Utils.SwapEndian(reader.ReadUInt32());
                        byte type7Unknown = reader.ReadByte();
                        byte type7Slot = reader.ReadByte();
                        byte type7InventoryType = reader.ReadByte();
                        value = new Type7Param(type7ActorId, type7Unknown, type7Slot, type7InventoryType);
                        break;  
                    case 0x9: //Two Longs (only storing first one)
                        value = new Type9Param(Utils.SwapEndian(reader.ReadUInt64()), Utils.SwapEndian(reader.ReadUInt64()));
                        break;
                    case 0xC: //Byte
                        value = reader.ReadByte();
                        break;
                    case 0x1B: //Short?
                        value = reader.ReadUInt16();
                        break;
                    case 0xF: //End
                        isDone = true;
                        continue;
                }

                if (isDone)
                    break;

                if (value != null)
                    luaParams.Add(new LuaParam(code, value));
                else if (wasNil)
                    luaParams.Add(new LuaParam(code, value));
            }

            return luaParams;
        }

        public static void WriteLuaParams(BinaryWriter writer, List<LuaParam> luaParams)
        {
            foreach (LuaParam l in luaParams)
            {           
                if (l.typeID == 0x1)
                    writer.Write((Byte)0);
                else
                    writer.Write((Byte)l.typeID);

                switch (l.typeID)
                {
                    case 0x0: //Int32                        
                        writer.Write((Int32)Utils.SwapEndian((Int32)l.value));
                        break;
                    case 0x1: //Int32                        
                        writer.Write((UInt32)Utils.SwapEndian((UInt32)l.value));
                        break;
                    case 0x2: //Null Termed String
                        string sv = (string)l.value;
                        writer.Write(Encoding.ASCII.GetBytes(sv), 0, Encoding.ASCII.GetByteCount(sv));
                        writer.Write((Byte)0);
                        break;
                    case 0x3: //Boolean True                        
                        break;
                    case 0x4: //Boolean False                        
                        break;
                    case 0x5: //Nil                        
                        break;
                    case 0x6: //Actor (By Id)
                        writer.Write((UInt32)Utils.SwapEndian((UInt32)l.value));
                        break;
                    case 0x7: //Weird one used for inventory
                        Type7Param type7 = (Type7Param)l.value;
                        writer.Write((UInt32)Utils.SwapEndian((UInt32)type7.actorId));
                        writer.Write((Byte)type7.unknown);
                        writer.Write((Byte)type7.slot);
                        writer.Write((Byte)type7.inventoryType);
                        break;
                    case 0x9: //Two Longs (only storing first one)
                        writer.Write((UInt64)Utils.SwapEndian(((Type9Param)l.value).item1));
                        writer.Write((UInt64)Utils.SwapEndian(((Type9Param)l.value).item2));
                        break;
                    case 0xC: //Byte
                        writer.Write((Byte)l.value);
                        break;
                    case 0x1B: //Short?                        
                        break;
                    case 0xF: //End                        
                        continue;
                }
            }

            writer.Write((Byte)0xF);
        }

        public static List<LuaParam> ReadLuaParams(byte[] bytesIn)
        {
            List<LuaParam> luaParams = new List<LuaParam>();            

            using (MemoryStream memStream = new MemoryStream(bytesIn))
            {
                using (BinaryReader reader = new BinaryReader(memStream))
                {
                    bool isDone = false;
                    while (true)
                    {
                        byte code = reader.ReadByte();
                        object value = null;
                        bool wasNil = false;

                        switch (code)
                        {
                            case 0x0: //Int32
                                value = Utils.SwapEndian(reader.ReadInt32());
                                break;
                            case 0x1: //Int32
                                value = Utils.SwapEndian(reader.ReadUInt32());
                                break;
                            case 0x2: //Null Termed String                        
                                List<byte> list = new List<byte>();
                                while (true)
                                {
                                    byte readByte = reader.ReadByte();
                                    if (readByte == 0)
                                        break;
                                    list.Add(readByte);
                                }
                                value = Encoding.ASCII.GetString(list.ToArray());
                                break;
                            case 0x3: //Boolean True
                                value = true;
                                break;
                            case 0x4: //Boolean False
                                value = false;
                                break;
                            case 0x5: //Nil
                                wasNil = true;
                                break;
                            case 0x6: //Actor (By Id)
                                value = Utils.SwapEndian(reader.ReadUInt32());
                                break;
                            case 0x7: //Weird one used for inventory
                                uint type7ActorId = Utils.SwapEndian(reader.ReadUInt32());
                                byte type7Unknown = reader.ReadByte();
                                byte type7Slot = reader.ReadByte();
                                byte type7InventoryType = reader.ReadByte();
                                value = new Type7Param(type7ActorId, type7Unknown, type7Slot, type7InventoryType);
                                break;
                            case 0x9: //Two Longs (only storing first one)
                                value = new Type9Param(Utils.SwapEndian(reader.ReadUInt64()), Utils.SwapEndian(reader.ReadUInt64()));
                                break;
                            case 0xC: //Byte
                                value = reader.ReadByte();
                                break;
                            case 0x1B: //Short?
                                value = reader.ReadUInt16();
                                break;
                            case 0xF: //End
                                isDone = true;
                                continue;
                        }

                        if (isDone)
                            break;

                        if (value != null)
                            luaParams.Add(new LuaParam(code, value));
                        else if (wasNil)
                            luaParams.Add(new LuaParam(code, value));
                    }
                }
            }
            return luaParams;
        }

        public static List<LuaParam> CreateLuaParamList(params object[] list)
        {
            List<LuaParam> luaParams = new List<LuaParam>();

            foreach (object o in list)
            {
                if (o != null && o.GetType().IsArray)
                {
                    Array arrayO = (Array)o;
                    foreach (object o2 in arrayO)
                        AddToList(o2, luaParams);
                }
                else
                    AddToList(o, luaParams);                     
            }

            return luaParams;
        }

        private static void AddToList(object o, List<LuaParam> luaParams)
        {
            if (o is int)
            {
                luaParams.Add(new LuaParam(0x0, (int)o));
            }
            else if (o is uint)
            {
                luaParams.Add(new LuaParam(0x1, (uint)o));
            }                
            else if (o is Double)
            {
                if (((double)o) % 1 == 0)
                    luaParams.Add(new LuaParam(0x0, (int)(double)o));
            }
            else if (o is string)
            {
                luaParams.Add(new LuaParam(0x2, (string)o));
            }
            else if (o is bool)
            {
                if (((bool)o))
                    luaParams.Add(new LuaParam(0x3, null));
                else
                    luaParams.Add(new LuaParam(0x4, null));
            }
            else if (o == null)
            {
                luaParams.Add(new LuaParam(0x5, null));
            }
            else if (o is Session)
            {
                luaParams.Add(new LuaParam(0x6, (uint)((Session)o).sessionId));
            }    
            else if (o is Type7Param)
            {
                luaParams.Add(new LuaParam(0x7, (Type7Param)o)); 
            }
            else if (o is Type9Param)
            {
                luaParams.Add(new LuaParam(0x9, (Type9Param)o));
            }
            else if (o is byte)
            {
                luaParams.Add(new LuaParam(0xC, (byte)o));
            }
        }

        public static object[] CreateLuaParamObjectList(List <LuaParam> luaParams)
        {
            object[] list = new object[luaParams.Count];

            for (int i = 0; i < list.Length; i++)
                list[i] = luaParams[i].value;

            return list;
        }
    
      
        public static string DumpParams(List<LuaParam> lParams)
        {
            if (lParams == null)
                return "Param list was null?";

            string dumpString = "";
            for (int i = 0; i < lParams.Count; i++)
            {
                switch (lParams[i].typeID)
                {
                    case 0x0: //Int32
                        dumpString += String.Format("0x{0:X}", (int)lParams[i].value);
                        break;
                    case 0x1: //Int32
                        dumpString += String.Format("0x{0:X}", (uint)lParams[i].value);
                        break;
                    case 0x2: //Null Termed String                        
                        dumpString += String.Format("\"{0}\"", (string)lParams[i].value);
                        break;
                    case 0x3: //Boolean True
                        dumpString += "true";
                        break;
                    case 0x4: //Boolean False
                        dumpString += "false";
                        break;
                    case 0x5: //NULL???                        
                        dumpString += "nil";
                        break;
                    case 0x6: //Actor (By Id)
                        dumpString += String.Format("0x{0:X}", (uint)lParams[i].value);
                        break;
                    case 0x7: //Weird one used for inventory
                        Type7Param type7Param = ((Type7Param)lParams[i].value);
                        dumpString += String.Format("Type7 Param: (0x{0:X}, 0x{1:X}, 0x{2:X}, 0x{3:X})", type7Param.actorId, type7Param.unknown, type7Param.slot, type7Param.inventoryType);
                        break;
                    case 0xC: //Byte
                        dumpString += String.Format("0x{0:X}", (byte)lParams[i].value);
                        break;
                    case 0x9: //Long (+ 8 bytes ignored)
                        Type9Param type9Param = ((Type9Param)lParams[i].value);
                        dumpString += String.Format("Type9 Param: (0x{0:X}, 0x{1:X})", type9Param.item1, type9Param.item2);
                        break;
                    case 0x1B: //Short?
                        dumpString += String.Format("0x{0:X}", (ushort)lParams[i].value);
                        break;
                    case 0xF: //End
                        break;
                }

                if (i != lParams.Count - 1)
                    dumpString += ", ";
            }

            return dumpString;
        }

    }
}

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
using Meteor.Map.Actors;
using Meteor.Map.lua;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meteor.Map
{
    class LuaUtils
    {
    
        public class ItemRefParam
        {
            public uint actorId;
            public byte unknown;
            public byte slot;
            public byte itemPackage;

            public ItemRefParam(uint actorId, byte unknown, byte slot, byte itemPackage)
            {
                this.actorId = actorId;
                this.unknown = unknown;
                this.slot = slot;
                this.itemPackage = itemPackage;
            }
        }

        public class ItemOfferParam
        {
            public uint actorId;
            public ushort offerSlot;
            public byte offerPackageId;
            public byte unknown1;
            public ushort seekSlot;
            public byte seekPackageId;
            public byte unknown2;

            public ItemOfferParam(uint actorId, ushort offerSlot, byte offerPackageId, byte unknown1, ushort seekSlot, byte seekPackageId, byte unknown2)
            {
                this.actorId = actorId;
                this.offerSlot = offerSlot;
                this.offerPackageId = offerPackageId;
                this.unknown1 = unknown1;
                this.seekSlot = seekSlot;
                this.seekPackageId = seekPackageId;
                this.unknown2 = unknown2;
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
                    case 0x7: //Item Reference to Inventory Spot
                        {
                            uint type7ActorId = Utils.SwapEndian(reader.ReadUInt32());
                            byte type7Unknown = reader.ReadByte();
                            byte type7Slot = reader.ReadByte();
                            byte type7InventoryType = reader.ReadByte();
                            value = new ItemRefParam(type7ActorId, type7Unknown, type7Slot, type7InventoryType);
                        }
                        break;  
                    case 0x8: //Used for offering
                        {
                            uint actorId = Utils.SwapEndian(reader.ReadUInt32());
                            ushort rewardSlot = Utils.SwapEndian(reader.ReadUInt16());
                            byte   rewardPackageId = reader.ReadByte();
                            byte   unk1 = reader.ReadByte(); //Always 0x2?
                            ushort seekSlot = Utils.SwapEndian(reader.ReadUInt16());
                            byte   seekPackageId = reader.ReadByte();
                            byte   unk2 = reader.ReadByte(); //Always 0xD?
                            value = new ItemOfferParam(actorId, rewardSlot, rewardPackageId, unk1, seekSlot, seekPackageId, unk2);
                        }
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
                    default:
                        throw new ArgumentException("Unknown lua param...");
                }

                if (isDone)
                    break;

                //Special case cause fuck Type8
                if (value != null && value is ItemOfferParam)
                {
                    luaParams.Add(new LuaParam(code, value));
                    luaParams.Add(new LuaParam(0x5, null)); //This is to clean up the seek script as it fucks with the args.
                }
                else if (value != null)
                    luaParams.Add(new LuaParam(code, value));
                else if (wasNil)
                    luaParams.Add(new LuaParam(code, value));
            }

            return luaParams;
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
                                value = new ItemRefParam(type7ActorId, type7Unknown, type7Slot, type7InventoryType);
                                break;
                            case 0x8:
                                uint actorId = Utils.SwapEndian(reader.ReadUInt32());
                                ushort rewardSlot = Utils.SwapEndian(reader.ReadUInt16());
                                byte rewardPackageId = reader.ReadByte();
                                byte unk1 = reader.ReadByte(); //Always 0x2?
                                ushort seekSlot = Utils.SwapEndian(reader.ReadUInt16());
                                byte seekPackageId = reader.ReadByte();
                                byte unk2 = reader.ReadByte(); //Always 0xD?
                                value = new ItemOfferParam(actorId, rewardSlot, rewardPackageId, unk1, seekSlot, seekPackageId, unk2);
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
                            default:
                                throw new ArgumentException("Unknown lua param...");
                        }

                        if (isDone)
                            break;

                        if (value != null && value is ItemOfferParam)
                        {
                            luaParams.Add(new LuaParam(code, value));
                            luaParams.Add(new LuaParam(0x5, null)); //This is to clean up the seek script as it fucks with the args.
                        }
                        else if (value != null)
                            luaParams.Add(new LuaParam(code, value));
                        else if (wasNil)
                            luaParams.Add(new LuaParam(code, value));
                    }
                }
            }
            return luaParams;
        }

        public static void WriteLuaParams(BinaryWriter writer, List<LuaParam> luaParams)
        {
            if (luaParams == null)
            {
                Program.Log.Error("LuaUtils.WriteLuaParams LuaParams are null!");
                return;
            }

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
                        ItemRefParam type7 = (ItemRefParam)l.value;
                        writer.Write((UInt32)Utils.SwapEndian((UInt32)type7.actorId));
                        writer.Write((Byte)type7.unknown);
                        writer.Write((Byte)type7.slot);
                        writer.Write((Byte)type7.itemPackage);
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

        public static List<LuaParam> CreateLuaParamList(DynValue fromScript)
        {
            List<LuaParam> luaParams = new List<LuaParam>();

            if (fromScript == null)
                return luaParams;

            if (fromScript.Type == DataType.Tuple)
            {
                foreach (DynValue d in fromScript.Tuple)
                {
                    AddToList(d, luaParams);
                }
            }
            else
                AddToList(fromScript, luaParams);

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

        private static void AddToList(DynValue d, List<LuaParam> luaParams)
        {
            if (d.Type == DataType.Number)
            {
                luaParams.Add(new LuaParam(0x0, (int)d.Number));
            }
            else if (d.Type == DataType.Number)
            {
                luaParams.Add(new LuaParam(0x1, (uint)d.Number));
            }
            else if (d.Type == DataType.String)
            {
                luaParams.Add(new LuaParam(0x2, (string)d.String));
            }
            else if (d.Type == DataType.Boolean)
            {
                if (d.Boolean)
                    luaParams.Add(new LuaParam(0x3, null));
                else
                    luaParams.Add(new LuaParam(0x4, null));
            }
            else if (d.Type == DataType.Nil)
            {
                luaParams.Add(new LuaParam(0x5, null));
            }
            else if (d.Type == DataType.Table)
            {
                //luaParams.Add(new LuaParam(0x6, ((Actor)o).actorId));
            }
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
            else if (o is Actor)
            {
                luaParams.Add(new LuaParam(0x6, ((Actor)o).actorId));
            }
            else if (o is ItemRefParam)
            {
                luaParams.Add(new LuaParam(0x7, (ItemRefParam)o));
            }
            else if (o is ItemOfferParam)
            {
                luaParams.Add(new LuaParam(0x8, (ItemOfferParam)o));
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
                        ItemRefParam type7Param = ((ItemRefParam)lParams[i].value);
                        dumpString += String.Format("Type7 Param: (0x{0:X}, 0x{1:X}, 0x{2:X}, 0x{3:X})", type7Param.actorId, type7Param.unknown, type7Param.slot, type7Param.itemPackage);
                        break;
                    case 0x8: //Weird one used for inventory
                        ItemOfferParam itemOfferParam = ((ItemOfferParam)lParams[i].value);
                        dumpString += String.Format("Type8 Param: (0x{0:X}, 0x{1:X}, 0x{2:X}, 0x{3:X}, 0x{4:X}, 0x{5:X}, 0x{6:X})", itemOfferParam.actorId, itemOfferParam.offerSlot, itemOfferParam.offerPackageId, itemOfferParam.unknown1, itemOfferParam.seekSlot, itemOfferParam.seekPackageId, itemOfferParam.unknown2);
                        break;
                    case 0x9: //Long (+ 8 bytes ignored)
                        Type9Param type9Param = ((Type9Param)lParams[i].value);
                        dumpString += String.Format("Type9 Param: (0x{0:X}, 0x{1:X})", type9Param.item1, type9Param.item2);
                        break;
                    case 0xC: //Byte
                        dumpString += String.Format("0x{0:X}", (byte)lParams[i].value);
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

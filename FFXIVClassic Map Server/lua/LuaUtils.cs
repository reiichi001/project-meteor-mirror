using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server
{
    class LuaUtils
    {
        
        public static List<LuaParam> readLuaParams(BinaryReader reader)
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
                        value = Utils.swapEndian(reader.ReadUInt32());                        
                        break;
                    case 0x1: //Int32
                        value = Utils.swapEndian(reader.ReadUInt32());        
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
                    case 0x3: //Boolean False
                        value = false;
                        break;
                    case 0x4: //Boolean True
                        value = true;
                        break;
                    case 0x5: //Nil
                        wasNil = true;
                        break;
                    case 0x6: //Actor (By Id)
                        value = Utils.swapEndian(reader.ReadUInt32());
                        break;
                    case 0x10: //Byte?
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

        public static void writeLuaParams(BinaryWriter writer, List<LuaParam> luaParams)
        {
            foreach (LuaParam l in luaParams)
            {           
                writer.Write((Byte)l.typeID);
                switch (l.typeID)
                {
                    case 0x0: //Int32
                        writer.Write((UInt32)l.value);
                        break;
                    case 0x1: //Int32
                        writer.Write((UInt32)l.value);
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
                        writer.Write((UInt32)l.value);
                        break;
                    case 0x10: //Byte?                        
                        break;
                    case 0x1B: //Short?                        
                        break;
                    case 0xF: //End                        
                        continue;
                }
            }

            writer.Write((Byte)0xF);
        }

        public static List<LuaParam> readLuaParams(byte[] bytesIn)
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
                                value = Utils.swapEndian(reader.ReadUInt32());
                                break;
                            case 0x1: //Int32
                                value = Utils.swapEndian(reader.ReadUInt32());
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
                            case 0x3: //Boolean False
                                value = false;
                                break;
                            case 0x4: //Boolean True
                                value = true;
                                break;
                            case 0x5: //Nil
                                wasNil = true;
                                break;
                            case 0x6: //Actor (By Id)
                                value = Utils.swapEndian(reader.ReadUInt32());
                                break;
                            case 0x10: //Byte?
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

        public static List<LuaParam> createLuaParamList(params object[] list)
        {
            List<LuaParam> luaParams = new List<LuaParam>();

            foreach (object o in list)
            {
                if (o is uint)
                {
                    luaParams.Add(new LuaParam(0x0, (uint)o));
                }
                else if (o is string)
                {
                    luaParams.Add(new LuaParam(0x2, (string)o));
                }
                else if (o is bool)
                {
                    if (((bool)o))
                        luaParams.Add(new LuaParam(0x4, null));
                    else
                        luaParams.Add(new LuaParam(0x3, null));
                }
                else if (o == null)
                {
                    luaParams.Add(new LuaParam(0x5, null));
                }
                else if (o is Actor)
                {
                    luaParams.Add(new LuaParam(0x6, ((Actor)o).actorId));
                }
            }

            return luaParams;
        }        

        public static object[] createLuaParamObjectList(List <LuaParam> luaParams)
        {
            object[] list = new object[luaParams.Count];

            for (int i = 0; i < list.Length; i++)
                list[i] = luaParams[i].value;

            return list;
        }
    
      
        public static string dumpParams(List<LuaParam> lParams)
        {
            string dumpString = "";
            for (int i = 0; i < lParams.Count; i++)
            {
                switch (lParams[i].typeID)
                {
                    case 0x0: //Int32
                        dumpString += String.Format("0x{0:X}", (uint)lParams[i].value);
                        break;
                    case 0x1: //Int32
                        dumpString += String.Format("0x{0:X}", (uint)lParams[i].value);
                        break;
                    case 0x2: //Null Termed String                        
                        dumpString += String.Format("\"{0}\"", (string)lParams[i].value);
                        break;
                    case 0x3: //Boolean False
                        dumpString += "false";
                        break;
                    case 0x4: //Boolean True
                        dumpString += "true";
                        break;
                    case 0x5: //NULL???                        
                        dumpString += "nil";
                        break;
                    case 0x6: //Actor (By Id)
                        dumpString += String.Format("0x{0:X}", (uint)lParams[i].value);
                        break;
                    case 0x10: //Byte?
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

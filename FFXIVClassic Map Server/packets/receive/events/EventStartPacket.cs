using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                                Log.error("LUA ERROR:");                            

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

                        luaParams = LuaUtils.readLuaParams(binReader);
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

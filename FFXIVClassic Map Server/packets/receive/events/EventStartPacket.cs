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

        public string eventStarter;

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

                        List<byte> strList = new List<byte>();
                        byte curByte;
                        while ((curByte = binReader.ReadByte())!=0)
                        {
                            strList.Add(curByte);
                        }
                        eventStarter = Encoding.ASCII.GetString(strList.ToArray());

                        binReader.ReadUInt32();
                        binReader.ReadUInt32();
                        binReader.ReadUInt32();
                        binReader.ReadUInt32();

                        binReader.ReadByte();

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

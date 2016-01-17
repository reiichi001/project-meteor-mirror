using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.events
{
    class EventUpdatePacket
    {
        public const ushort OPCODE = 0x012E;
        public const uint PACKET_SIZE = 0x78;

        public bool invalidPacket = false;

        public uint actorID;
        public uint scriptOwnerActorID;
        public uint val1;
        public uint val2;
        public byte step;
        public List<LuaParam> luaParams;

        public EventUpdatePacket(byte[] data)
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
                        step = binReader.ReadByte();
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

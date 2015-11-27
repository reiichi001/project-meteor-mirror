using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.script
{
    class CommandStartRequestPacket
    {
        public const ushort OPCODE = 0x012E;
        public const uint PACKET_SIZE = 0x78;

        public bool invalidPacket = false;

        public uint actorID;
        public uint scriptOwnerActorID;
        public uint val1;
        public uint val2;
        public ScriptParamReader reader;

        public CommandStartRequestPacket(byte[] data)
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
                        binReader.ReadByte();

                        binReader.BaseStream.Seek(0x31, SeekOrigin.Begin);
                        reader = new ScriptParamReader(binReader);                      
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

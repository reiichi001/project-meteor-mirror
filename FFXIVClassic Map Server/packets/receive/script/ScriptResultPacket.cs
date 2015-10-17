using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.script
{
    class ScriptResultPacket
    {
        bool invalidPacket = false;

        public uint actorID;
        public uint val1;
        public uint val2;
        public uint val3;
        public uint val4;
        public uint val5;

        public ScriptResultPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        val1 = binReader.ReadUInt32();
                        val2 = binReader.ReadUInt32();
                        val3 = binReader.ReadUInt32();
                        val4 = binReader.ReadUInt32();
                        val5 = binReader.ReadUInt32();                        
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

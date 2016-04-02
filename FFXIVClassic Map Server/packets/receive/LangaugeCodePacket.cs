using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class LangaugeCodePacket
    {
        public bool invalidPacket = false;
        public uint languageCode;

        public LangaugeCodePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        binReader.ReadUInt32();
                        languageCode = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

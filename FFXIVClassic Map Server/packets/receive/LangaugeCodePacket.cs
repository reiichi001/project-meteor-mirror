using System;
using System.IO;

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

using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive.supportdesk
{
    class FaqListRequestPacket
    {
        public bool invalidPacket = false;
        public uint langCode;
        public uint unknown;

        public FaqListRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        langCode = binReader.ReadUInt32();
                        unknown = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

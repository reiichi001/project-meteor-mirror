using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive.supportdesk
{
    class FaqBodyRequestPacket
    {
        public bool invalidPacket = false;
        public uint faqIndex;
        public uint langCode;

        public FaqBodyRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        faqIndex = binReader.ReadUInt32();
                        langCode = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

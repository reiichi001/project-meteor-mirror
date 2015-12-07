using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.recruitment
{
    class RecruitmentDetailsRequestPacket
    {
        public bool invalidPacket = false;

        public ulong recruitmentId;

        public RecruitmentDetailsRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        recruitmentId = binReader.ReadUInt64();
                        
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

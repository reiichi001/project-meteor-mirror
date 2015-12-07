using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.recruitment
{
    class RecruitmentSearchRequestPacket
    {
        public bool invalidPacket = false;

        public uint purposeId;
        public uint locationId;

        public uint discipleId;
        public uint classjobId;

        public byte unknown1;
        public byte unknown2;
        
        public string text;

        public RecruitmentSearchRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        purposeId = binReader.ReadUInt32();
                        locationId = binReader.ReadUInt32();                       
                        discipleId = binReader.ReadUInt32();
                        classjobId = binReader.ReadUInt32();

                        unknown1 = binReader.ReadByte();
                        unknown2 = binReader.ReadByte();
                        
                        text = Encoding.ASCII.GetString(binReader.ReadBytes(0x20));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

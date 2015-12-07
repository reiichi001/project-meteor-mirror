using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.recruitment
{
    class StartRecruitingRequestPacket
    {
        public bool invalidPacket = false;

        public uint purposeId;
        public uint locationId;
        public uint subTaskId;

        public uint[] discipleId = new uint[4];
        public uint[] classjobId = new uint[4];
        public byte[] minLvl = new byte[4];
        public byte[] maxLvl = new byte[4];
        public byte[] num = new byte[4];

        public string comment;

        public StartRecruitingRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        purposeId = binReader.ReadUInt32();
                        locationId = binReader.ReadUInt32();
                        subTaskId = binReader.ReadUInt32();

                        for (int i = 0; i < 4; i++)
                        {
                            discipleId[i] = binReader.ReadUInt32();
                            classjobId[i] = binReader.ReadUInt32();
                            minLvl[i] = binReader.ReadByte();
                            maxLvl[i] = binReader.ReadByte();
                            num[i] = binReader.ReadByte();
                            binReader.ReadByte();
                        }

                        comment = Encoding.ASCII.GetString(binReader.ReadBytes(0x168));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

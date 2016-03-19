using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class ParameterDataRequestPacket
    {
        public const ushort OPCODE = 0x012F;
        public const uint PACKET_SIZE = 0x48;

        public bool invalidPacket = false;

        public uint actorID;
        public string paramName;
       
        public ParameterDataRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        List<byte> strList = new List<byte>();
                        byte curByte;
                        while ((curByte = binReader.ReadByte()) != 0 && strList.Count<=0x20)
                        {
                            strList.Add(curByte);
                        }
                        paramName = Encoding.ASCII.GetString(strList.ToArray());
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

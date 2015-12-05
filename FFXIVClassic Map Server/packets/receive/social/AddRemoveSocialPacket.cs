using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.social
{
    class AddRemoveSocialPacket
    {
        public bool invalidPacket = false;
        public string name;

        public AddRemoveSocialPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        name = Encoding.ASCII.GetString(binReader.ReadBytes(0x20));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

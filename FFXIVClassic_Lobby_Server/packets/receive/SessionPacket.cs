using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets.receive
{
    class SessionPacket
    {        
        public bool invalidPacket = false;
        public UInt64 sequence;
        public String session;
        public String version;   

        public SessionPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sequence = binReader.ReadUInt64();
                        binReader.ReadUInt32();
                        binReader.ReadUInt32();
                        session = Encoding.ASCII.GetString(binReader.ReadBytes(0x40)).Trim(new[] { '\0' });
                        version = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });                       
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

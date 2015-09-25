using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class HandshakePacket
    {
        bool invalidPacket = false;

        public uint actorID;

        public HandshakePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        binReader.BaseStream.Seek(4, SeekOrigin.Begin);
                        actorID = UInt32.Parse(Encoding.ASCII.GetString(binReader.ReadBytes(10)));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

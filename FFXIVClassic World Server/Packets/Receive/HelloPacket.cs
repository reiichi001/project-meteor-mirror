using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.Receive
{
    class HelloPacket
    {
        public bool invalidPacket = false;
        public uint sessionId;

        public HelloPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        byte[] readIn = new byte[12];
                        binReader.BaseStream.Seek(0x14, SeekOrigin.Begin);
                        binReader.Read(readIn, 0, 12);
                        sessionId = UInt32.Parse(Encoding.ASCII.GetString(readIn));
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class _0x07Packet
    {
        public bool invalidPacket = false;
        public uint timestamp;
        public uint unknown;

        public _0x07Packet(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        timestamp = binReader.ReadUInt32();
                        unknown = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

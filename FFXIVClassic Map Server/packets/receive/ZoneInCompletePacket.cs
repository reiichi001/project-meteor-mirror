using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class ZoneInCompletePacket
    {
        public bool invalidPacket = false;
        public uint timestamp;
        public int unknown;

        public ZoneInCompletePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        timestamp = binReader.ReadUInt32();
                        unknown = binReader.ReadInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

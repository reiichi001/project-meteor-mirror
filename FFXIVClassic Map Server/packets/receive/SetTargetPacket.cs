using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class SetTargetPacket
    {
        public bool invalidPacket = false;
        public uint actorID;
        public uint otherVal; //Usually 0xE0000000

        public SetTargetPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        otherVal = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class LockTargetPacket
    {
        public bool invalidPacket = false;
        public uint actorID;
        public uint otherVal; //Camera related?

        public LockTargetPacket(byte[] data)
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

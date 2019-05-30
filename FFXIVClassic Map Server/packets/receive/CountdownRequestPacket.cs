using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class CountdownRequestPacket
    {
        public bool invalidPacket = false;
        public byte countdownLength;
        public ulong syncTime;

        public CountdownRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        countdownLength = binReader.ReadByte();
                        binReader.BaseStream.Seek(8, SeekOrigin.Begin);
                        syncTime = binReader.ReadUInt64();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

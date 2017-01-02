using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.Receive.Subpackets
{
    class PartyChatMessagePacket
    {
        public uint actorId;
        public string message;

        public bool invalidPacket = false;

        public PartyChatMessagePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorId = binReader.ReadUInt32();
                        message = Encoding.ASCII.GetString(binReader.ReadBytes(0x200)).Trim(new [] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class DeleteGroupPacket
    {
        public bool invalidPacket = false;    
        public ulong groupId;
        
        public DeleteGroupPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        groupId = binReader.ReadUInt64();                        
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

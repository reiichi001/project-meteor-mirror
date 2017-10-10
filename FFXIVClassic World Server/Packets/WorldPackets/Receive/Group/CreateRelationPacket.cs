using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class CreateRelationPacket
    {
        public bool invalidPacket = false;

        public uint host;
        public uint guest;
        public uint command;
        
        public CreateRelationPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        host = binReader.ReadUInt32();
                        guest = binReader.ReadUInt32();
                        command = binReader.ReadUInt32();
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

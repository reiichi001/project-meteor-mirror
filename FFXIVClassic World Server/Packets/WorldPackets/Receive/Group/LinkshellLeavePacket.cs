using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class LinkshellLeavePacket
    {
        public bool invalidPacket = false;
                
        public bool isDisband;
        public string lsName;

        public LinkshellLeavePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        isDisband = binReader.ReadUInt16() == 1;
                        lsName = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
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

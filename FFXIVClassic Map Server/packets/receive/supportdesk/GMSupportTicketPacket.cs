using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive.supportdesk
{
    class GMSupportTicketPacket
    {
        public bool invalidPacket = false;
        public string ticketTitle, ticketBody;
        public uint ticketIssueIndex;
        public uint langCode;

        public GMSupportTicketPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        langCode = binReader.ReadUInt32();
                        ticketIssueIndex = binReader.ReadUInt32();
                        ticketTitle = Encoding.ASCII.GetString(binReader.ReadBytes(0x80)).Trim(new[] { '\0' });
                        ticketBody = Encoding.ASCII.GetString(binReader.ReadBytes(0x800)).Trim(new[] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

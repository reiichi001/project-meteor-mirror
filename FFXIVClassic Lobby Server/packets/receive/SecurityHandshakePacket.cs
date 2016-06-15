using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Lobby_Server.packets.receive
{
    class SecurityHandshakePacket
    {
        public string ticketPhrase;
        public uint clientNumber;

        public bool invalidPacket = false;

        public SecurityHandshakePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        binReader.BaseStream.Seek(0x34, SeekOrigin.Begin);
                        ticketPhrase = Encoding.ASCII.GetString(binReader.ReadBytes(0x40)).Trim(new[] { '\0' });
                        clientNumber = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets
{
    class SelectCharacterConfirmPacket
    {
        public const ushort OPCODE = 0x0F;

        private UInt64 sequence;
        private UInt32 characterId;
        private string sessionToken;
        private string worldIp;
        private UInt16 worldPort;
        private UInt64 selectCharTicket;

        public SelectCharacterConfirmPacket(UInt64 sequence, UInt32 characterId, string sessionToken, string worldIp, ushort worldPort, UInt64 selectCharTicket)
        {
            this.sequence = sequence;
            this.characterId = characterId;
            this.sessionToken = sessionToken;
            this.worldIp = worldIp;
            this.worldPort = worldPort;
            this.selectCharTicket = selectCharTicket;            
        }        

        public List<SubPacket> buildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            byte[] data;
            
            using (MemoryStream memStream = new MemoryStream(0x98))
            {
                using (BinaryWriter binWriter = new BinaryWriter(memStream))
                {
                    binWriter.Write((UInt64)sequence);
                    binWriter.Write((UInt32)characterId); //ActorId
                    binWriter.Write((UInt32)characterId); //CharacterId
                    binWriter.Write((UInt32)0);
                    binWriter.Write(Encoding.ASCII.GetBytes(sessionToken.PadRight(0x42, '\0'))); //Session Token
                    binWriter.Write((UInt16)worldPort); //World Port
                    binWriter.Write(Encoding.ASCII.GetBytes(worldIp.PadRight(0x38, '\0'))); //World Hostname/IP
                    binWriter.Write((UInt64)selectCharTicket); //Ticket or Handshake of somekind
                }
                data = memStream.GetBuffer();
            }
            
            SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, 0xe0006868, data);
            subPackets.Add(subpacket);
            
            return subPackets;
        }
    }
}

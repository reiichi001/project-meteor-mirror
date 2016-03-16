using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets.receive
{
    class SelectCharacterPacket
    {
        public UInt64 sequence;
        public uint characterId;
        public uint unknownId;
        public UInt64 ticket;

        public bool invalidPacket = false;

        public SelectCharacterPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sequence = binReader.ReadUInt64();
                        characterId = binReader.ReadUInt32();
                        unknownId = binReader.ReadUInt32();
                        ticket = binReader.ReadUInt64();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

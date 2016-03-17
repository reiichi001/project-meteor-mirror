using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.packets.receive
{
    class CharacterModifyPacket
    {
        public UInt64 sequence;
        public uint characterId;
        public uint personType;
        public byte slot;
        public byte command;
        public ushort worldId;
        public String characterName;
        public String characterInfoEncoded;

        public bool invalidPacket = false;

        public CharacterModifyPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sequence = binReader.ReadUInt64();
                        characterId = binReader.ReadUInt32();
                        personType = binReader.ReadUInt32();
                        slot = binReader.ReadByte();
                        command = binReader.ReadByte();
                        worldId = binReader.ReadUInt16();

                        characterName = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                        characterInfoEncoded = Encoding.ASCII.GetString(binReader.ReadBytes(0x190)).Trim(new[] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

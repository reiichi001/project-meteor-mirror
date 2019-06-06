using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.Receive.Subpackets
{
    class GroupCreatedPacket
    {    
        public ulong groupId;
        public string workString;

        public bool invalidPacket = false;

        public GroupCreatedPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        groupId = binReader.ReadUInt64();
                        workString = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

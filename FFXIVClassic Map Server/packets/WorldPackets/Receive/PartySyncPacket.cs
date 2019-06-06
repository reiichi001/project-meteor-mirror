using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Receive
{
    class PartySyncPacket
    {
        public ulong partyGroupId;
        public uint owner;
        public uint[] memberActorIds;

        public bool invalidPacket = false;

        public PartySyncPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        partyGroupId = binReader.ReadUInt64();
                        owner = binReader.ReadUInt32();
                        uint numMembers = binReader.ReadUInt32();
                        memberActorIds = new uint[numMembers];
                        
                        for (int i = 0; i < numMembers; i++)                        
                            memberActorIds[i] = binReader.ReadUInt32();                        
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

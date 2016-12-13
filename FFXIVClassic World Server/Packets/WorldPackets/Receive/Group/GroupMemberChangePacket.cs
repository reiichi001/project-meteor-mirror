using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class GroupMemberChangePacket
    {
        public const int GROUP_MEMBER_ADD = 0;
        public const int GROUP_MEMBER_REMOVE = 0;

        public bool invalidPacket = false;
        public uint controlCode;
        public ulong groupId;
        public uint memberId;

        public GroupMemberChangePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        controlCode = binReader.ReadUInt32();
                        groupId = binReader.ReadUInt64();
                        memberId = binReader.ReadUInt32();
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

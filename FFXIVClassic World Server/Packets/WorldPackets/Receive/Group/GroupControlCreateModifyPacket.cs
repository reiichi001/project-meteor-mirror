using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class GroupControlCreateModifyPacket
    {
        public const byte GROUP_CONTROL_CREATE = 0;
        public const byte GROUP_CONTROL_MODIFY = 1;

        public const byte GROUP_PARTY = 0;
        public const byte GROUP_RETAINER = 1;
        public const byte GROUP_LINKSHELL = 2;
        public const byte GROUP_RELATION = 3;

        public bool invalidPacket = false;
        public byte controlCode;        
        public ulong groupId;
        public byte groupType;
        
        public GroupControlCreateModifyPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        controlCode = binReader.ReadByte();
                        groupType = binReader.ReadByte();
                        groupId = binReader.ReadUInt64();
                        
                        //Work value data
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

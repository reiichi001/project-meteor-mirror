using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class GroupControlGetDeletePacket
    {
        public const byte GROUP_CONTROL_GET = 0;
        public const byte GROUP_CONTROL_DELETE = 1;
        
        public bool invalidPacket = false;
        public uint controlCode;        
        public ulong groupId;
        
        public GroupControlGetDeletePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        controlCode = binReader.ReadUInt32();
                        groupId = binReader.ReadUInt64();                        
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

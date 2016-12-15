using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Actor.Group;
using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups
{
    class CreateNamedGroup
    {
        public const ushort OPCODE = 0x0188;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket buildPacket(uint sessionId, Group group)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)group.groupIndex);
                    binWriter.Write((UInt32)group.GetTypeId());
                    binWriter.Write((Int32)group.GetGroupLocalizedName());

                    binWriter.Write((UInt16)0x121C);

                    binWriter.Seek(0x20, SeekOrigin.Begin);

                    binWriter.Write(Encoding.ASCII.GetBytes(group.GetGroupName()), 0, Encoding.ASCII.GetByteCount(group.GetGroupName()) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(group.GetGroupName()));                    
                }
            }

            return new SubPacket(OPCODE, sessionId, sessionId, data);
        }
    }
}

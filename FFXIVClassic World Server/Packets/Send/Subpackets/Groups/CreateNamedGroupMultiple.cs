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
    class CreateNamedGroupMultiple
    {
        public const ushort OPCODE = 0x0189;
        public const uint PACKET_SIZE = 0x228;

        public static SubPacket buildPacket(uint playerActorID, Group[] groups, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max = 8;
                    if (groups.Length - offset <= 8)
                        max = groups.Length - offset;

                    for (int i = 0; i < max; i++)
                    {                        
                        binWriter.Seek(i * 0x40, SeekOrigin.Begin);

                        Group group = groups[offset+i];

                        binWriter.Write((UInt64)group.groupIndex);
                        binWriter.Write((UInt32)group.GetTypeId());
                        binWriter.Write((Int32)group.GetGroupLocalizedName());

                        binWriter.Write((UInt16)0x121C);

                        binWriter.Seek(0x20, SeekOrigin.Begin);

                        binWriter.Write(Encoding.ASCII.GetBytes(group.GetGroupName()), 0, Encoding.ASCII.GetByteCount(group.GetGroupName()) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(group.GetGroupName()));                    
                    }

                    binWriter.Seek(0x200, SeekOrigin.Begin);
                    binWriter.Write((Byte)max);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

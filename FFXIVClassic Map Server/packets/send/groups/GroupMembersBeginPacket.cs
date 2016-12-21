using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.group
{
    class GroupMembersBeginPacket
    {
        public const ushort OPCODE = 0x017D;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, Group group)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write List Header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);
                    //Write List Info
                    binWriter.Write((UInt64)group.groupIndex);
                    binWriter.Write((UInt32)group.GetMemberCount());
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

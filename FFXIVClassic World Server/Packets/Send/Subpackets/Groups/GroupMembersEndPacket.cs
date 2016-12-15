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
    class GroupMembersEndPacket
    {
        public const ushort OPCODE = 0x017E;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket buildPacket(uint sessionId, uint locationCode, ulong sequenceId, Group group)
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
                }
            }

            return new SubPacket(OPCODE, sessionId, sessionId, data);
        }

    }
}

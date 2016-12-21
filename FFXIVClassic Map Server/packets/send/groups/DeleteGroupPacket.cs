using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.groups
{
    class DeleteGroupPacket
    {
        public const ushort OPCODE = 0x0143;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint playerActorID, Group group)
        {
            return buildPacket(playerActorID, group.groupIndex);
        }

        public static SubPacket buildPacket(uint playerActorID, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write control num ????
                    binWriter.Write((UInt64)3);

                    //Write Ids
                    binWriter.Write((UInt64)groupId);
                    binWriter.Write((UInt64)0);
                    binWriter.Write((UInt64)groupId);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

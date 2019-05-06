using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.group
{
    class ContentMembersX08Packet
    {
        public const ushort OPCODE = 0x0183;
        public const uint PACKET_SIZE = 0x1B8;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, List<GroupMember> entries, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write List Header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);
                    //Write Entries
                    int max = 8;
                    if (entries.Count-offset < 8)
                        max = entries.Count - offset;
                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Seek(0x10 + (0xC * i), SeekOrigin.Begin);

                        GroupMember entry = entries[i];
                        binWriter.Write((UInt32)entry.actorId);
                        binWriter.Write((UInt32)1001); //Layout ID
                        binWriter.Write((UInt32)1); //?

                        offset++;
                    }
                    //Write Count
                    binWriter.Seek(0x10 + (0xC * 8), SeekOrigin.Begin);
                    binWriter.Write(max);
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}

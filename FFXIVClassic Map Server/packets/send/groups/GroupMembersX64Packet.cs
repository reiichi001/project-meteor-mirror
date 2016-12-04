using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.group
{
    class GroupMembersX64Packet
    {
        public const ushort OPCODE = 0x0182;
        public const uint PACKET_SIZE = 0xC30;

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
                    int max = 64;
                    if (entries.Count - offset < 64)
                        max = entries.Count - offset;
                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Seek(0x10 + (0x30 * i), SeekOrigin.Begin);

                        GroupMember entry = entries[i];
                        binWriter.Write((UInt32)entry.actorId);
                        binWriter.Write((Int32)entry.localizedName);
                        binWriter.Write((UInt32)entry.unknown2);
                        binWriter.Write((Byte)(entry.flag1? 1 : 0));
                        binWriter.Write((Byte)(entry.isOnline? 1 : 0));
                        binWriter.Write(Encoding.ASCII.GetBytes(entry.name), 0, Encoding.ASCII.GetByteCount(entry.name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(entry.name));

                        offset++;
                    }                   
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

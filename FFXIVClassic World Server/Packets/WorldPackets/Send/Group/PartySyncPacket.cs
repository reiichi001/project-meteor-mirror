using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send.Group
{
    class PartySyncPacket
    {
        public const ushort OPCODE = 0x1020;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket BuildPacket(Session session, Party party)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)party.groupIndex);
                    binWriter.Write((UInt32)party.GetLeader());
                    binWriter.Write((UInt32)party.members.Count);
                    for (int i = 0; i < party.members.Count; i++)
                        binWriter.Write((UInt32)party.members[i]);
                }
            }

            return new SubPacket(true, OPCODE, 0, session.sessionId, data);
        }
    }
}

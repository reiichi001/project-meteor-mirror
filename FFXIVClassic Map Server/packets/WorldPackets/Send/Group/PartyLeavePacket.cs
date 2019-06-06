using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class PartyLeavePacket
    {
        public const ushort OPCODE = 0x1021;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(Session session, bool isDisband)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)(isDisband ? 1 : 0));
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }

    }
}

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class LinkshellLeavePacket
    {
        public const ushort OPCODE = 0x1031;
        public const uint PACKET_SIZE = 0x68;

        public static SubPacket BuildPacket(Session session, string lsName, string kickedName, bool isKicked)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)(isKicked ? 1 : 0));
                    if (kickedName != null && isKicked)
                        binWriter.Write(Encoding.ASCII.GetBytes(kickedName), 0, Encoding.ASCII.GetByteCount(kickedName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(kickedName));
                    binWriter.Seek(0x22, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(lsName), 0, Encoding.ASCII.GetByteCount(lsName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(lsName));
                }
            }
            return new SubPacket(true, OPCODE, session.id, session.id, data);
        }

    }
}

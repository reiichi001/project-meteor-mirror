using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class LinkshellRankChangePacket
    {
        public const ushort OPCODE = 0x1032;
        public const uint PACKET_SIZE = 0x68;

        public static SubPacket BuildPacket(Session session, string name, string lsName, byte rank)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));
                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(lsName), 0, Encoding.ASCII.GetByteCount(lsName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(lsName));
                    binWriter.Seek(0x40, SeekOrigin.Begin);
                    binWriter.Write((Byte)rank);
                }
            }
            return new SubPacket(true, OPCODE, session.id, session.id, data);
        }

    }
}

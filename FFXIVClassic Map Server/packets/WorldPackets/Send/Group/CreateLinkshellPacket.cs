using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class CreateLinkshellPacket
    {
        public const ushort OPCODE = 0x1025;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(Session session, string name, ushort crest, uint master)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));
                    binWriter.BaseStream.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt16)crest);
                    binWriter.Write((UInt32)master);
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }      
    }
}

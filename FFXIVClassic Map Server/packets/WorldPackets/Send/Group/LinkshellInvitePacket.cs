using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class LinkshellInvitePacket
    {
        public const ushort OPCODE = 0x1029;
        public const uint PACKET_SIZE = 0x48;       

        public static SubPacket BuildPacket(Session session, uint actorId, string linkshellName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)actorId);
                    binWriter.Write(Encoding.ASCII.GetBytes(linkshellName), 0, Encoding.ASCII.GetByteCount(linkshellName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(linkshellName));
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }      
    }
}

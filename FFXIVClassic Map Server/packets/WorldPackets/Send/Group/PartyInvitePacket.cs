using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class PartyInvitePacket
    {
        public const ushort OPCODE = 0x1022;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(Session session, string name)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)0);
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));
                }
            }
            return new SubPacket(true, OPCODE, session.id, session.id, data);
        }

        public static SubPacket BuildPacket(Session session, uint actorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt16)1);
                    binWriter.Write((UInt32)actorId);
                }
            }
            return new SubPacket(true, OPCODE, session.id, session.id, data);
        }      
    }
}

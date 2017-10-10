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
    class LinkshellInviteCancelPacket
    {
        public const ushort OPCODE = 0x1030;
        public const uint PACKET_SIZE = 0x28;       

        public static SubPacket BuildPacket(Session session)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];            
            return new SubPacket(true, OPCODE, session.id, data);
        }      
    }
}

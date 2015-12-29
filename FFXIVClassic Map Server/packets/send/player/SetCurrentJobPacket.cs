using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCurrentJobPacket
    {
        public const ushort OPCODE = 0x01A4;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint sourceActorID, uint targetActorID, uint jobId)
        {
            return new SubPacket(OPCODE, sourceActorID, targetActorID, BitConverter.GetBytes((uint)jobId));
        }
    }
}

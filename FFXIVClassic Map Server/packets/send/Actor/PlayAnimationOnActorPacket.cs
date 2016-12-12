using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class PlayAnimationOnActorPacket
    {
        public const ushort OPCODE = 0x00DA;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint targetActorID, uint animationID)
        {
            return new SubPacket(OPCODE, playerActorID, targetActorID, BitConverter.GetBytes((ulong)animationID));
        }
    }
}

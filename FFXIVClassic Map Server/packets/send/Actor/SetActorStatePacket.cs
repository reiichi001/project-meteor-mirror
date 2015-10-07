using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorStatePacket
    {
        public const ushort OPCODE = 0x0134;
        public const uint PACKET_SIZE = 0x28;

        public const int STATE_NONE     = 0x0000;
		public const int STATE_DEAD		= 0x0303;
		public const int STATE_PASSIVE	= 0xBF00;
		public const int STATE_ACTIVE	= 0xBF02;

        public static SubPacket buildPacket(uint playerActorID, uint targetID, uint state)
        {
            ulong combined = 0;

            combined |= state;

            return new SubPacket(OPCODE, playerActorID, targetID, BitConverter.GetBytes(combined));
        }
    }
}

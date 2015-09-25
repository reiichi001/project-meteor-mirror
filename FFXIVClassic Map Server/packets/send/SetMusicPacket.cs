using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetMusicPacket
    {
        public const ushort OPCODE = 0x000C;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint musicID, uint musicTrackMode)
        {
            ulong combined = musicID | (musicTrackMode << 32);
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(combined));
        }
    }
}

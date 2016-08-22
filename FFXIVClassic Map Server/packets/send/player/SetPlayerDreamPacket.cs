using FFXIVClassic.Common;
using System;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerDreamPacket
    {
        public const ushort OPCODE = 0x01A7;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint dreamID)
        {
            dreamID += 0x20E;
            return new SubPacket(OPCODE, playerActorID, playerActorID, BitConverter.GetBytes((uint)dreamID));
        }
    }
}

using FFXIVClassic.Common;
using System;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetPlayerDreamPacket
    {
        public const ushort OPCODE = 0x01A7;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, uint dreamID)
        {
            dreamID += 0x20E;
            return new SubPacket(OPCODE, sourceActorId, BitConverter.GetBytes((uint)dreamID));
        }
    }
}

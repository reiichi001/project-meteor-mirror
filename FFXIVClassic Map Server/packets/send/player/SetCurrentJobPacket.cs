using System;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCurrentJobPacket
    {
        public const ushort OPCODE = 0x01A4;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorID, uint targetActorID, uint jobId)
        {
            return new SubPacket(OPCODE, sourceActorID, targetActorID, BitConverter.GetBytes((uint)jobId));
        }
    }
}

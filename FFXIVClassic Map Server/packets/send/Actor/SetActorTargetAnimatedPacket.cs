using System;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorTargetAnimatedPacket
    {
        public const ushort OPCODE = 0x00D3;
        public const uint PACKET_SIZE = 0x28;
        
        public static SubPacket BuildPacket(uint sourceActorId, uint targetID)
        {            
            return new SubPacket(OPCODE, sourceActorId, BitConverter.GetBytes((ulong)targetID));
        }
    }
}

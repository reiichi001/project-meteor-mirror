using System;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorTargetAnimatedPacket
    {
        public const ushort OPCODE = 0x00D3;
        public const uint PACKET_SIZE = 0x28;
        
        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, uint tarGetID)
        {            
            return new SubPacket(OPCODE, playerActorID, tarGetID, BitConverter.GetBytes((ulong)tarGetID));
        }
    }
}

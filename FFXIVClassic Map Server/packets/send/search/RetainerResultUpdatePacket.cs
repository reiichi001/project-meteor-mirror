using System.IO;
using System.Text;
using System;
using FFXIVClassic.Common;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.packets.send.search
{
    class RetainerResultUpdatePacket
    {
        public const ushort OPCODE = 0x01DC;
        public const uint PACKET_SIZE = 0x028;

        public static SubPacket BuildPacket(uint sourceActorId, bool isSuccess, string nameToAdd)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

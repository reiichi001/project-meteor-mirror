using System;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class ActorDoEmotePacket
    {
        public const ushort OPCODE = 0x00E1;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint sourceActorId, uint targettedActorId, uint animationId, uint descriptionId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            if (targettedActorId == 0)
            {
                targettedActorId = sourceActorId;
                if (descriptionId != 10105)
                    descriptionId++;
            }

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    uint realAnimID = 0x5000000 | (animationId << 12);                    
                    binWriter.Write((UInt32)realAnimID);
                    binWriter.Write((UInt32)targettedActorId);
                    binWriter.Write((UInt32)descriptionId);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, data);
            packet.DebugPrintSubPacket();
            return packet;
        }
    }
}

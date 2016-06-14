using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class ActorDoEmotePacket
    {
        public const ushort OPCODE = 0x00E1;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint sourceActorId, uint tarGetActorId, uint tarGettedActorId, uint emoteID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            if (tarGettedActorId == 0xC0000000)
                tarGettedActorId = sourceActorId;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    uint realAnimID = 0x5000000 | ((emoteID - 100) << 12);
                    uint realDescID = 20000 + ((emoteID - 1) * 10) + (tarGettedActorId == sourceActorId ? (uint)2 : (uint)1);
                    binWriter.Write((UInt32)realAnimID);
                    binWriter.Write((UInt32)tarGettedActorId);
                    binWriter.Write((UInt32)realDescID);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, tarGetActorId, data);
            packet.DebugPrintSubPacket();
            return packet;
        }
    }
}

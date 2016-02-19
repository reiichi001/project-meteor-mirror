using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class ActorDoEmotePacket
    {
        public const ushort OPCODE = 0x00E1;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket buildPacket(uint sourceActorId, uint targetActorId, uint targettedActorId, uint emoteID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            if (targettedActorId == 0xC0000000)
                targettedActorId = sourceActorId;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    uint realAnimID = 0x5000000 | ((emoteID - 100) << 12);
                    uint realDescID = 20000 + ((emoteID - 1) * 10) + (targettedActorId == sourceActorId ? (uint)2 : (uint)1);
                    binWriter.Write((UInt32)realAnimID);
                    binWriter.Write((UInt32)targettedActorId);
                    binWriter.Write((UInt32)realDescID);
                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, targetActorId, data);
            packet.debugPrintSubPacket();
            return packet;
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class MoveActorToPositionPacket
    {
        public const ushort OPCODE = 0x00CF;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket buildPacket(uint playerActorID, uint actorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {

                }
                data = mem.GetBuffer();
            }

            SubPacket packet = new SubPacket(OPCODE, playerActorID, actorID, data);
            return packet;
        }

    }
}

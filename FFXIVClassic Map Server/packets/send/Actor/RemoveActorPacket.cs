using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class RemoveActorPacket
    {
        public const ushort OPCODE = 0x00CB;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint actorId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)actorId);                 
                }
            }

            return new SubPacket(OPCODE, actorId, playerActorID, data);
        }

    }
}

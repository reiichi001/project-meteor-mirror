using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorStatusAllPacket
    {
        public const ushort OPCODE = 0x0179;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, ushort[] statusIds)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
          
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < statusIds.Length; i++)
                    {
                        if (i >= 20)
                            break;
                        binWriter.Write((UInt16)statusIds[i]);
                    }
                }
            }

            SubPacket packet = new SubPacket(OPCODE, playerActorID, targetActorID, data);
            return packet;
        }
    }
}

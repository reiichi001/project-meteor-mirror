using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class PongPacket
    {
        public const ushort OPCODE = 0x0001;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint playerActorID, uint pingTicks)
        {          
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using(BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)pingTicks);
                    binWriter.Write((UInt32)0x14D);
                }
            }

            SubPacket subpacket = new SubPacket(OPCODE, playerActorID, playerActorID, data);
            return subpacket;
        }

    }
}

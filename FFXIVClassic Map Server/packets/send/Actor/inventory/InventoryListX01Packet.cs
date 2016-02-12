using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX01Packet
    {
        public const ushort OPCODE = 0x0148;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket buildPacket(uint playerActorID, Item item)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                        binWriter.Write(item.toPacketBytes());                       
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }

    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class InventorySetBeginPacket
    {
        public const ushort OPCODE = 0x0146;
        public const uint PACKET_SIZE = 0x28;

        public const ushort CODE_ITEMS = 0x0000;
        public const ushort CODE_CURRANCYITEMS = 0x0063;
        public const ushort CODE_KEYITEMS = 0x0064;
        public const ushort CODE_EQUIPMENT = 0x00FE;        

        public static SubPacket buildPacket(uint playerActorID, ushort size, ushort code)
        {
            byte[] data = new byte[8];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)playerActorID);
                    binWriter.Write((UInt16)size);
                    binWriter.Write((UInt16)code);
                }                
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }

    }
}

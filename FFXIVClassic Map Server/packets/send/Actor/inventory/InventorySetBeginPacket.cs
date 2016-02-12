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

        public const ushort CODE_INVENTORY      = 0x0000; //Max 0xC8
        public const ushort CODE_LOOT           = 0x0004; //Max 0xA
        public const ushort CODE_MELDREQUEST    = 0x0005; //Max 0x04
        public const ushort CODE_BAZAAR         = 0x0007; //Max 0x0A
        public const ushort CODE_CURRANCY       = 0x0063; //Max 0x140
        public const ushort CODE_KEYITEMS       = 0x0064; //Max 0x500
        public const ushort CODE_EQUIPMENT      = 0x00FE; //Max 0x23

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

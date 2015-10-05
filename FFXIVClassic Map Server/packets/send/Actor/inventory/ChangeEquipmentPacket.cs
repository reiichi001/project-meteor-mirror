using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor.inventory
{
    class ChangeEquipmentPacket
    {
        public const ushort OPCODE      = 0x014D;
        public const uint   PACKET_SIZE = 0x28;

        public const UInt16 SLOT_MAINHAND			    = 0x00;
		public const UInt16 SLOT_OFFHAND			    = 0x01;
		public const UInt16 SLOT_THROWINGWEAPON		= 0x04;
		public const UInt16 SLOT_PACK				    = 0x05;
		public const UInt16 SLOT_POUCH				    = 0x06;
		public const UInt16 SLOT_HEAD				    = 0x08;
		public const UInt16 SLOT_UNDERSHIRT			= 0x09;
		public const UInt16 SLOT_BODY				    = 0x0A;
		public const UInt16 SLOT_UNDERGARMENT		    = 0x0B;
		public const UInt16 SLOT_LEGS				    = 0x0C;
		public const UInt16 SLOT_HANDS				    = 0x0D;
		public const UInt16 SLOT_BOOTS				    = 0x0E;
		public const UInt16 SLOT_WAIST				    = 0x0F;
		public const UInt16 SLOT_NECK				    = 0x10;
		public const UInt16 SLOT_EARS				    = 0x11;
		public const UInt16 SLOT_WRISTS				= 0x13;
		public const UInt16 SLOT_RIGHTFINGER		    = 0x15;
		public const UInt16 SLOT_LEFTFINGER			= 0x16;


        public static SubPacket buildPacket(uint playerActorID, ushort equipSlot, uint inventorySlot)
        {
            ulong combined = equipSlot | (inventorySlot << 32);
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(combined));
        }

    }
}

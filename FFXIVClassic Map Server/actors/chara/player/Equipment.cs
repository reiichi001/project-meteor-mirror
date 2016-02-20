using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.player
{
    class Equipment
    {
        public const int SLOT_MAINHAND = 0x00;
        public const int SLOT_OFFHAND = 0x01;
        public const int SLOT_THROWINGWEAPON = 0x04;
        public const int SLOT_PACK = 0x05;
        public const int SLOT_POUCH = 0x06;
        public const int SLOT_HEAD = 0x08;
        public const int SLOT_UNDERSHIRT = 0x09;
        public const int SLOT_BODY = 0x0A;
        public const int SLOT_UNDERGARMENT = 0x0B;
        public const int SLOT_LEGS = 0x0C;
        public const int SLOT_HANDS = 0x0D;
        public const int SLOT_BOOTS = 0x0E;
        public const int SLOT_WAIST = 0x0F;
        public const int SLOT_NECK = 0x10;
        public const int SLOT_EARS = 0x11;
        public const int SLOT_WRISTS = 0x13;
        public const int SLOT_RIGHTFINGER = 0x15;
        public const int SLOT_LEFTFINGER = 0x16;

        private Player owner;
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private Item[] list;

        public Equipment(Player ownerPlayer, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
        }

        public Item GetItemAtSlot(ushort slot)
        {
            if (slot < list.Length)
                return list[slot];
            else
                return null;
        }

        public void SetEquipment(List<Tuple<ushort, Item>> toEquip)
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            for (int i = 0; i < toEquip.Count; i++)
            {
                slotsToUpdate.Add(toEquip[i].Item1);
                list[toEquip[i].Item1] = toEquip[i].Item2;
            }

            SendEquipmentPackets(slotsToUpdate);            
        }

        public void Equip(ushort slot, Item item)
        {
            if (slot < list.Length)
                list[slot] = item;
            SendEquipmentPackets(slot, item);
        }

        public void Unequip(ushort slot)
        {
            if (slot < list.Length)
                list[slot] = null;
            SendEquipmentPackets(slot, null);
        }

        private void SendEquipmentPackets(ushort equipSlot, Item item)
        {
            if (item == null)
                owner.queuePacket(EquipmentListX01Packet.buildPacket(owner.actorId, equipSlot, 0));
            else
                owner.queuePacket(EquipmentListX01Packet.buildPacket(owner.actorId, equipSlot, item.slot));
        }

        private void SendEquipmentPackets(List<ushort> slotsToUpdate)
        {
            int currentIndex = 0;

            while (true)
            {
                if (list.Length - currentIndex >= 64)
                    owner.queuePacket(EquipmentListX64Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (list.Length - currentIndex >= 32)
                    owner.queuePacket(EquipmentListX32Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (list.Length - currentIndex >= 16)
                    owner.queuePacket(EquipmentListX16Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (list.Length - currentIndex > 1)
                    owner.queuePacket(EquipmentListX08Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (list.Length - currentIndex == 1)
                {
                    owner.queuePacket(EquipmentListX01Packet.buildPacket(owner.actorId, slotsToUpdate[currentIndex], list[currentIndex].slot));
                    currentIndex++;
                }
                else
                    break;
            }

        }

    }
}

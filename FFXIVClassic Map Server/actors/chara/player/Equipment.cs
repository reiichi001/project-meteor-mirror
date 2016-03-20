using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
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
        public const int SLOT_MAINHAND = 0;
        public const int SLOT_OFFHAND = 1;
        public const int SLOT_THROWINGWEAPON = 4;
        public const int SLOT_PACK = 5;
        public const int SLOT_POUCH = 6;
        public const int SLOT_HEAD = 8;
        public const int SLOT_UNDERSHIRT = 9;
        public const int SLOT_BODY = 10;
        public const int SLOT_UNDERGARMENT = 11;
        public const int SLOT_LEGS = 12;
        public const int SLOT_HANDS = 13;
        public const int SLOT_BOOTS = 14;
        public const int SLOT_WAIST = 15;
        public const int SLOT_NECK = 16;
        public const int SLOT_EARS = 17;
        public const int SLOT_WRISTS = 19;
        public const int SLOT_RIGHTFINGER = 21;
        public const int SLOT_LEFTFINGER = 22;

        private Player owner;
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private InventoryItem[] list;
        private Inventory normalInventory;

        private bool writeToDB = true;

        public Equipment(Player ownerPlayer, Inventory normalInventory, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
            list = new InventoryItem[inventoryCapacity];
            this.normalInventory = normalInventory;
        }

        public InventoryItem GetItemAtSlot(ushort slot)
        {
            if (slot < list.Length)
                return list[slot];
            else
                return null;
        }

        public void SendCheckEquipmentToPlayer(Player toPlayer)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            for (ushort i = 0; i < list.Length; i++)
            {
                if (list[i] != null)
                {
                    InventoryItem equipItem = new InventoryItem(list[i], i);
                    items.Add(equipItem);
                }
            }

            toPlayer.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, toPlayer.actorId, 0x23, Inventory.EQUIPMENT_OTHERPLAYER));
            int currentIndex = 0;

            while (true)
            {
                if (items.Count - currentIndex >= 16)
                    toPlayer.queuePacket(InventoryListX16Packet.buildPacket(owner.actorId, toPlayer.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex > 1)
                    toPlayer.queuePacket(InventoryListX08Packet.buildPacket(owner.actorId, toPlayer.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex == 1)
                {
                    toPlayer.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, toPlayer.actorId, items[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }
            toPlayer.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId, toPlayer.actorId));
        }

        public void SendFullEquipment(bool doClear)
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            for (ushort i = 0; i < list.Length; i++)
            {
                if (list[i] == null && doClear)
                    slotsToUpdate.Add(0);
                else if (list[i] != null)
                    slotsToUpdate.Add(i);
            }

            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slotsToUpdate);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));      
        }

        public void SetEquipment(ushort[] slots, ushort[] itemSlots)
        {
            if (slots.Length != itemSlots.Length)
                return;

            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItem item = normalInventory.getItemBySlot(itemSlots[i]);

                if (item == null)
                    continue;

                Database.equipItem(owner, slots[i], item.uniqueId);
                list[slots[i]] = normalInventory.getItemBySlot(itemSlots[i]);
            }

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            SendFullEquipment(false);
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void SetEquipment(InventoryItem[] toEquip)
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            for (ushort i = 0; i < toEquip.Length; i++)
            {
                if (toEquip[i] != null)
                    slotsToUpdate.Add(i); 
            }
            list = toEquip;
        }

        public void Equip(ushort slot, ushort invSlot)
        {
            InventoryItem item = normalInventory.getItemBySlot(invSlot);

            if (item == null)
                return;

            Equip(slot, item);
        }

        public void Equip(ushort slot, InventoryItem item)
        {
            if (slot >= list.Length)
                return;

            if (writeToDB)
                Database.equipItem(owner, slot, item.uniqueId);

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));

            if (list[slot] != null)            
                normalInventory.RefreshItem(list[slot], item);            
            else
                normalInventory.RefreshItem(item);

            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slot, item);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));

            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));

            list[slot] = item;
        }

        public void ToggleDBWrite(bool flag)
        {
            writeToDB = flag;
        }

        public void Unequip(ushort slot)
        {
            if (slot >= list.Length)
                return;

            if (writeToDB)
                Database.unequipItem(owner, slot);

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));

            normalInventory.RefreshItem(list[slot]);

            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slot, null);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));

            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));

            list[slot] = null;
        }

        private void SendEquipmentPackets(ushort equipSlot, InventoryItem item)
        {
            if (item == null)
                owner.queuePacket(InventoryRemoveX01Packet.buildPacket(owner.actorId, equipSlot));
            else
                owner.queuePacket(EquipmentListX01Packet.buildPacket(owner.actorId, equipSlot, item.slot));
        }

        private void SendEquipmentPackets(List<ushort> slotsToUpdate)
        {
            
            int currentIndex = 0;

            while (true)
            {
                if (slotsToUpdate.Count - currentIndex >= 64)
                    owner.queuePacket(EquipmentListX64Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 32)
                    owner.queuePacket(EquipmentListX32Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 16)
                    owner.queuePacket(EquipmentListX16Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex > 1)
                    owner.queuePacket(EquipmentListX08Packet.buildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex == 1)
                {
                    owner.queuePacket(EquipmentListX01Packet.buildPacket(owner.actorId, slotsToUpdate[currentIndex], list[slotsToUpdate[currentIndex]].slot));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        public int GetCapacity()
        {
            return list.Length;
        }

    }
}

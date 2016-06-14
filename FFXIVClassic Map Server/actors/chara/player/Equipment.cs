using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
using System.Collections.Generic;

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

            toPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, toPlayer.actorId, 0x23, Inventory.EQUIPMENT_OTHERPLAYER));
            int currentIndex = 0;

            while (true)
            {
                if (items.Count - currentIndex >= 16)
                    toPlayer.QueuePacket(InventoryListX16Packet.BuildPacket(owner.actorId, toPlayer.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex > 1)
                    toPlayer.QueuePacket(InventoryListX08Packet.BuildPacket(owner.actorId, toPlayer.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex == 1)
                {
                    toPlayer.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, toPlayer.actorId, items[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }
            toPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId, toPlayer.actorId));
        }

        public void SendFullEquipment(bool DoClear)
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            for (ushort i = 0; i < list.Length; i++)
            {
                if (list[i] == null && DoClear)
                    slotsToUpdate.Add(0);
                else if (list[i] != null)
                    slotsToUpdate.Add(i);
            }

            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slotsToUpdate);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));      
        }

        public void SetEquipment(ushort[] slots, ushort[] itemSlots)
        {
            if (slots.Length != itemSlots.Length)
                return;

            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItem item = normalInventory.GetItemBySlot(itemSlots[i]);

                if (item == null)
                    continue;

                Database.EquipItem(owner, slots[i], item.uniqueId);
                list[slots[i]] = normalInventory.GetItemBySlot(itemSlots[i]);
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            SendFullEquipment(false);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
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
            InventoryItem item = normalInventory.GetItemBySlot(invSlot);

            if (item == null)
                return;

            Equip(slot, item);
        }

        public void Equip(ushort slot, InventoryItem item)
        {
            if (slot >= list.Length)
                return;

            if (writeToDB)
                Database.EquipItem(owner, slot, item.uniqueId);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));

            if (list[slot] != null)            
                normalInventory.RefreshItem(list[slot], item);            
            else
                normalInventory.RefreshItem(item);

            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slot, item);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

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
                Database.UnequipItem(owner, slot);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));

            normalInventory.RefreshItem(list[slot]);

            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendEquipmentPackets(slot, null);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            list[slot] = null;
        }

        private void SendEquipmentPackets(ushort equipSlot, InventoryItem item)
        {
            if (item == null)
                owner.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, equipSlot));
            else
                owner.QueuePacket(EquipmentListX01Packet.BuildPacket(owner.actorId, equipSlot, item.slot));
        }

        private void SendEquipmentPackets(List<ushort> slotsToUpdate)
        {
            
            int currentIndex = 0;

            while (true)
            {
                if (slotsToUpdate.Count - currentIndex >= 64)
                    owner.QueuePacket(EquipmentListX64Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 32)
                    owner.QueuePacket(EquipmentListX32Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 16)
                    owner.QueuePacket(EquipmentListX16Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex > 1)
                    owner.QueuePacket(EquipmentListX08Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex == 1)
                {
                    owner.QueuePacket(EquipmentListX01Packet.BuildPacket(owner.actorId, slotsToUpdate[currentIndex], list[slotsToUpdate[currentIndex]].slot));
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

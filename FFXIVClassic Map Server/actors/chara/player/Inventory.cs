
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVClassic_Map_Server.actors.chara.player
{
    class Inventory
    {       
        public const ushort NORMAL = 0x0000; //Max 0xC8
        public const ushort LOOT = 0x0004; //Max 0xA
        public const ushort MELDREQUEST = 0x0005; //Max 0x04
        public const ushort BAZAAR = 0x0007; //Max 0x0A
        public const ushort CURRENCY = 0x0063; //Max 0x140
        public const ushort KEYITEMS = 0x0064; //Max 0x500
        public const ushort EQUIPMENT = 0x00FE; //Max 0x23
        public const ushort EQUIPMENT_OTHERPLAYER = 0x00F9; //Max 0x23

        private Player owner;
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private List<InventoryItem> list;

        public Inventory(Player ownerPlayer, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
        }

        #region Inventory Management
        public void InitList(List<InventoryItem> itemsFromDB)
        {
            list = itemsFromDB;
        }

        public InventoryItem GetItemBySlot(ushort slot)
        {
            if (slot < list.Count)
                return list[slot];
            else
                return null;
        }

        public InventoryItem GetItemByUniqueId(ulong uniqueItemId)
        {
            foreach (InventoryItem item in list)
            {
                if (item.uniqueId == uniqueItemId)
                    return item;
            }
            return null;
        }

        public InventoryItem GetItemByCatelogId(ulong catelogId)
        {
            foreach (InventoryItem item in list)
            {
                if (item.itemId == catelogId)
                    return item;
            }
            return null;
        }

        public void RefreshItem(InventoryItem item)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(item);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void RefreshItem(params InventoryItem[] items)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(items.ToList());
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void RefreshItem(List<InventoryItem> items)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(items);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void AddItem(uint itemId)
        {
            AddItem(itemId, 1, 1);
        }

        public void AddItem(uint itemId, int quantity)
        {
            AddItem(itemId, quantity, 1);
        }

        public bool AddItem(uint itemId, int quantity, byte quality)
        {
            if (!IsSpaceForAdd(itemId, quantity))
                return false;

            ItemData gItem = Server.GetItemGamedata(itemId);
            List<ushort> slotsToUpdate = new List<ushort>();
            List<SubPacket> addItemPackets = new List<SubPacket>();

            if (gItem == null)
            {
                Program.Log.Error("Inventory.AddItem: unable to find item %u", itemId);
                return false;
            }

            //Check if item id exists 
            int quantityCount = quantity;
            for (int i = 0; i < list.Count; i++)
            {                
                InventoryItem item = list[i];
                if (item.itemId == itemId && item.quantity < gItem.maxStack)
                {
                    slotsToUpdate.Add(item.slot);
                    int oldQuantity = item.quantity;
                    item.quantity = Math.Min(item.quantity + quantityCount, gItem.maxStack);
                    quantityCount -= (gItem.maxStack - oldQuantity);
                    if (quantityCount <= 0)
                        break;
                }
            }
                  
            //If it's unique, abort
            //if (quantityCount > 0 && storedItem.isUnique)
          //      return ITEMERROR_UNIQUE;

            //If Inventory is full
            //if (quantityCount > 0 && isInventoryFull())
         //       return ITEMERROR_FULL;

            //Update lists and db
            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            //These had their quantities Changed
            foreach (ushort slot in slotsToUpdate)
            {
                Database.SetQuantity(owner, slot, inventoryCode, list[slot].quantity);

                if (inventoryCode != CURRENCY && inventoryCode != KEYITEMS)
                    SendInventoryPackets(list[slot]);
            }

            //New item that spilled over
            while (quantityCount > 0)
            {
                InventoryItem addedItem = Database.AddItem(owner, itemId, Math.Min(quantityCount, gItem.maxStack), quality, gItem.isExclusive ? (byte)0x3 : (byte)0x0, gItem.durability, inventoryCode);

                
                list.Add(addedItem);

                if (inventoryCode != CURRENCY && inventoryCode != KEYITEMS)
                    SendInventoryPackets(addedItem);

                quantityCount -= gItem.maxStack;
            }

            if (inventoryCode == CURRENCY || inventoryCode == KEYITEMS)
                SendFullInventory();

            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            return true;
        }

        public void AddItem(uint[] itemId)
        {
            if (!IsSpaceForAdd(itemId[0], itemId.Length))
                return;

            //Update lists and db
            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            int startPos = list.Count;

            //New item that spilled over
            for (int i = 0; i < itemId.Length; i++)
            {
                ItemData gItem = Server.GetItemGamedata(itemId[i]);
                InventoryItem addedItem = Database.AddItem(owner, itemId[i], 1, (byte)1, gItem.isExclusive ? (byte)0x3 : (byte)0x0, gItem.durability, inventoryCode);
                list.Add(addedItem);
            }

            SendInventoryPackets(startPos);

            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }

        public void RemoveItem(uint itemId, int quantity)
        {
            if (!HasItem(itemId, quantity))
                return;

            List<ushort> slotsToUpdate = new List<ushort>();
            List<InventoryItem> itemsToRemove = new List<InventoryItem>();
            List<ushort> slotsToRemove = new List<ushort>();
            List<SubPacket> AddItemPackets = new List<SubPacket>();

            //Remove as we go along
            int quantityCount = quantity;
            ushort lowestSlot = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                InventoryItem item = list[i];
                if (item.itemId == itemId)
                {
                    int oldQuantity = item.quantity;
                    //Stack nomnomed
                    if (item.quantity - quantityCount <= 0)
                    {
                        itemsToRemove.Add(item);
                        slotsToRemove.Add(item.slot);
                    }
                    else
                    {
                        slotsToUpdate.Add(item.slot);
                        item.quantity -= quantityCount; //Stack reduced
                    }

                    quantityCount -= oldQuantity;
                    lowestSlot = item.slot;

                    if (quantityCount <= 0)
                        break;
                }
            }

            for (int i = 0; i < slotsToUpdate.Count; i++)
            {
                Database.SetQuantity(owner, slotsToUpdate[i], inventoryCode, list[slotsToUpdate[i]].quantity);
            }

            int oldListSize = list.Count;
            for (int i = 0; i < itemsToRemove.Count; i++)
            {
                Database.RemoveItem(owner, itemsToRemove[i].uniqueId, inventoryCode);
                list.Remove(itemsToRemove[i]);
            }

            //Realign slots
            for (int i = lowestSlot; i < list.Count; i++)
                list[i].slot = (ushort)i;

            //Added tail end items that need to be cleared for slot realignment
            for (int i = oldListSize-1; i >= oldListSize - itemsToRemove.Count; i--)
            {
                if (!slotsToRemove.Contains((ushort)i))
                    slotsToRemove.Add((ushort)i);
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            SendInventoryPackets(lowestSlot);
            SendInventoryRemovePackets(slotsToRemove);

            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.GetEquipment().SendFullEquipment(false);

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }

        public void RemoveItemByUniqueId(ulong itemDBId)
        {
            ushort slot = 0;
            InventoryItem toDelete = null;
            foreach (InventoryItem item in list)
            {
                if (item.uniqueId == itemDBId)
                {
                    toDelete = item;
                    break;
                }
                slot++;
            }

            if (toDelete == null)
                return;

            int oldListSize = list.Count;
            list.RemoveAt(slot);
            Database.RemoveItem(owner, itemDBId, inventoryCode);

            //Realign slots
            for (int i = slot; i < list.Count; i++)
                list[i].slot = (ushort)i;
           
            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            SendInventoryPackets(slot);
            SendInventoryRemovePackets(slot);
            if (slot != oldListSize - 1)
                SendInventoryRemovePackets((ushort)(oldListSize - 1));

            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.GetEquipment().SendFullEquipment(false);

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

        }

        public void RemoveItemAtSlot(ushort slot)
        {
            if (slot >= list.Count)
                return;

            int oldListSize = list.Count;
            list.RemoveAt((int)slot);
            Database.RemoveItem(owner, slot, inventoryCode);

            //Realign slots
            for (int i = slot; i < list.Count; i++)
                list[i].slot = (ushort)i;

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            SendInventoryPackets(slot);
            SendInventoryRemovePackets(slot);
            if (slot != oldListSize - 1)
                SendInventoryRemovePackets((ushort)(oldListSize - 1));

            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.GetEquipment().SendFullEquipment(false);

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }

        public void ChangeDurability(uint slot, uint durabilityChange)
        {

        }

        public void ChangeSpiritBind(uint slot, uint spiritBindChange)
        {

        }

        public void ChangeMateria(uint slot, byte materiaSlot, byte materiaId)
        {

        }
        #endregion

        #region Packet Functions
        public void SendFullInventory()
        {            
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(0);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        private void SendInventoryPackets(InventoryItem item)
        {
            owner.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, item));
        }

        private void SendInventoryPackets(List<InventoryItem> items)
        {
            int currentIndex = 0;

            while (true)
            {
                if (items.Count - currentIndex >= 64)
                    owner.QueuePacket(InventoryListX64Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 32)
                    owner.QueuePacket(InventoryListX32Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 16)
                    owner.QueuePacket(InventoryListX16Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex > 1)
                    owner.QueuePacket(InventoryListX08Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex == 1)
                {
                    owner.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, items[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void SendInventoryPackets(int startOffset)
        {
            int currentIndex = startOffset;

            while (true)
            {
                if (list.Count - currentIndex >= 64)
                    owner.QueuePacket(InventoryListX64Packet.BuildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex >= 32)
                    owner.QueuePacket(InventoryListX32Packet.BuildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex >= 16)
                    owner.QueuePacket(InventoryListX16Packet.BuildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex > 1)
                    owner.QueuePacket(InventoryListX08Packet.BuildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex == 1)
                {
                    owner.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, list[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void SendInventoryRemovePackets(ushort index)
        {
            owner.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, index));
        }

        private void SendInventoryRemovePackets(List<ushort> indexes)
        {
            int currentIndex = 0;

            while (true)
            {
                if (indexes.Count - currentIndex >= 64)
                    owner.QueuePacket(InventoryRemoveX64Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 32)
                    owner.QueuePacket(InventoryRemoveX32Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 16)
                    owner.QueuePacket(InventoryRemoveX16Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex > 1)
                    owner.QueuePacket(InventoryRemoveX08Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex == 1)
                {
                    owner.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, indexes[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        #endregion

        #region Inventory Utils

        public bool IsFull()
        {
            return list.Count >= inventoryCapacity;
        }
        
        public bool IsSpaceForAdd(uint itemId, int quantity)
        {
            int quantityCount = quantity;
            for (int i = 0; i < list.Count; i++)
            {
                InventoryItem item = list[i];
                ItemData gItem = Server.GetItemGamedata(item.itemId);
                if (item.itemId == itemId && item.quantity < gItem.maxStack)
                {
                    quantityCount -= (gItem.maxStack - item.quantity);
                    if (quantityCount <= 0)
                        break;
                }
            }

            return quantityCount <= 0 || (quantityCount > 0 && !IsFull());
        }

        public bool HasItem(uint itemId)
        {
            return HasItem(itemId, 1);
        }

        public bool HasItem(uint itemId, int minQuantity)
        {
            int count = 0;

            foreach (InventoryItem item in list)
            {
                if (item.itemId == itemId)
                    count += item.quantity;

                if (count >= minQuantity)
                    return true;
            }

            return false;
        }

        public int GetNextEmptySlot()
        {
            return list.Count == 0 ? 0 : list.Count();                
        }

        #endregion

    }
}

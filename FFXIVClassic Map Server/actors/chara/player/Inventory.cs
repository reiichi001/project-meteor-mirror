
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
        public const ushort TRADE = 0x0001; //Max 0x96
        public const ushort LOOT = 0x0004; //Max 0xA
        public const ushort MELDREQUEST = 0x0005; //Max 0x04
        public const ushort BAZAAR = 0x0007; //Max 0x0A
        public const ushort RETAINER_BAZAAR = 0x0008; //????
        public const ushort CURRENCY = 0x0063; //Max 0x140
        public const ushort KEYITEMS = 0x0064; //Max 0x500
        public const ushort EQUIPMENT = 0x00FE; //Max 0x23
        public const ushort EQUIPMENT_OTHERPLAYER = 0x00F9; //Max 0x23

        private int endOfListIndex = 0;

        private Character owner;
        private List<Player> viewer;
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private InventoryItem[] list;
        private InventoryItem[] lastList;
        private bool[] isDirty;

        public Inventory(Character ownerPlayer, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
        }

        #region Inventory Management
        public void InitList(List<InventoryItem> itemsFromDB)
        {
            int i = 0;
            foreach (InventoryItem item in itemsFromDB)
                list[i++] = item;
        }

        public InventoryItem GetItemAtSlot(ushort slot)
        {
            if (slot < list.Length)
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
          
            if (gItem == null)
            {
                Program.Log.Error("Inventory.AddItem: unable to find item %u", itemId);
                return false;
            }

            //Check if item id exists 
            int quantityCount = quantity;
            for (int i = 0; i < endOfListIndex; i++)
            {                
                InventoryItem item = list[i];

                if (item == null)
                    throw new Exception("Item slot was null!!!");

                if (item.itemId == itemId && item.quantity < gItem.maxStack)
                {
                    int oldQuantity = item.quantity;
                    item.quantity = Math.Min(item.quantity + quantityCount, gItem.maxStack);
                    isDirty[i] = true;
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

            //New item that spilled over
            while (quantityCount > 0)
            {
                InventoryItem addedItem = Database.CreateItem(itemId, Math.Min(quantityCount, gItem.maxStack), quality, gItem.isExclusive ? (byte)0x3 : (byte)0x0, gItem.durability);
                isDirty[endOfListIndex] = true;
                list[endOfListIndex++] = addedItem;                
                quantityCount -= gItem.maxStack;
            }

            return true;
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
            for (int i = endOfListIndex - 1; i >= 0; i--)
            {
                InventoryItem item = list[i];

                if (item == null)
                    throw new Exception("Item slot was null!!!");

                if (item.itemId == itemId)
                {
                    int oldQuantity = item.quantity;
                    //Stack nomnomed
                    if (item.quantity - quantityCount <= 0)                    
                        list[i] = null;                    
                    //Stack reduced
                    else                    
                        item.quantity -= quantityCount;

                    isDirty[i] = true;

                    quantityCount -= oldQuantity;
                    lowestSlot = item.slot;

                    if (quantityCount <= 0)
                        break;
                }
            }

            doRealign();

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

            list[slot] = null;
            isDirty[slot] = true;
            doRealign();
        }

        public void RemoveItemAtSlot(ushort slot)
        {
            if (slot >= endOfListIndex)
                return;

            list[slot] = null;
            isDirty[slot] = true;
            doRealign();
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
        public void SendFullInventory(Player player)
        {
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(player, 0);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        private void SendInventoryPackets(Player player, InventoryItem item)
        {
            player.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, item));
        }

        private void SendInventoryPackets(Player player, List<InventoryItem> items)
        {
            int currentIndex = 0;

            while (true)
            {
                if (items.Count - currentIndex >= 64)
                    player.QueuePacket(InventoryListX64Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 32)
                    player.QueuePacket(InventoryListX32Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 16)
                    player.QueuePacket(InventoryListX16Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex > 1)
                    player.QueuePacket(InventoryListX08Packet.BuildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex == 1)
                {
                    player.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, items[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void SendInventoryPackets(Player player, int startOffset)
        {
            int currentIndex = startOffset;

            List<InventoryItem> lst = new List<InventoryItem>(list);

            while (true)
            {
                if (endOfListIndex - currentIndex >= 64)
                    player.QueuePacket(InventoryListX64Packet.BuildPacket(owner.actorId, lst, ref currentIndex));
                else if (endOfListIndex - currentIndex >= 32)
                    player.QueuePacket(InventoryListX32Packet.BuildPacket(owner.actorId, lst, ref currentIndex));
                else if (endOfListIndex - currentIndex >= 16)
                    player.QueuePacket(InventoryListX16Packet.BuildPacket(owner.actorId, lst, ref currentIndex));
                else if (endOfListIndex - currentIndex > 1)
                    player.QueuePacket(InventoryListX08Packet.BuildPacket(owner.actorId, lst, ref currentIndex));
                else if (endOfListIndex - currentIndex == 1)
                {
                    player.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, list[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void SendInventoryRemovePackets(Player player, ushort index)
        {
            player.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, index));
        }

        private void SendInventoryRemovePackets(Player player, List<ushort> indexes)
        {
            int currentIndex = 0;

            while (true)
            {
                if (indexes.Count - currentIndex >= 64)
                    player.QueuePacket(InventoryRemoveX64Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 32)
                    player.QueuePacket(InventoryRemoveX32Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 16)
                    player.QueuePacket(InventoryRemoveX16Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex > 1)
                    player.QueuePacket(InventoryRemoveX08Packet.BuildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex == 1)
                {
                    player.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, indexes[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        #endregion

        #region Client Updating
        private void SendUpdatePackets(Player player)
        {
            List<InventoryItem> items = new List<InventoryItem>();
            List<ushort> slotsToRemove = new List<ushort>();

            for (int i = 0; i < list.Length; i++)
            {
                if (i == endOfListIndex)
                    break;
                if (isDirty[i])
                    items.Add(list[i]);
            }

            for (int i = endOfListIndex; i < lastList.Length; i++)
            {
                if (lastList[i] != null)
                    slotsToRemove.Add((ushort)i);
            }

            //Send Updated Slots
            SendInventoryPackets(player, items);

            //Send Remove packets for tail end
            SendInventoryRemovePackets(player, slotsToRemove);
        }
        #endregion

        #region Inventory Utils

        public bool IsFull()
        {
            return endOfListIndex >= inventoryCapacity;
        }
        
        public bool IsSpaceForAdd(uint itemId, int quantity)
        {
            int quantityCount = quantity;
            for (int i = 0; i < endOfListIndex; i++)
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
            return endOfListIndex;
        }

        private void doRealign()
        {
            int lastNullSlot = -1;            

            for (int i = 0; i < endOfListIndex; i++)
            {
                if (list[i] == null && lastNullSlot != -1)
                {
                    lastNullSlot = i;                    
                    continue;
                }
                else if (list[i] != null && lastNullSlot != -1)
                {
                    list[lastNullSlot] = list[i];
                    list[lastNullSlot].slot = (ushort)lastNullSlot;
                    list[i] = null;
                    isDirty[lastNullSlot] = true;
                    isDirty[i] = true;
                    lastNullSlot++;
                }
            }

            endOfListIndex = lastNullSlot;
        }

        #endregion

    }
}

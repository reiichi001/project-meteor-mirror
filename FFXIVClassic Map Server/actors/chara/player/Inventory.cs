
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private bool isTemporary;
        private InventoryItem[] list;
        private bool[] isDirty;

        public Inventory(Character ownerPlayer, ushort capacity, ushort code, bool temporary = false)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
            isTemporary = temporary;
            list = new InventoryItem[capacity];
            isDirty = new bool[capacity];
        }

        #region Inventory Management
        public void InitList(List<InventoryItem> itemsFromDB)
        {
            int i = 0;
            foreach (InventoryItem item in itemsFromDB)
                list[i++] = item;
            endOfListIndex = i;
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
            for (int i = 0; i < endOfListIndex; i++)
            {
                InventoryItem item = list[i];

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.uniqueId == uniqueItemId)
                    return item;
            }
            return null;
        }

        public InventoryItem GetItemByCatelogId(ulong catelogId)
        {
            for (int i = 0; i < endOfListIndex; i++)
            {
                InventoryItem item = list[i];

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.itemId == catelogId)
                    return item;
            }
            return null;
        }

        public bool AddItem(uint itemId)
        {
            return AddItem(itemId, 1, 1);
        }

        public bool AddItem(uint itemId, int quantity)
        {
            return AddItem(itemId, quantity, 1);
        }

        public bool AddItem(uint itemId, int quantity, byte quality)
        {
            if (!IsSpaceForAdd(itemId, quantity, quality))
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

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.itemId == itemId && item.quality == quality && item.quantity < gItem.maxStack)
                {
                    int oldQuantity = item.quantity;
                    item.quantity = Math.Min(item.quantity + quantityCount, gItem.maxStack);
                    isDirty[i] = true;
                    quantityCount -= (gItem.maxStack - oldQuantity);

                    DoDatabaseQuantity(item.uniqueId, item.quantity);
                    
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
                addedItem.slot = (ushort)endOfListIndex;
                isDirty[endOfListIndex] = true;
                list[endOfListIndex++] = addedItem;                
                quantityCount -= gItem.maxStack;

                DoDatabaseAdd(addedItem);
            }

            SendUpdatePackets();           

            return true;
        }

        public void RemoveItem(uint itemId)
        {
            RemoveItem(itemId, 1);
        }
        
        public void RemoveItem(uint itemId, int quantity)
        {
            RemoveItem(itemId, quantity, 1);
        }

        public void RemoveItem(uint itemId, int quantity, int quality)
        {
            if (!HasItem(itemId, quantity, quality))
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

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.itemId == itemId && item.quality == quality)
                {
                    int oldQuantity = item.quantity;
                    //Stack nomnomed
                    if (item.quantity - quantityCount <= 0)
                    {
                        DoDatabaseRemove(list[i].uniqueId);                        
                        list[i] = null;
                    }
                    //Stack reduced
                    else
                    {
                        item.quantity -= quantityCount;
                        DoDatabaseQuantity(list[i].uniqueId, list[i].quantity);
                    }                        

                    isDirty[i] = true;

                    quantityCount -= oldQuantity;
                    lowestSlot = item.slot;

                    if (quantityCount <= 0)
                        break;
                }
            }

            DoRealign();
            SendUpdatePackets();
        }

        public void RemoveItemByUniqueId(ulong itemDBId)
        {
            ushort slot = 0;
            InventoryItem toDelete = null;
            for (int i = endOfListIndex - 1; i >= 0; i--)
            {
                InventoryItem item = list[i];

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.uniqueId == itemDBId)
                {
                    toDelete = item;
                    break;
                }
                slot++;
            }

            if (toDelete == null)
                return;
            
            DoDatabaseRemove(toDelete.uniqueId);

            list[slot] = null;
            isDirty[slot] = true;

            DoRealign();
            SendUpdatePackets();
        }

        public void RemoveItemAtSlot(ushort slot)
        {
            if (slot >= endOfListIndex)
                return;

            DoDatabaseRemove(list[slot].uniqueId);

            list[slot] = null;
            isDirty[slot] = true;
            
            DoRealign();
            SendUpdatePackets();
        }

        public void RemoveItemAtSlot(ushort slot, int quantity)
        {
            if (slot >= endOfListIndex)
                return;

            if (list[slot] != null)
            {
                list[slot].quantity -= quantity;

                if (list[slot].quantity <= 0)
                {
                    DoDatabaseRemove(list[slot].uniqueId);

                    list[slot] = null;
                    DoRealign();
                }
                else                
                    DoDatabaseQuantity(list[slot].uniqueId, list[slot].quantity);                

                isDirty[slot] = true;
                SendUpdatePackets((Player)owner);
            }                                  
        }

        public void ChangeDurability(uint slot, uint durabilityChange)
        {
            isDirty[slot] = true;
        }

        public void ChangeSpiritBind(uint slot, uint spiritBindChange)
        {
            isDirty[slot] = true;
        }

        public void ChangeMateria(uint slot, byte materiaSlot, byte materiaId)
        {
            isDirty[slot] = true;
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

            List<InventoryItem> lst = new List<InventoryItem>();
            for (int i = 0; i < endOfListIndex; i++)
                lst.Add(list[i]);

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

        public void RefreshItem(Player player, InventoryItem item)
        {
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(player, item);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void RefreshItem(Player player, params InventoryItem[] items)
        {
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(player, items.ToList());
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void RefreshItem(Player player, List<InventoryItem> items)
        {
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendInventoryPackets(player, items);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        #endregion

        #region Automatic Client and DB Updating

        private void DoDatabaseAdd(InventoryItem addedItem)
        {
            if (isTemporary)
                return;

            if (owner is Player)            
                Database.AddItem((Player)owner, addedItem, inventoryCode);            
            else if (owner is Retainer)
            {

            }
        }

        private void DoDatabaseQuantity(ulong itemDBId, int quantity)
        {
            if (isTemporary)
                return;

            if (owner is Player)            
                Database.SetQuantity((Player)owner, itemDBId, inventoryCode);            
            else if (owner is Retainer)
            {

            }
        }

        private void DoDatabaseRemove(ulong itemDBId)
        {
            if (isTemporary)
                return;

            if (owner is Player)            
                Database.RemoveItem((Player)owner, itemDBId);            
            else if (owner is Retainer)
            {

            }
        }

        private void SendUpdatePackets()
        {
            if (owner is Player)
                SendUpdatePackets((Player)owner);
        }

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

            for (int i = endOfListIndex; i < list.Length; i++)
            {
                if (isDirty[i])
                    slotsToRemove.Add((ushort)i);
            }

            Array.Clear(isDirty, 0, isDirty.Length);

            player.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));                        
            //Send Updated Slots
            SendInventoryPackets(player, items);
            //Send Remove packets for tail end
            SendInventoryRemovePackets(player, slotsToRemove);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
            player.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }
        #endregion

        #region Inventory Utils

        public bool IsFull()
        {
            return endOfListIndex >= inventoryCapacity;
        }
        
        public bool IsSpaceForAdd(uint itemId, int quantity, int quality)
        {
            int quantityCount = quantity;
            for (int i = 0; i < endOfListIndex; i++)
            {
                InventoryItem item = list[i];
                ItemData gItem = Server.GetItemGamedata(item.itemId);
                if (item.itemId == itemId && item.quality == quality && item.quantity < gItem.maxStack)
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
            return HasItem(itemId, minQuantity, 1);
        }

        public bool HasItem(uint itemId, int minQuantity, int quality)
        {
            int count = 0;

            for (int i = endOfListIndex - 1; i >= 0; i--)
            {
                InventoryItem item = list[i];

                Debug.Assert(item != null, "Item slot was null!!!");

                if (item.itemId == itemId && item.quality == quality)
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

        private void DoRealign()
        {
            int lastNullSlot = -1;            

            for (int i = 0; i < endOfListIndex; i++)
            {
                if (list[i] == null && lastNullSlot == -1)
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

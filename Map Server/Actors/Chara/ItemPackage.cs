/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.Common;
using Meteor.Map.actors.chara.npc;
using Meteor.Map.Actors;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.actor.inventory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Meteor.Map.actors.chara.player
{
    class ItemPackage
    {       
        public const ushort NORMAL                  = 0; //Max 0xC8
        public const ushort UNKNOWN                 = 1; //Max 0x96
        public const ushort LOOT                    = 4; //Max 0xA
        public const ushort MELDREQUEST             = 5; //Max 0x04
        public const ushort BAZAAR                  = 7; //Max 0x0A
        public const ushort CURRENCY_CRYSTALS       = 99; //Max 0x140
        public const ushort KEYITEMS                = 100; //Max 0x500
        public const ushort EQUIPMENT               = 0x00FE; //Max 0x23
        public const ushort TRADE                   = 0x00FD; //Max 0x04
        public const ushort EQUIPMENT_OTHERPLAYER   = 0x00F9; //Max 0x23

        public const ushort MAXSIZE_NORMAL = 200;
        public const ushort MAXSIZE_CURRANCY = 320;
        public const ushort MAXSIZE_KEYITEMS = 500;
        public const ushort MAXSIZE_LOOT = 10;
        public const ushort MAXSIZE_TRADE = 4;
        public const ushort MAXSIZE_MELDREQUEST = 4;
        public const ushort MAXSIZE_BAZAAR = 10;
        public const ushort MAXSIZE_EQUIPMENT = 35;
        public const ushort MAXSIZE_EQUIPMENT_OTHERPLAYER = 0x23;

        public const int ERROR_SUCCESS = 0;
        public const int ERROR_FULL = 1;
        public const int ERROR_HAS_UNIQUE = 2;
        public const int ERROR_SYSTEM = 3;

        private Character owner;
        private ushort itemPackageCapacity;
        private ushort itemPackageCode;
        private bool isTemporary;
        private InventoryItem[] list;
        private bool[] isDirty;
        private bool holdingUpdates = false;

        private int endOfListIndex = 0;
        
        public ItemPackage(Character ownerPlayer, ushort capacity, ushort code, bool temporary = false)
        {
            owner = ownerPlayer;
            itemPackageCapacity = capacity;
            itemPackageCode = code;
            isTemporary = temporary;
            list = new InventoryItem[capacity];
            isDirty = new bool[capacity];
        }

        #region Inventory Management
        public void InitList(List<InventoryItem> itemsFromDB)
        {
            int i = 0;
            foreach (InventoryItem item in itemsFromDB)
            {
                item.SetOwner(owner, itemPackageCode, (ushort) i);
                list[i++] = item;
            }
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

        public int AddItem(uint itemId)
        {
            return AddItem(itemId, 1, 1);
        }

        public int AddItem(uint itemId, int quantity)
        {
            return AddItem(itemId, quantity, 1);
        }

        public int AddItem(uint itemId, int quantity, byte quality)
        {
            if (!IsSpaceForAdd(itemId, quantity, quality))
                return ERROR_FULL;

            ItemData gItem = Server.GetItemGamedata(itemId);

            //If it's unique, abort
            if (HasItem(itemId) && gItem.isExclusive)
                return ERROR_HAS_UNIQUE;

            if (gItem == null)
            {
                Program.Log.Error("Inventory.AddItem: unable to find item %u", itemId);
                return ERROR_SYSTEM;
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

            //New item that spilled over
            while (quantityCount > 0)
            {
                InventoryItem.ItemModifier modifiers = null;
                if (gItem.durability != 0)
                {
                    modifiers = new InventoryItem.ItemModifier();
                    modifiers.durability = (uint)gItem.durability;
                }

                InventoryItem addedItem = Database.CreateItem(itemId, Math.Min(quantityCount, gItem.maxStack), quality, modifiers);
                addedItem.SetOwner(owner, itemPackageCode, (ushort)endOfListIndex);
                isDirty[endOfListIndex] = true;
                list[endOfListIndex++] = addedItem;
                quantityCount -= gItem.maxStack;

                DoDatabaseAdd(addedItem);
            }

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }

            return ERROR_SUCCESS;
        }

        public int AddItems(uint[] itemIds, uint[] quantity = null, byte[] quality = null)
        {
            //Check if has space
            if (!CanAdd(itemIds, quantity, quality))
                return ERROR_FULL;
                        
            for (int i = 0; i < itemIds.Length; i++)
            {
                ItemData gItem = Server.GetItemGamedata(itemIds[i]);

                //If it's unique, abort
                if (HasItem(itemIds[i]) && gItem.isExclusive)
                    return ERROR_HAS_UNIQUE;

                if (gItem == null)
                {
                    Program.Log.Error("Inventory.AddItem: unable to find item %u", itemIds[i]);
                    return ERROR_SYSTEM;
                }

                //Check if item id exists 
                uint setQuantity = quantity != null ? quantity[i] : 1;
                int quantityCount = (int)setQuantity;
                for (int j = 0; j < endOfListIndex; j++)
                {
                    InventoryItem item = list[j];

                    Debug.Assert(item != null, "Item slot was null!!!");

                    byte setQuality = quality != null ? quality[i] : (byte)1;

                    if (item.itemId == itemIds[i] && item.quality == setQuality && item.quantity < gItem.maxStack)
                    {
                        int oldQuantity = item.quantity;
                        item.quantity = Math.Min(item.quantity + quantityCount, gItem.maxStack);
                        isDirty[j] = true;
                        quantityCount -= (gItem.maxStack - oldQuantity);

                        DoDatabaseQuantity(item.uniqueId, item.quantity);

                        if (quantityCount <= 0)
                            break;
                    }
                }

                //New item that spilled over
                while (quantityCount > 0)
                {
                    InventoryItem.ItemModifier modifiers = null;
                    if (gItem.durability != 0)
                    {
                        modifiers = new InventoryItem.ItemModifier();
                        modifiers.durability = (uint)gItem.durability;
                    }

                    byte setQuality = quality != null ? quality[i] : (byte)1;

                    InventoryItem addedItem = Database.CreateItem(itemIds[i], Math.Min(quantityCount, gItem.maxStack), setQuality, modifiers);
                    addedItem.SetOwner(owner, itemPackageCode, (ushort)endOfListIndex);
                    isDirty[endOfListIndex] = true;
                    list[endOfListIndex++] = addedItem;
                    quantityCount -= gItem.maxStack;

                    DoDatabaseAdd(addedItem);
                }
            }

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }

            return ERROR_SUCCESS;
        }
        
        public int AddItem(InventoryItem itemRef)
        {
            //If it isn't a single item (ie: armor) just add like normal (not valid for BAZAAR)
            if (itemPackageCode != BAZAAR && itemRef.GetItemData().maxStack > 1)
                return AddItem(itemRef.itemId, itemRef.quantity, itemRef.quality);

            if (!IsSpaceForAdd(itemRef.itemId, itemRef.quantity, itemRef.quality))
                return ERROR_FULL;

            ItemData gItem = Server.GetItemGamedata(itemRef.itemId);

            if (gItem == null)
            {
                Program.Log.Error("Inventory.AddItem: unable to find item %u", itemRef.itemId);
                return ERROR_SYSTEM;
            }

            itemRef.SetOwner(owner, itemPackageCode, (ushort)endOfListIndex);

            isDirty[endOfListIndex] = true;
            list[endOfListIndex++] = itemRef;
            DoDatabaseAdd(itemRef);

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }

            return ERROR_SUCCESS;           
        }

        public int MoveItem(ushort position, ItemPackage destinationPackage)
        {
            InventoryItem item = GetItemAtSlot(position);

            if (destinationPackage.CanAdd(item))
            {
                RemoveItemAtSlot(position);
                destinationPackage.AddItem(item);
                return ERROR_SUCCESS;
            }
            return ERROR_FULL;
        }

        public int MoveItem(InventoryItem item, ItemPackage destinationPackage)
        {
            if (destinationPackage == null || item == null)
                return ERROR_SYSTEM;

            if (destinationPackage.CanAdd(item))
            {
                RemoveItem(item);
                destinationPackage.AddItem(item);
                return ERROR_SUCCESS;
            }
            return ERROR_FULL;
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
                        DoDatabaseQuantity(list[i].uniqueId, list[i].quantity);}

                    isDirty[i] = true;

                    quantityCount -= oldQuantity;
                    lowestSlot = item.slot;

                    if (quantityCount <= 0)
                        break;
                }
            }

            DoRealign();

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }
        }

        public void RemoveItem(InventoryItem item)
        {
            if (itemPackageCode == item.itemPackage)
                RemoveItemAtSlot(item.slot);
        }
        
        public void RemoveItemByUniqueId(ulong itemDBId, int quantity)
        {
            ushort slot = 0;
            InventoryItem toDelete = null;
            for (int i = 0; i < endOfListIndex; i++)
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

            if (quantity >= toDelete.quantity)
            {
                DoDatabaseRemove(toDelete.uniqueId);
                list[slot].ClearOwner();
                list[slot] = null;
            }
            else
            {
                list[slot].quantity -= quantity;
                DoDatabaseQuantity(list[slot].uniqueId, list[slot].quantity);
            }

            isDirty[slot] = true;

            DoRealign();

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }
        }

        public void RemoveItemAtSlot(ushort slot)
        {
            if (slot >= endOfListIndex)
                return;

            DoDatabaseRemove(list[slot].uniqueId);

            list[slot].ClearOwner();
            list[slot] = null;
            isDirty[slot] = true;

            DoRealign();

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }
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

                    list[slot].ClearOwner();
                    list[slot] = null;
                    DoRealign();
                }
                else
                    DoDatabaseQuantity(list[slot].uniqueId, list[slot].quantity);

                isDirty[slot] = true;

                if (owner is Player)
                {
                    (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                    SendUpdate();
                    (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
                }
            }                   
        }

        public void Clear()
        {
            for (int i = 0; i < endOfListIndex; i++)
            {
                list[i].ClearOwner();
                list[i] = null;
                isDirty[i] = true;
            }
            endOfListIndex = 0;

            if (owner is Player)
            {
                (owner as Player).QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
                SendUpdate();
                (owner as Player).QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
            }
        }

        public bool CanAdd(InventoryItem item)
        {
            return itemPackageCapacity - GetCount() > 0;
        }

        public bool CanAdd(uint[] itemIds, uint[] quantity, byte[] quality)
        {
            int tempInvSize = GetCount();

            for (int i = 0; i < itemIds.Length; i++)
            {
                ItemData gItem = Server.GetItemGamedata(itemIds[i]);
                //Check if item id exists and fill up til maxstack 
                int quantityCount = (int)(quantity != null ? quantity[i] : 1);
                for (int j = 0; j < endOfListIndex; j++)
                {
                    InventoryItem item = list[j];

                    Debug.Assert(item != null, "Item slot was null!!!");

                    if (item.itemId == itemIds[i] && item.quality == (quality != null ? quality[i] : 1) && item.quantity < gItem.maxStack)
                    {
                        quantityCount -= (gItem.maxStack - item.quantity);
                        if (quantityCount <= 0)
                            break;
                    }
                }

                //New items that spilled over creating new stacks
                while (quantityCount > 0)
                {
                    quantityCount -= gItem.maxStack;
                    tempInvSize++;
                }

                //If the new stacks push us over capacity, can't add these items
                if (tempInvSize > itemPackageCapacity)
                    return false;
            }

            return true;
        }

        public void MarkDirty(InventoryItem item)
        {
            if (item.itemPackage != itemPackageCode || list[item.slot] == null)
                return;

            isDirty[item.slot] = true;
        }

        public void MarkDirty(ushort slot)
        {
            isDirty[slot] = true;
        }

        public InventoryItem[] GetRawList()
        {
            return list;
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
        public void SendFullPackage(Player player)
        {
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, itemPackageCapacity, itemPackageCode));
            SendItemPackets(player, 0);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));            
        }

        public void SendUpdate()
        {
            if (owner is Player && !holdingUpdates)
            {
                SendUpdate((Player)owner);
            }
        }

        public void SendUpdate(Player player)
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

            if (!holdingUpdates)
                Array.Clear(isDirty, 0, isDirty.Length);
            
            player.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, itemPackageCapacity, itemPackageCode));
            //Send Updated Slots
            SendItemPackets(player, items);
            //Send Remove packets for tail end
            SendItemPackets(player, slotsToRemove);
            player.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
            //If player is updating their normal inventory, we need to send
            //an equip update as well to resync the slots.
            if (player.Equals(owner) && itemPackageCode == NORMAL)
                player.GetEquipment().SendUpdate();
        }

        private void SendItemPackets(Player player, InventoryItem item)
        {
             player.QueuePacket(InventoryListX01Packet.BuildPacket(owner.actorId, item));            
        }

        private void SendItemPackets(Player player, List<InventoryItem> items)
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

        private void SendItemPackets(Player player, int startOffset)
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

        private void SendItemPackets(Player player, ushort index)
        {
            player.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, index));
        }

        private void SendItemPackets(Player player, List<ushort> indexes)
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

        #region Automatic Client and DB Updating

        private void DoDatabaseAdd(InventoryItem addedItem)
        {
            if (isTemporary)
                return;

            Database.AddItem(owner, addedItem, itemPackageCode, addedItem.slot);
        }

        private void DoDatabaseQuantity(ulong itemDBId, int quantity)
        {
            if (isTemporary)
                return;
            
            Database.SetQuantity(itemDBId, quantity);
        }

        private void DoDatabaseRemove(ulong itemDBId)
        {
            if (isTemporary)
                return;
            
            Database.RemoveItem(owner, itemDBId);
        }
        
        public void StartSendUpdate()
        {
            holdingUpdates = true;
        }

        public void DoneSendUpdate()
        {
            holdingUpdates = false;
            SendUpdate();
            Array.Clear(isDirty, 0, isDirty.Length);
        }

        #endregion

        #region Inventory Utils

        public bool IsFull()
        {
            return endOfListIndex >= itemPackageCapacity;
        }

        public int GetFreeSlots()
        {
            return itemPackageCapacity - endOfListIndex;
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
            List<InventoryItem> positionUpdate = new List<InventoryItem>();

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
                    if (list[lastNullSlot].GetOfferedTo() != null)
                    {
                        list[lastNullSlot].UpdateOfferedSlot((ushort)(list[lastNullSlot].slot - lastNullSlot));                        
                    }
                    list[lastNullSlot].slot = (ushort)lastNullSlot;
                    positionUpdate.Add(list[lastNullSlot]);
                    list[i] = null;
                    isDirty[lastNullSlot] = true;
                    isDirty[i] = true;
                    lastNullSlot++;
                }
            }

            if (lastNullSlot != -1)
                endOfListIndex = lastNullSlot;

            Database.UpdateItemPositions(positionUpdate);
        }

        #endregion
            
        public int GetCount()
        {
            return endOfListIndex;
        }

        public override string ToString()
        {
            string packageName;
            switch (itemPackageCode)
            {
                case NORMAL:
                    packageName = "Inventory";
                    break;
                case LOOT:
                    packageName = "Loot";
                    break;
                case MELDREQUEST:
                    packageName = "Meld Request";
                    break;
                case BAZAAR:
                    packageName = "Bazaar";
                    break;
                case CURRENCY_CRYSTALS:
                    packageName = "Currency";
                    break;
                case KEYITEMS:
                    packageName = "KeyItems";
                    break;
                case EQUIPMENT:
                    packageName = "Equipment";
                    break;
                case TRADE:
                    packageName = "Trade";
                    break;
                case EQUIPMENT_OTHERPLAYER:
                    packageName = "CheckEquip";
                    break;
                default:
                    packageName = "Unknown";
                    break;
            }

            return string.Format("{0} Package", packageName);
        }
    }
}

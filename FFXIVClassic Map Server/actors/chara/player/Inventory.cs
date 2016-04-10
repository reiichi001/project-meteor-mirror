using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.packets;
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
        public void initList(List<InventoryItem> itemsFromDB)
        {
            list = itemsFromDB;
        }

        public InventoryItem getItemBySlot(ushort slot)
        {
            if (slot < list.Count)
                return list[slot];
            else
                return null;
        }

        public InventoryItem getItemById(ulong itemId)
        {
            foreach (InventoryItem item in list)
            {
                if (item.uniqueId == itemId)
                    return item;
            }
            return null;
        }

        public void RefreshItem(InventoryItem item)
        {
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            sendInventoryPackets(item);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
        }

        public void RefreshItem(params InventoryItem[] items)
        {
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            sendInventoryPackets(items.ToList());
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
        }

        public void RefreshItem(List<InventoryItem> items)
        {
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            sendInventoryPackets(items);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
        }

        public void addItem(uint itemId)
        {
            addItem(itemId, 1, 1);
        }

        public void addItem(uint itemId, int quantity)
        {
            addItem(itemId, quantity, 1);
        }

        public void addItem(uint itemId, int quantity, byte quality)
        {
            if (!isSpaceForAdd(itemId, quantity))
                return;

            Item gItem = Server.getItemGamedata(itemId);
            List<ushort> slotsToUpdate = new List<ushort>();
            List<SubPacket> addItemPackets = new List<SubPacket>();

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
            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            //These had their quantities changed
            foreach (ushort slot in slotsToUpdate)
            {
                Database.setQuantity(owner, slot, inventoryCode, list[slot].quantity);

                if (inventoryCode != CURRENCY && inventoryCode != KEYITEMS)
                    sendInventoryPackets(list[slot]);
            }

            //New item that spilled over
            while (quantityCount > 0)
            {
                InventoryItem addedItem = Database.addItem(owner, itemId, Math.Min(quantityCount, gItem.maxStack), quality, gItem.isExclusive ? (byte)0x3 : (byte)0x0, gItem.durability, inventoryCode);

                
                list.Add(addedItem);

                if (inventoryCode != CURRENCY && inventoryCode != KEYITEMS)
                    sendInventoryPackets(addedItem);

                quantityCount -= gItem.maxStack;
            }

            if (inventoryCode == CURRENCY || inventoryCode == KEYITEMS)
                sendFullInventory();

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void addItem(uint[] itemId)
        {
            if (!isSpaceForAdd(itemId[0], itemId.Length))
                return;

            //Update lists and db
            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            int startPos = list.Count;

            //New item that spilled over
            for (int i = 0; i < itemId.Length; i++)
            {
                Item gItem = Server.getItemGamedata(itemId[i]);
                InventoryItem addedItem = Database.addItem(owner, itemId[i], 1, (byte)1, gItem.isExclusive ? (byte)0x3 : (byte)0x0, gItem.durability, inventoryCode);
                list.Add(addedItem);
            }

            sendInventoryPackets(startPos);

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void removeItem(uint itemId, int quantity)
        {
            if (!hasItem(itemId, quantity))
                return;

            List<ushort> slotsToUpdate = new List<ushort>();
            List<InventoryItem> itemsToRemove = new List<InventoryItem>();
            List<ushort> slotsToRemove = new List<ushort>();
            List<SubPacket> addItemPackets = new List<SubPacket>();

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
                Database.setQuantity(owner, slotsToUpdate[i], inventoryCode, list[slotsToUpdate[i]].quantity);
            }

            int oldListSize = list.Count;
            for (int i = 0; i < itemsToRemove.Count; i++)
            {
                Database.removeItem(owner, itemsToRemove[i].uniqueId, inventoryCode);
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

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(lowestSlot);
            sendInventoryRemovePackets(slotsToRemove);

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.getEquipment().SendFullEquipment(false);

            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void removeItem(ulong itemDBId)
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
            Database.removeItem(owner, itemDBId, inventoryCode);

            //Realign slots
            for (int i = slot; i < list.Count; i++)
                list[i].slot = (ushort)i;
           
            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(slot);
            sendInventoryRemovePackets(slot);
            if (slot != oldListSize - 1)
                sendInventoryRemovePackets((ushort)(oldListSize - 1));

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.getEquipment().SendFullEquipment(false);

            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));

        }

        public void removeItem(ushort slot)
        {
            if (slot >= list.Count)
                return;

            int oldListSize = list.Count;
            list.RemoveAt((int)slot);
            Database.removeItem(owner, slot, inventoryCode);

            //Realign slots
            for (int i = slot; i < list.Count; i++)
                list[i].slot = (ushort)i;

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(slot);
            sendInventoryRemovePackets(slot);
            if (slot != oldListSize - 1)
                sendInventoryRemovePackets((ushort)(oldListSize - 1));

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));

            if (inventoryCode == NORMAL)
                owner.getEquipment().SendFullEquipment(false);

            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void changeDurability(uint slot, uint durabilityChange)
        {

        }

        public void changeSpiritBind(uint slot, uint spiritBindChange)
        {

        }

        public void changeMateria(uint slot, byte materiaSlot, byte materiaId)
        {

        }
        #endregion

        #region Packet Functions
        public void sendFullInventory()
        {            
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            sendInventoryPackets(0);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
        }

        private void sendInventoryPackets(InventoryItem item)
        {
            owner.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, item));
        }

        private void sendInventoryPackets(List<InventoryItem> items)
        {
            int currentIndex = 0;

            while (true)
            {
                if (items.Count - currentIndex >= 64)
                    owner.queuePacket(InventoryListX64Packet.buildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 32)
                    owner.queuePacket(InventoryListX32Packet.buildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex >= 16)
                    owner.queuePacket(InventoryListX16Packet.buildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex > 1)
                    owner.queuePacket(InventoryListX08Packet.buildPacket(owner.actorId, items, ref currentIndex));
                else if (items.Count - currentIndex == 1)
                {
                    owner.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, items[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void sendInventoryPackets(int startOffset)
        {
            int currentIndex = startOffset;

            while (true)
            {
                if (list.Count - currentIndex >= 64)
                    owner.queuePacket(InventoryListX64Packet.buildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex >= 32)
                    owner.queuePacket(InventoryListX32Packet.buildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex >= 16)
                    owner.queuePacket(InventoryListX16Packet.buildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex > 1)
                    owner.queuePacket(InventoryListX08Packet.buildPacket(owner.actorId, list, ref currentIndex));
                else if (list.Count - currentIndex == 1)
                {
                    owner.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, list[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        private void sendInventoryRemovePackets(ushort index)
        {
            owner.queuePacket(InventoryRemoveX01Packet.buildPacket(owner.actorId, index));
        }

        private void sendInventoryRemovePackets(List<ushort> indexes)
        {
            int currentIndex = 0;

            while (true)
            {
                if (indexes.Count - currentIndex >= 64)
                    owner.queuePacket(InventoryRemoveX64Packet.buildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 32)
                    owner.queuePacket(InventoryRemoveX32Packet.buildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex >= 16)
                    owner.queuePacket(InventoryRemoveX16Packet.buildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex > 1)
                    owner.queuePacket(InventoryRemoveX08Packet.buildPacket(owner.actorId, indexes, ref currentIndex));
                else if (indexes.Count - currentIndex == 1)
                {
                    owner.queuePacket(InventoryRemoveX01Packet.buildPacket(owner.actorId, indexes[currentIndex]));
                    currentIndex++;
                }
                else
                    break;
            }

        }

        #endregion

        #region Inventory Utils

        public bool isFull()
        {
            return list.Count >= inventoryCapacity;
        }
        
        public bool isSpaceForAdd(uint itemId, int quantity)
        {
            int quantityCount = quantity;
            for (int i = 0; i < list.Count; i++)
            {
                InventoryItem item = list[i];
                Item gItem = Server.getItemGamedata(item.itemId);
                if (item.itemId == itemId && item.quantity < gItem.maxStack)
                {
                    quantityCount -= (gItem.maxStack - item.quantity);
                    if (quantityCount <= 0)
                        break;
                }
            }

            return quantityCount <= 0 || (quantityCount > 0 && !isFull());
        }

        public bool hasItem(uint itemId)
        {
            return hasItem(itemId, 1);
        }

        public bool hasItem(uint itemId, int minQuantity)
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

        public int getNextEmptySlot()
        {
            return list.Count == 0 ? 0 : list.Count();                
        }

        #endregion

    }
}

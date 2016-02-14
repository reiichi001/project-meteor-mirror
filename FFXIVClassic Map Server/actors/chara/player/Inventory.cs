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
        public const ushort CURRANCY = 0x0063; //Max 0x140
        public const ushort KEYITEMS = 0x0064; //Max 0x500
        public const ushort EQUIPMENT = 0x00FE; //Max 0x23

        private Player owner;
        private ushort inventoryCapacity;
        private ushort inventoryCode;
        private List<Item> list;

        public Inventory(Player ownerPlayer, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
        }

        #region Inventory Management
        public void initList(List<Item> itemsFromDB)
        {
            list = itemsFromDB;
        }

        public void addItem(uint itemId, ushort type, int quantity, byte quality)
        {
            if (!isSpaceForAdd(itemId, quantity))
                return;

            List<ushort> slotsToUpdate = new List<ushort>();
            List<SubPacket> addItemPackets = new List<SubPacket>();

            //Check if item id exists 
            int quantityCount = quantity;
            for (int i = 0; i < list.Count; i++)
            {
                Item item = list[i];
                if (item.itemId == itemId && item.quantity < item.maxStack)
                {
                    slotsToUpdate.Add(item.slot);
                    int oldQuantity = item.quantity;
                    item.quantity = Math.Min(item.quantity + quantityCount, item.maxStack);
                    quantityCount -= (item.maxStack - oldQuantity);
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
                Database.setQuantity(owner, slot, list[slot].quantity);                
                sendInventoryPackets(list[slot]);
            }

            //New item that spilled over
            while (quantityCount > 0)
            {
                Item addedItem = Database.addItem(owner, itemId, Math.Min(quantityCount, 5), quality, false, 100, type);
                list.Add(addedItem);
                sendInventoryPackets(addedItem);

                quantityCount -= addedItem.maxStack;
            }
            
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void removeItem(uint itemId, int quantity)
        {
            if (!hasItem(itemId, quantity))
                return;

            List<ushort> slotsToUpdate = new List<ushort>();
            List<Item> itemsToRemove = new List<Item>();
            List<ushort> slotsToRemove = new List<ushort>();
            List<SubPacket> addItemPackets = new List<SubPacket>();

            //Remove as we go along
            int quantityCount = quantity;
            ushort lowestSlot = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Item item = list[i];
                if (item.itemId == itemId)
                {
                    //Stack nomnomed
                    if (item.quantity - quantityCount <= 0)
                    {
                        itemsToRemove.Add(item);
                    }
                    else
                    {
                        slotsToUpdate.Add(item.slot);
                        item.quantity -= quantityCount; //Stack reduced
                    }

                    quantityCount -= item.quantity;
                    lowestSlot = item.slot;

                    if (quantityCount <= 0)
                        break;
                }
            }

            for (int i = 0; i < slotsToUpdate.Count; i++)
            {
                Database.setQuantity(owner, slotsToUpdate[i], list[slotsToUpdate[i]].quantity);
            }

            for (int i = 0; i < itemsToRemove.Count; i++)
            {
                Database.removeItem(owner, itemsToRemove[i].uniqueId);
                list.Remove(itemsToRemove[i]);
            }

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(lowestSlot);
            sendInventoryRemovePackets(slotsToRemove);

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void removeItem(ulong itemDBId)
        {
            ushort slot = 0;
            Item toDelete = null;
            foreach (Item item in list)
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

            list.RemoveAt(slot);
            Database.removeItem(owner, itemDBId);

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(slot);
            sendInventoryRemovePackets(slot);

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));

        }

        public void removeItem(ushort slot)
        {
            if (slot >= list.Count)
                return;

            list.RemoveAt((int)slot);
            Database.removeItem(owner, slot);

            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));

            sendInventoryPackets(slot);
            sendInventoryRemovePackets(slot);

            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
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

        private void sendInventoryPackets(Item item)
        {
            owner.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, item));
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
                Item item = list[i];
                if (item.itemId == itemId && item.quantity < item.maxStack)
                {
                    quantityCount -= (item.maxStack - item.quantity);
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

            foreach (Item item in list)
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

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
            List<SubPacket> addItemPackets = new List<SubPacket>();
            Item storedItem = null;

            //Check if item id exists 
            foreach (Item item in list)
            {
                if (item.itemId == itemId && item.quantity < 99)
                {
                    storedItem = item;
                    break;
                }
            }
                  
            //If it's unique, abort
           // if (storedItem != null && storedItem.isUnique)
          //      return ITEMERROR_UNIQUE;

            //If Inventory is full
         //   if (storedItem == null && isInventoryFull())
         //       return ITEMERROR_FULL;

            //Update lists and db
            owner.queuePacket(InventoryBeginChangePacket.buildPacket(owner.actorId));
            owner.queuePacket(InventorySetBeginPacket.buildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            if (storedItem == null)
            {
                Item addedItem = Database.addItem(owner, itemId, quantity, quality, false, 100, type);
                list.Add(addedItem);
                sendInventoryPackets(addedItem);
            }
            else
            {
                Database.addQuantity(owner, storedItem.slot, quantity);
                storedItem.quantity += quantity;
                sendInventoryPackets(storedItem);
            }
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
            owner.queuePacket(InventoryEndChangePacket.buildPacket(owner.actorId));
        }

        public void removeItem(uint itemId, uint quantity)
        {

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
            sendInventoryPackets(list);
            owner.queuePacket(InventorySetEndPacket.buildPacket(owner.actorId));
        }

        private void sendInventoryPackets(Item item)
        {
            owner.queuePacket(InventoryListX01Packet.buildPacket(owner.actorId, item));
        }

        private void sendInventoryPackets(List<Item> items)
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
        #endregion

        #region Inventory Utils

        public bool isFull()
        {
            return list.Count >= inventoryCapacity;
        }

        public int getNextEmptySlot()
        {
            return list.Count == 0 ? 0 : list.Count();                
        }

        #endregion

    }
}

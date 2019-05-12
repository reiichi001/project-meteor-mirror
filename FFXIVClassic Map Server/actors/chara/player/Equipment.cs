using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
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

        readonly private Player owner;
        readonly private ushort inventoryCapacity;
        readonly private ushort inventoryCode;
        private InventoryItem[] list;
        readonly private ItemPackage normalInventory;

        private bool writeToDB = true;

        public Equipment(Player ownerPlayer, ItemPackage normalInventory, ushort capacity, ushort code)
        {
            owner = ownerPlayer;
            inventoryCapacity = capacity;
            inventoryCode = code;
            list = new InventoryItem[inventoryCapacity];
            this.normalInventory = normalInventory;
        }

        public void SetEquipment(ushort[] slots, ushort[] itemSlots)
        {
            if (slots.Length != itemSlots.Length)
                return;

            for (int i = 0; i < slots.Length; i++)
            {
                InventoryItem item = normalInventory.GetItemAtSlot(itemSlots[i]);

                if (item == null)
                    continue;

                Database.EquipItem(owner, slots[i], item.uniqueId);
                list[slots[i]] = normalInventory.GetItemAtSlot(itemSlots[i]);
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            SendFullEquipment(owner);
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
            InventoryItem item = normalInventory.GetItemAtSlot(invSlot);

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
                normalInventory.RefreshItem(owner, list[slot], item);            
            else
                normalInventory.RefreshItem(owner, item);

            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            SendSingleEquipmentUpdatePacket(slot, item);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            list[slot] = item;
            owner.CalculateBaseStats();// RecalculateStats();
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


            normalInventory.RefreshItem(owner, list[slot]);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            SendSingleEquipmentUpdatePacket(slot, null);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            list[slot] = null;
            owner.RecalculateStats();
        }

        public InventoryItem GetItemAtSlot(ushort slot)
        {
            if (slot < list.Length)
                return list[slot];
            else
                return null;
        }

        public int GetCapacity()
        {
            return list.Length;
        }

        #region Packet Functions
        private void SendSingleEquipmentUpdatePacket(ushort equipSlot, InventoryItem item)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            if (item == null)
                owner.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, equipSlot));
            else
                owner.QueuePacket(EquipmentListX01Packet.BuildPacket(owner.actorId, equipSlot, item.slot));
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        private void SendEquipmentPackets(List<ushort> slotsToUpdate, Player targetPlayer)
        {            
            int currentIndex = 0;

            while (true)
            {
                if (slotsToUpdate.Count - currentIndex >= 64)
                    targetPlayer.QueuePacket(EquipmentListX64Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 32)
                    targetPlayer.QueuePacket(EquipmentListX32Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 16)
                    targetPlayer.QueuePacket(EquipmentListX16Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex > 1)
                    targetPlayer.QueuePacket(EquipmentListX08Packet.BuildPacket(owner.actorId, list, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex == 1)
                {
                    targetPlayer.QueuePacket(EquipmentListX01Packet.BuildPacket(owner.actorId, slotsToUpdate[currentIndex], list[slotsToUpdate[currentIndex]].slot));
                    currentIndex++;
                }
                else
                    break;
            }
        }

        public void SendFullEquipment()
        {
            SendFullEquipment(owner);
        }

        public void SendFullEquipment(Player targetPlayer)
        {
            List<ushort> slotsToUpdate = new List<ushort>();

            for (ushort i = 0; i < list.Length; i++)
            {
                if (list[i] != null)
                    slotsToUpdate.Add(i);
            }

            if (targetPlayer.Equals(owner))
                targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, inventoryCapacity, inventoryCode));
            else
                targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, 0x23, ItemPackage.EQUIPMENT_OTHERPLAYER));

            SendEquipmentPackets(slotsToUpdate, targetPlayer);

            targetPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        #endregion
    }
}

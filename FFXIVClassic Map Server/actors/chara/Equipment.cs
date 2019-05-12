using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFXIVClassic_Map_Server.actors.chara.player
{
    /// <summary>
    /// This class stores the current equipment that a <c>Player</c> has equipped on themselves. Technically
    /// it is an <c>ItemPackage</c> like other inventories, however due to how this one operates, it is in
    /// it's own class. While on the server this is stored as a list of <c>InventoryItem</c>s like other
    /// ItemPackages, on the client it exists as a link to the "normal inventory" package. The Equipment list
    /// therefore is a list of slot values (the slots in the inventory), with each position in the list being
    /// a position on the paper doll (in the game's Gear menu).
    /// </summary>
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

        private const ushort EQUIP_ITEMPACKAGE_CAPACITY = 0x23;
        private const ushort EQUIP_ITEMPACKAGE_CODE = ItemPackage.EQUIPMENT;

        readonly private InventoryItem[] list = new InventoryItem[EQUIP_ITEMPACKAGE_CAPACITY];

        readonly private Player owner;
        readonly private ItemPackage normalInventory;

        private bool writeToDB = true;

        /// <param name="ownerPlayer">The player client that owns this ItemPackage.</param>
        /// <param name="normalInventory">A reference to the normal inventory ItemPackage this equipment ItemPackage links to.</param>
        public Equipment(Player ownerPlayer, ItemPackage normalInventory)
        {
            owner = ownerPlayer;
            this.normalInventory = normalInventory;
        }

        /// <summary>
        /// Sets the full equipment ItemPackage to the given list. <paramref name="toEquip"/>'s length must
        /// equal EQUIP_ITEMPACKAGE_CAPACITY. Used to initialize the list when loading from the DB.
        /// </summary>
        /// <param name="toEquip">The list of inventory items to set the full list to.</param>
        public void SetEquipment(InventoryItem[] toEquip)
        {
            Debug.Assert(toEquip.Length == EQUIP_ITEMPACKAGE_CAPACITY);
            toEquip.CopyTo(list, 0);
        }

        /// <summary>
        /// Sets the given equipSlots to link to the given itemSlots. The lengths of both must be equal.
        /// </summary>
        /// <param name="equipSlots">The list of equipmentSlots that get linked.</param>
        /// <param name="itemSlots">The list of itemSlots that the equipSlots will link to.</param>        
        public void SetEquipment(ushort[] equipSlots, ushort[] itemSlots)
        {
            if (equipSlots.Length != itemSlots.Length)
                return;

            for (int i = 0; i < equipSlots.Length; i++)
            {
                InventoryItem item = normalInventory.GetItemAtSlot(itemSlots[i]);

                if (item == null)
                    continue;

                Database.EquipItem(owner, equipSlots[i], item.uniqueId);
                list[equipSlots[i]] = normalInventory.GetItemAtSlot(itemSlots[i]);
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            SendFullEquipment(owner);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }

        /// <summary>
        /// Equips the item at the given item slot to the given equipment slot.
        /// </summary>
        /// <param name="equipSlot">The equipment slot being equipped.</param>
        /// <param name="invSlot">The inventory slot of where the equipped item is.</param>   
        public void Equip(ushort equipSlot, ushort invSlot)
        {
            InventoryItem item = normalInventory.GetItemAtSlot(invSlot);

            if (item == null)
                return;

            Equip(equipSlot, item);
        }

        /// <summary>
        /// Equips the given inventory item to the given equipment slot.
        /// </summary>
        /// <param name="equipSlot">The equipment slot being equipped.</param>
        /// <param name="item">The inventory item being equiped.</param> 
        public void Equip(ushort equipSlot, InventoryItem item)
        {
            if (equipSlot >= list.Length)
                return;

            if (writeToDB)
                Database.EquipItem(owner, equipSlot, item.uniqueId);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));

            if (list[equipSlot] != null)            
                normalInventory.RefreshItem(owner, list[equipSlot], item);            
            else
                normalInventory.RefreshItem(owner, item);

            list[equipSlot] = item;

            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, EQUIP_ITEMPACKAGE_CAPACITY, EQUIP_ITEMPACKAGE_CODE));
            SendSingleEquipmentUpdatePacket(equipSlot);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            owner.CalculateBaseStats();// RecalculateStats();
        }

        /// <summary>
        /// Toggles if equipment changes are saved to the DB.
        /// </summary>
        /// <param name="flag">If true, equipment changes are saved to the db.</param>        
        public void ToggleDBWrite(bool flag)
        {
            writeToDB = flag;
        }

        /// <summary>
        /// Removes the linked item at the given <paramref name="equipSlot"/>.
        /// </summary>
        /// <param name="equipSlot">The slot that is being cleared.</param>        
        public void Unequip(ushort equipSlot)
        {
            if (equipSlot >= list.Length)
                return;

            if (writeToDB)
                Database.UnequipItem(owner, equipSlot);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            normalInventory.RefreshItem(owner, list[equipSlot]);
            list[equipSlot] = null;            
            SendSingleEquipmentUpdatePacket(equipSlot);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            owner.RecalculateStats();
        }

        /// <summary>
        /// Returns the item equipped at the given <paramref name="equipSlot"/>.
        /// </summary>
        /// <param name="equipSlot">The slot to retrieve from.</param>     
        public InventoryItem GetItemAtSlot(ushort equipSlot)
        {
            if (equipSlot < list.Length)
                return list[equipSlot];
            else
                return null;
        }

        /// <summary>
        /// Returns the capacity of this ItemPackage.
        /// </summary>
        public int GetCapacity()
        {
            return list.Length;
        }

        #region Packet Functions

        /// <summary>
        /// Syncs the client the link status of a single equipment slot. If the item was null,
        /// sends a delete packet instead.
        /// </summary>
        /// <param name="equipSlot">The slot we are updating the client about.</param>    
        private void SendSingleEquipmentUpdatePacket(ushort equipSlot)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, EQUIP_ITEMPACKAGE_CAPACITY, EQUIP_ITEMPACKAGE_CODE));
            if (list[equipSlot] == null)
                owner.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, equipSlot));
            else
                owner.QueuePacket(EquipmentListX01Packet.BuildPacket(owner.actorId, equipSlot, list[equipSlot].slot));
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        /// <summary>
        /// Syncs the full list of equipped items to the client owner of this ItemPackage. Used on login/zone in.
        /// </summary>
        public void SendFullEquipment()
        {
            SendFullEquipment(owner);
        }

        /// <summary>
        /// Syncs the full list of equipped items to a given target. Used to send the equipment list of this ItemPackage to the owner,
        /// or in the case of examining another player, sends the list of this ItemPackage to the player client examining. A different
        /// ItemPackage Code is used for /checking.
        /// </summary>
        /// <param name="targetPlayer">The player client that is being synced.</param> 
        public void SendFullEquipment(Player targetPlayer)
        {
            List<ushort> slotsToUpdate = new List<ushort>();

            for (ushort i = 0; i < list.Length; i++)
            {
                if (list[i] != null)
                    slotsToUpdate.Add(i);
            }

            if (targetPlayer.Equals(owner))
                targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, EQUIP_ITEMPACKAGE_CAPACITY, EQUIP_ITEMPACKAGE_CODE));
            else
                targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, 0x23, ItemPackage.EQUIPMENT_OTHERPLAYER));

            SendEquipmentPackets(slotsToUpdate, targetPlayer);

            targetPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        /// <summary>
        /// Main sync function. Syncs the given targetPlayer client the link status of multiple equipment slots.
        /// </summary>
        /// <param name="slotsToUpdate">The slots that will be synced.</param> 
        /// <param name="targetPlayer">The player client that is being synced.</param> 
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

        #endregion
    }
}

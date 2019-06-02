using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFXIVClassic_Map_Server.actors.chara
{

    class ReferencedItemPackage
    {
        const uint EMPTY = 0xFFFFFFFF;

        private readonly Player owner;
        private readonly uint[] contentList;
        private readonly ushort itemPackageCode;
        private readonly ushort itemPackageCapacity;
        private bool writeToDB = false;

        public ReferencedItemPackage(Player owner, ushort capacity, ushort code)
        {
            this.owner = owner;
            itemPackageCode = code;
            itemPackageCapacity = capacity;
            contentList = new uint[capacity];

            if (code == ItemPackage.EQUIPMENT)
                writeToDB = true;

            for (int i = 0; i < contentList.Length; i++)
                contentList[i] = EMPTY;
        }

        public void ToggleDBWrite(bool flag)
        {
            writeToDB = flag;
        }

        #region Package Management
        public void SetList(uint[] toSet)
        {
            Debug.Assert(contentList.Length == itemPackageCapacity);
            toSet.CopyTo(contentList, 0);
        }

        public void SetList(ushort[] positions, uint[] values)
        {
            Debug.Assert(positions.Length == values.Length);
           
            for (int i = 0; i < positions.Length; i++)
            {
                InventoryItem item = GetItem(values[i]);

                if (item == null)
                    continue;

                //Database.EquipItem(owner, positions[i], item.uniqueId);
                contentList[positions[i]] = values[i];
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            SendUpdate();
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }
        
        public void Set(ushort position, ushort itemPackagePosition, ushort itemPackageCode)
        {
            InventoryItem item = owner.GetItemPackage(itemPackageCode).GetItemAtSlot(itemPackagePosition);

            if (item == null)
                return;

            Set(position, item);
        }

        public void Set(ushort position, InventoryItem item)
        {
            if (position >= contentList.Length)
                return;

            if (writeToDB)
                Database.EquipItem(owner, position, item.uniqueId);

            ItemPackage newPackage = owner.GetItemPackage(item.itemPackage);
            ItemPackage oldPackage = GetItemPackage(contentList[position]);
            InventoryItem oldItem = GetItem(contentList[position]);

            if (oldPackage != null && oldItem != null)
                oldPackage.MarkDirty(oldItem);
            newPackage.MarkDirty(item);

            contentList[position] = GetValue(item.itemPackage, item.slot);

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            if (oldPackage != null)
                oldPackage.SendUpdate();
            newPackage.SendUpdate();
            SendSingleUpdate(position);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            owner.CalculateBaseStats();// RecalculateStats();
        }

        public void Clear(ushort position)
        {
            if (position >= contentList.Length)
                return;

            if (writeToDB)
                Database.UnequipItem(owner, position);

            ItemPackage oldItemPackage = GetItemPackage(contentList[position]);

            oldItemPackage.MarkDirty(GetItem(contentList[position]));
            contentList[position] = EMPTY;

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            oldItemPackage.SendUpdate();
            SendSingleUpdate(position);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            owner.RecalculateStats();
        }

        public void ClearAll()
        {
            List<ItemPackage> packagesToRefresh = new List<ItemPackage>();

            for (int i = 0; i < contentList.Length; i++)
            {
                if (contentList[i] == EMPTY)
                    continue;

                if (writeToDB)
                    Database.UnequipItem(owner, (ushort)i);

                ItemPackage package = GetItemPackage(contentList[i]);               
                package.MarkDirty(GetItem(contentList[i]));            
                packagesToRefresh.Add(package);

                contentList[i] = EMPTY;               
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            for (int i = 0; i < packagesToRefresh.Count; i++)
                packagesToRefresh[i].SendUpdate();
            SendUpdate();
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));

            owner.RecalculateStats();
        }
        #endregion

        #region Send Update Functions
        public void SendSingleUpdate(ushort position)
        {
            owner.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, itemPackageCapacity, itemPackageCode));
            SendSingleLinkedItemPacket(owner, position);
            owner.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void SendUpdate()
        {
            SendUpdate(owner);
        }

        public void SendUpdate(Player targetPlayer)
        {
            List<ushort> slotsToUpdate = new List<ushort>();

            for (ushort i = 0; i < contentList.Length; i++)
            {
                if (contentList[i] != EMPTY)
                    slotsToUpdate.Add(i);
            }
         
            targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, itemPackageCapacity, itemPackageCode));
            SendLinkedItemPackets(targetPlayer, slotsToUpdate);
            targetPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }

        public void SendUpdateAsItemPackage(Player targetPlayer)
        {
            SendUpdateAsItemPackage(targetPlayer, itemPackageCapacity, itemPackageCode);
        }

        public void SendUpdateAsItemPackage(Player targetPlayer, ushort destinationCapacity, ushort destinationCode)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            for (int i = 0; i < contentList.Length; i++)
            {
                if (contentList[i] == EMPTY)
                    continue;
                items.Add(GetItem(contentList[i]));
            }
            
            targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, destinationCapacity, destinationCode));         
            SendItemPackets(targetPlayer, items);
            targetPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));
        }
        #endregion

        #region Packet Functions (Private)
        private void SendSingleLinkedItemPacket(Player targetPlayer, ushort position)
        {
            if (contentList[position] == EMPTY)
                targetPlayer.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, position));
            else
                targetPlayer.QueuePacket(LinkedItemListX01Packet.BuildPacket(owner.actorId, position, contentList[position]));
        }

        private void SendLinkedItemPackets(Player targetPlayer, List<ushort> slotsToUpdate)
        {
            int currentIndex = 0;

            while (true)
            {
                if (slotsToUpdate.Count - currentIndex >= 64)
                    targetPlayer.QueuePacket(LinkedItemListX64Packet.BuildPacket(owner.actorId, contentList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 32)
                    targetPlayer.QueuePacket(LinkedItemListX32Packet.BuildPacket(owner.actorId, contentList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 16)
                    targetPlayer.QueuePacket(LinkedItemListX16Packet.BuildPacket(owner.actorId, contentList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex > 1)
                    targetPlayer.QueuePacket(LinkedItemListX08Packet.BuildPacket(owner.actorId, contentList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex == 1)
                {
                    targetPlayer.QueuePacket(LinkedItemListX01Packet.BuildPacket(owner.actorId, slotsToUpdate[currentIndex], contentList[slotsToUpdate[currentIndex]]));
                    currentIndex++;
                }
                else
                    break;
            }
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
        #endregion

        #region Getters/Setters
        public ushort GetCode()
        {
            return itemPackageCode;
        }

        public int GetCapacity()
        {
            return itemPackageCapacity;
        }

        public Player GetOwner()
        {
            return owner;
        }

        public InventoryItem GetItemAtSlot(ushort position)
        {
            if (position < contentList.Length)
                return GetItem(contentList[position]);
            else
                return null;
        }
        #endregion

        #region Utils
        private ItemPackage GetItemPackage(uint value)
        {
            if (value == EMPTY)
                return null;
            return owner.GetItemPackage((ushort)((value >> 16) & 0xFFFF));
        }

        private InventoryItem GetItem(uint value)
        {
            if (value == EMPTY)
                return null;
            ItemPackage package = GetItemPackage(value);
            if (package != null)
                return package.GetItemAtSlot((ushort)(value & 0xFFFF));
            return null;
        }

        private uint GetValue(ushort code, ushort slot)
        {
            return (uint) ((code << 16) | slot);
        }
        #endregion
    }
}

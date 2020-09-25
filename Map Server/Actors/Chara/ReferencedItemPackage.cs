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

using Meteor.Map.actors.chara.player;
using Meteor.Map.Actors;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.actor.inventory;
using System.Collections.Generic;
using System.Diagnostics;

namespace Meteor.Map.actors.chara
{

    class ReferencedItemPackage
    {
        const uint EMPTY = 0xFFFFFFFF;

        private readonly Player owner;
        private readonly InventoryItem[] referenceList;
        private readonly ushort itemPackageCode;
        private readonly ushort itemPackageCapacity;
        private bool writeToDB = false;

        public ReferencedItemPackage(Player owner, ushort capacity, ushort code)
        {
            this.owner = owner;
            itemPackageCode = code;
            itemPackageCapacity = capacity;
            referenceList = new InventoryItem[capacity];

            if (code == ItemPackage.EQUIPMENT)
                writeToDB = true;
        }

        public void ToggleDBWrite(bool flag)
        {
            writeToDB = flag;
        }

        #region Package Management
        public void SetList(InventoryItem[] toSet)
        {
            Debug.Assert(referenceList.Length == itemPackageCapacity);
            toSet.CopyTo(referenceList, 0);
        }

        public void Set(ushort[] positions, ushort[] itemSlots, ushort itemPackage)
        {
            Debug.Assert(positions.Length == itemSlots.Length);
           
            for (int i = 0; i < positions.Length; i++)
            {
                InventoryItem item = owner.GetItemPackage(itemPackage)?.GetItemAtSlot(itemSlots[i]);

                if (item == null)
                    continue;

                Database.EquipItem(owner, positions[i], item.uniqueId);
                referenceList[positions[i]] = item;
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
            if (position >= referenceList.Length)
                return;

            if (writeToDB)
                Database.EquipItem(owner, position, item.uniqueId);

            ItemPackage newPackage = owner.GetItemPackage(item.itemPackage);
            ItemPackage oldPackage = null;

            if (referenceList[position] != null)
            {
                oldPackage = owner.GetItemPackage(referenceList[position].itemPackage);
                InventoryItem oldItem = referenceList[position];
                oldPackage.MarkDirty(oldItem);
            }
            
            newPackage.MarkDirty(item);

            referenceList[position] = item;

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            if (oldPackage != null)
                oldPackage.SendUpdate();
            newPackage.SendUpdate();
            SendSingleUpdate(position);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));            
        }

        public void Clear(ushort position)
        {
            if (position >= referenceList.Length)
                return;

            if (writeToDB)
                Database.UnequipItem(owner, position);

            ItemPackage oldItemPackage = owner.GetItemPackage(referenceList[position].itemPackage);

            oldItemPackage.MarkDirty(referenceList[position]);
            referenceList[position] = null;

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            oldItemPackage.SendUpdate();
            SendSingleUpdate(position);
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
        }

        public void ClearAll()
        {
            List<ItemPackage> packagesToRefresh = new List<ItemPackage>();

            for (int i = 0; i < referenceList.Length; i++)
            {
                if (referenceList[i] == null)
                    continue;

                if (writeToDB)
                    Database.UnequipItem(owner, (ushort)i);

                ItemPackage package = owner.GetItemPackage(referenceList[i].itemPackage);               
                package.MarkDirty(referenceList[i]);            
                packagesToRefresh.Add(package);

                referenceList[i] = null;               
            }

            owner.QueuePacket(InventoryBeginChangePacket.BuildPacket(owner.actorId));
            for (int i = 0; i < packagesToRefresh.Count; i++)
                packagesToRefresh[i].SendUpdate();
            SendUpdate();
            owner.QueuePacket(InventoryEndChangePacket.BuildPacket(owner.actorId));
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

            for (ushort i = 0; i < referenceList.Length; i++)
            {
                if (referenceList[i] != null)
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

            for (ushort i = 0; i < referenceList.Length; i++)
            {
                if (referenceList[i] == null)
                    continue;

                InventoryItem item = referenceList[i];
                item.linkSlot = i; //We have to set the linkSlot as this is the position in the Referenced IP, not the original IP it's linked from.
                items.Add(referenceList[i]);
            }
            
            targetPlayer.QueuePacket(InventorySetBeginPacket.BuildPacket(owner.actorId, destinationCapacity, destinationCode));         
            SendItemPackets(targetPlayer, items);
            targetPlayer.QueuePacket(InventorySetEndPacket.BuildPacket(owner.actorId));

            //Clean Up linkSlots
            for (ushort i = 0; i < referenceList.Length; i++)
            {
                if (referenceList[i] == null)
                    continue;
                InventoryItem item = referenceList[i];
                item.linkSlot = 0xFFFF;
            }
        }
        #endregion

        #region Packet Functions (Private)
        private void SendSingleLinkedItemPacket(Player targetPlayer, ushort position)
        {
            if (referenceList[position] == null)
                targetPlayer.QueuePacket(InventoryRemoveX01Packet.BuildPacket(owner.actorId, position));
            else
                targetPlayer.QueuePacket(LinkedItemListX01Packet.BuildPacket(owner.actorId, position, referenceList[position]));
        }

        private void SendLinkedItemPackets(Player targetPlayer, List<ushort> slotsToUpdate)
        {
            int currentIndex = 0;

            while (true)
            {
                if (slotsToUpdate.Count - currentIndex >= 64)
                    targetPlayer.QueuePacket(LinkedItemListX64Packet.BuildPacket(owner.actorId, referenceList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 32)
                    targetPlayer.QueuePacket(LinkedItemListX32Packet.BuildPacket(owner.actorId, referenceList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex >= 16)
                    targetPlayer.QueuePacket(LinkedItemListX16Packet.BuildPacket(owner.actorId, referenceList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex > 1)
                    targetPlayer.QueuePacket(LinkedItemListX08Packet.BuildPacket(owner.actorId, referenceList, slotsToUpdate, ref currentIndex));
                else if (slotsToUpdate.Count - currentIndex == 1)
                {
                    targetPlayer.QueuePacket(LinkedItemListX01Packet.BuildPacket(owner.actorId, slotsToUpdate[currentIndex], referenceList[slotsToUpdate[currentIndex]]));
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
            if (position < referenceList.Length)
                return referenceList[position];
            else
                return null;
        }
        #endregion        
    }
}

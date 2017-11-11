
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class Character:Actor
    {
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int MAINHAND = 5;
        public const int OFFHAND = 6;
        public const int SPMAINHAND = 7;
        public const int SPOFFHAND = 8;
        public const int THROWING = 9;
        public const int PACK = 10;
        public const int POUCH = 11;
        public const int HEADGEAR = 12;
        public const int BODYGEAR = 13;
        public const int LEGSGEAR = 14;
        public const int HANDSGEAR = 15;
        public const int FEETGEAR = 16;
        public const int WAISTGEAR = 17;
        public const int NECKGEAR = 18;
        public const int L_EAR = 19;
        public const int R_EAR = 20;
        public const int R_WRIST = 21;
        public const int L_WRIST = 22;
        public const int R_RINGFINGER = 23;
        public const int L_RINGFINGER = 24;
        public const int R_INDEXFINGER = 25;
        public const int L_INDEXFINGER = 26;
        public const int UNKNOWN = 27;

        public bool isStatic = false;

        public uint modelId;
        public uint[] appearanceIds = new uint[28];

        public uint animationId = 0;

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public uint currentActorIcon = 0;

        public Work work = new Work();
        public CharaWork charaWork = new CharaWork();

        public Group currentParty = null;
        public ContentGroup currentContentGroup = null;

        //Inventory        
        protected Dictionary<ushort, Inventory> itemPackages = new Dictionary<ushort, Inventory>();
        private Equipment equipment;

        public Character(uint actorID) : base(actorID)
        {            
            //Init timer array to "notimer"
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
                charaWork.statusShownTime[i] = 0xFFFFFFFF;
        }

        public SubPacket CreateAppearancePacket()
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelId, appearanceIds);
            return setappearance.BuildPacket(actorId);
        }

        public SubPacket CreateInitStatusPacket()
        {
            return (SetActorStatusAllPacket.BuildPacket(actorId, charaWork.status));                      
        }

        public SubPacket CreateSetActorIconPacket()
        {
            return SetActorIconPacket.BuildPacket(actorId, currentActorIcon);
        }

        public SubPacket CreateIdleAnimationPacket()
        {
            return SetActorSubStatPacket.BuildPacket(actorId, 0, 0, 0, 0, 0, 0, animationId);
        }

        public void SetQuestGraphic(Player player, int graphicNum)
        {
            player.QueuePacket(SetActorQuestGraphicPacket.BuildPacket(actorId, graphicNum));
        }

        public void SetCurrentContentGroup(ContentGroup group)
        {
            if (group != null)
                charaWork.currentContentGroup = group.GetTypeId();
            else
                charaWork.currentContentGroup = 0;

            currentContentGroup = group;

            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("charaWork/currentContentGroup", this);
            propPacketUtil.AddProperty("charaWork.currentContentGroup");            
            zone.BroadcastPacketsAroundActor(this, propPacketUtil.Done());

        }     
   
        public void PlayAnimation(uint animId, bool onlySelf = false)
        {            
            if (onlySelf)
            {
                if (this is Player)
                    ((Player)this).QueuePacket(PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
            }
            else
                zone.BroadcastPacketAroundActor(this, PlayAnimationOnActorPacket.BuildPacket(actorId, animId));
        }
        
        #region Inventory

        public void AddItem(uint catalogID)
        {
            AddItem(catalogID, 1);
        }

        public void AddItem(uint catalogID, int quantity)
        {
            AddItem(catalogID, quantity, 1);
        }

        public void AddItem(uint catalogID, int quantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                InventoryItem item = Server.GetWorldManager().CreateItem(catalogID, quantity, quality);
                itemPackages[itemPackage].AddItem(item);
            }
        }

        public void AddItem(InventoryItem item)
        {
            ushort itemPackage = GetPackageForItem(item.GetItemData().catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {                
                itemPackages[itemPackage].AddItem(item);
            }
        }

        public void SetItem(InventoryItem item, ushort itemPackage, ushort slot)
        {
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].AddItemSpecial(slot, item);
            }
        }

        public void MoveItem(InventoryItem item, ushort destinationPackage)
        {
            ushort sourcePackage = item.itemPackage;

            if (!itemPackages.ContainsKey(sourcePackage) && !itemPackages.ContainsKey(destinationPackage))
                return;

            itemPackages[sourcePackage].RemoveItem(item);
            itemPackages[destinationPackage].AddItem(item);            
        }
        
        public void RemoveItem(uint catalogID)
        {
            RemoveItem(catalogID, 1);
        }

        public void RemoveItem(uint catalogID, int quantity)
        {
            RemoveItem(catalogID, quantity, 1);
        }

        public void RemoveItem(uint catalogID, int quantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].RemoveItem(catalogID, quantity, quantity);
            }
        }

        public void RemoveItemAtSlot(ushort itemPackage, ushort slot)
        {
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].RemoveItemAtSlot(slot);
            }
        }

        public void RemoveItem(InventoryItem item)
        {
            RemoveItem(item, 1);
        }

        public void RemoveItem(InventoryItem item, int quantity)
        {
            if (itemPackages.ContainsKey(item.itemPackage))
            {
                itemPackages[item.itemPackage].RemoveItem(item, quantity);
            }
        }

        public bool HasItem(uint catalogID)
        {
            return HasItem(catalogID, 1);
        }

        public bool HasItem(uint catalogID, int minQuantity)
        {
            return HasItem(catalogID, minQuantity, 1);
        }

        public bool HasItem(uint catalogID, int minQuantity, byte quality)
        {
            ushort itemPackage = GetPackageForItem(catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                itemPackages[itemPackage].HasItem(catalogID, minQuantity, quality);
            }
            return false;
        }

        public bool HasItem(InventoryItem item)
        {
            ushort itemPackage = GetPackageForItem(item.GetItemData().catalogID);
            if (itemPackages.ContainsKey(itemPackage))
            {
                //return itemPackages[itemPackage].HasItem(item);
                return false; //TODO FIX
            }
            return false;
        }

        public InventoryItem GetItem(LuaUtils.ItemRefParam reference)
        {
            if (reference.actorId != actorId)
                return null;
            if (itemPackages.ContainsKey(reference.itemPackage))
            {
                return itemPackages[reference.itemPackage].GetItemAtSlot(reference.slot);
            }
            return null;
        }

        public ushort GetPackageForItem(uint catalogID)
        {
            ItemData data = Server.GetItemGamedata(catalogID);

            if (data == null)
                return Inventory.NORMAL;
            else
            {                
                if (data.IsMoney())
                    return Inventory.CURRENCY_CRYSTALS;
                else if (data.IsImportant())
                    return Inventory.KEYITEMS;
                else
                    return Inventory.NORMAL;
            }
        }

        #endregion

        //public void removeItem(byUniqueId)
        //public void removeItem(byUniqueId, quantity)
        //public void removeItem(slot)
        //public void removeItem(slot, quantity)

    }

}

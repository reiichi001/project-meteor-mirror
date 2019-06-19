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
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Meteor.Map.dataobjects
{
    class InventoryItem
    {
        public const byte DEALINGMODE_NONE = 0;
        public const byte DEALINGMODE_REFERENCED = 1;
        public const byte DEALINGMODE_PRICED = 2;

        public const byte TAG_EXCLUSIVE = 0x3;
        public const byte TAG_DEALING = 0xC9;
        public const byte TAG_ATTACHED = 0xCA;

        public const byte MODE_SELL_SINGLE = 11; //0xB
        public const byte MODE_SELL_PSTACK = 12; //0xC
        public const byte MODE_SELL_FSTACK = 13; //0xD
        public const byte MODE_SEEK_ITEM = 20; //0x14
        public const byte MODE_SEEK_REPAIR = 30; //0x1E
        public const byte MODE_SEEK_MELD = 40; //0x28

        public ulong uniqueId;
        public uint itemId;
        public int quantity = 1;

        private byte dealingVal      = 0;
        private byte dealingMode     = DEALINGMODE_NONE;
        private int dealingAttached1 = 0;
        private int dealingAttached2 = 0;
        private int dealingAttached3 = 0;

        private byte[] tags = new byte[4];
        private byte[] tagValues = new byte[4];

        public byte quality = 1;
        
        public ItemModifier modifiers;

        public readonly ItemData itemData;
        public Character owner = null;
        public ushort slot = 0xFFFF;
        public ushort linkSlot = 0xFFFF;
        public ushort itemPackage = 0xFFFF;

        public class ItemModifier
        {
            public uint durability = 0;
            public ushort use = 0;
            public uint materiaId = 0;
            public uint materiaLife = 0;
            public byte mainQuality = 0;
            public byte[] subQuality = new byte[3];
            public uint polish = 0;
            public uint param1 = 0;
            public uint param2 = 0;
            public uint param3 = 0;
            public ushort spiritbind = 0;
            public byte[] materiaType = new byte[5];
            public byte[] materiaGrade = new byte[5];

            public ItemModifier()
            {                
            }

            public ItemModifier(MySql.Data.MySqlClient.MySqlDataReader reader)
            {
                durability = reader.GetUInt32("durability"); 
                mainQuality = reader.GetByte("mainQuality");
                subQuality[0] = reader.GetByte("subQuality1");
                subQuality[1] = reader.GetByte("subQuality2");
                subQuality[2] = reader.GetByte("subQuality3");
                param1 = reader.GetUInt32("param1");
                param2 = reader.GetUInt32("param2");
                param3 = reader.GetUInt32("param3");
                spiritbind = reader.GetUInt16("spiritbind");

                ushort materia1 = reader.GetUInt16("materia1");
                ushort materia2 = reader.GetUInt16("materia2");
                ushort materia3 = reader.GetUInt16("materia3");
                ushort materia4 = reader.GetUInt16("materia4");
                ushort materia5 = reader.GetUInt16("materia5");

                materiaType[0] = (byte)(materia1 & 0xFF);
                materiaGrade[0] = (byte)((materia1 >> 8) & 0xFF);
                materiaType[1] = (byte)(materia2 & 0xFF);
                materiaGrade[1] = (byte)((materia2 >> 8) & 0xFF);
                materiaType[2] = (byte)(materia3 & 0xFF);
                materiaGrade[2] = (byte)((materia3 >> 8) & 0xFF);
                materiaType[3] = (byte)(materia4 & 0xFF);
                materiaGrade[3] = (byte)((materia4 >> 8) & 0xFF);
                materiaType[4] = (byte)(materia5 & 0xFF);
                materiaGrade[4] = (byte)((materia5 >> 8) & 0xFF);
            }

            public void WriteBytes(BinaryWriter binWriter)
            {
                binWriter.Write((UInt32) durability);
                binWriter.Write((UInt16) use);
                binWriter.Write((UInt32) materiaId);
                binWriter.Write((UInt32) materiaLife);
                binWriter.Write((Byte)   mainQuality);
                binWriter.Write((Byte)   subQuality[0]);
                binWriter.Write((Byte)   subQuality[1]);
                binWriter.Write((Byte)   subQuality[2]);
                binWriter.Write((UInt32) polish);
                binWriter.Write((UInt32) param1);
                binWriter.Write((UInt32) param2);
                binWriter.Write((UInt32) param3);
                binWriter.Write((UInt16) spiritbind);
                binWriter.Write((Byte)   materiaType[0]);
                binWriter.Write((Byte)   materiaType[1]);
                binWriter.Write((Byte)   materiaType[2]);
                binWriter.Write((Byte)   materiaType[3]);
                binWriter.Write((Byte)   materiaType[4]);
                binWriter.Write((Byte)   materiaGrade[0]);
                binWriter.Write((Byte)   materiaGrade[1]);
                binWriter.Write((Byte)   materiaGrade[2]);
                binWriter.Write((Byte)   materiaGrade[3]);
                binWriter.Write((Byte)   materiaGrade[4]);
            }
        }
        
        //For loading already existing items
        public InventoryItem(MySqlDataReader reader)
        {
            uniqueId = reader.GetUInt32("serverItemId");
            itemId = reader.GetUInt32("itemId");
            itemData = Server.GetItemGamedata(itemId);
            quantity = reader.GetInt32("quantity");
            quality = reader.GetByte("quality");

            bool hasDealing = !reader.IsDBNull(reader.GetOrdinal("bazaarMode"));
            if (hasDealing)
            {
                dealingVal = reader.GetByte("dealingValue");
                dealingMode = reader.GetByte("dealingMode");
                dealingAttached1 = reader.GetInt32("dealingAttached1");
                dealingAttached2 = reader.GetInt32("dealingAttached2");
                dealingAttached3 = reader.GetInt32("dealingAttached3");
                tags[0] = reader.GetByte("dealingTag");
                tagValues[0] = reader.GetByte("bazaarMode");
            }

            bool hasModifiers = !reader.IsDBNull(reader.GetOrdinal("modifierId"));
            if (hasModifiers)
                modifiers = new InventoryItem.ItemModifier(reader);

            tags[1] = itemData.isExclusive ? TAG_EXCLUSIVE : (byte)0;
        }

        //For creating new items (only should be called by the DB)
        public InventoryItem(uint uniqueId, uint itemId, int quantity, byte qualityNumber, ItemModifier modifiers = null)
        {
            this.uniqueId = uniqueId;
            this.itemId = itemId;
            this.itemData = Server.GetItemGamedata(itemId);
            this.quantity = quantity;
            this.quality = qualityNumber;
            this.modifiers = modifiers;

            tags[1] = itemData.isExclusive ? TAG_EXCLUSIVE : (byte)0;
        }

        public void SaveDealingInfo(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@serverItemId", uniqueId);
            cmd.Parameters.AddWithValue("@dealingValue", dealingVal);
            cmd.Parameters.AddWithValue("@dealingMode", dealingMode);
            cmd.Parameters.AddWithValue("@dealingAttached1", dealingAttached1);
            cmd.Parameters.AddWithValue("@dealingAttached2", dealingAttached2);
            cmd.Parameters.AddWithValue("@dealingAttached3", dealingAttached3);
            cmd.Parameters.AddWithValue("@dealingTag", tags[0]);
            cmd.Parameters.AddWithValue("@bazaarMode", tagValues[0]);
        }

        public byte[] ToPacketBytes()
        {           
            byte[] data = new byte[0x70];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)uniqueId);
                    binWriter.Write((Int32)quantity);
                    binWriter.Write((UInt32)itemId);

                    if (linkSlot == 0xFFFF)
                        binWriter.Write((UInt16)slot);
                    else
                        binWriter.Write((UInt16)linkSlot);
                    linkSlot = 0xFFFF;

                    binWriter.Write((Byte)dealingVal);
                    binWriter.Write((Byte)dealingMode);

                    binWriter.Write((UInt32)dealingAttached1);
                    binWriter.Write((UInt32)dealingAttached2);
                    binWriter.Write((UInt32)dealingAttached3);

                    for (int i = 0; i < tags.Length; i++)
                        binWriter.Write((Byte) tags[i]);
                    for (int i = 0; i < tagValues.Length; i++)
                        binWriter.Write((Byte) tagValues[i]);

                    binWriter.Write((Byte)quality);
                                        
                    if (modifiers != null)
                    {
                        binWriter.Write((Byte)0x01);
                        modifiers.WriteBytes(binWriter);
                    }                  
                }                
            }

            return data;        
        }

        public void SetQuantity(uint quantity)
        {
            lock (owner.GetItemPackage(itemPackage))
            {
                this.quantity = (int)quantity;
                if (quantity < 0)
                    quantity = 0;
                Database.SetQuantity(uniqueId, this.quantity);

                if (owner != null)
                    owner.GetItemPackage(itemPackage).MarkDirty(this);
            }
        }

        public void ChangeQuantity(int quantityDelta)
        {
            lock (owner.GetItemPackage(itemPackage))
            {
                this.quantity += quantityDelta;
                if (quantity < 0)
                    quantity = 0;

                if (quantity == 0)
                {
                    owner.RemoveItem(this);
                    return;
                }

                Database.SetQuantity(uniqueId, this.quantity);

                if (owner != null)
                    owner.GetItemPackage(itemPackage).MarkDirty(this);       
            }
        }

        public void SetOwner(Character owner, ushort itemPackage, ushort slot)
        {
            this.owner = owner;
            this.itemPackage = itemPackage;
            this.slot = slot;            
        }    

        public void ClearOwner()
        {
            owner = null;
            itemPackage = 0xFFFF;
            slot = 0xFFFF;
        }
        
        public void SetNormal()
        {           
            if (dealingMode != 0 || tags[0] == TAG_ATTACHED)
                Database.ClearDealingInfo(this);

            tags[0] = 0;
            tagValues[0] = 0;
            dealingVal = 0;
            dealingMode = 0;
            dealingAttached1 = 0;
            dealingAttached2 = 0;
            dealingAttached3 = 0;
            
            if (owner != null)
                owner.GetItemPackage(itemPackage).MarkDirty(this);
        }        

        public void SetSelling(byte mode, int price)
        {
            tags[0] = TAG_DEALING;
            tagValues[0] = mode;
            
            dealingVal = 0;
            dealingMode = DEALINGMODE_PRICED;
            dealingAttached1 = 0;
            dealingAttached2 = price;
            dealingAttached3 = 0;
           
            if (owner != null)
                owner.GetItemPackage(itemPackage).MarkDirty(this);

            Database.SetDealingInfo(this);
        }

        public void SetAsOfferTo(byte mode, InventoryItem seeked)
        {
            tags[0] = TAG_DEALING;
            tagValues[0] = mode;

            dealingVal = 0;
            dealingMode = DEALINGMODE_REFERENCED;
            dealingAttached1 = ((seeked.itemPackage << 16) | seeked.slot);
            dealingAttached2 = 0;
            dealingAttached3 = 0;

            seeked.SetSeeking();

            if (owner != null)
                owner.GetItemPackage(itemPackage).MarkDirty(this);

            Database.SetDealingInfo(this);
        }

        public void UpdateOfferedSlot(ushort delta)
        {
            if (dealingMode == DEALINGMODE_REFERENCED)
            {
                ushort attachedItemPackage = (ushort)((dealingAttached1 >> 16) & 0xFF);
                ushort attachedSlot = (ushort)(dealingAttached1 & 0xFF);
                attachedSlot -= delta;
                dealingAttached1 = ((attachedItemPackage << 16) | attachedSlot);
                Database.SetDealingInfo(this);
            }
        }

        protected void SetSeeking()
        {
            tags[0] = TAG_ATTACHED;
            tagValues[0] = 0;

            dealingVal = 0;
            dealingMode = DEALINGMODE_NONE;
            dealingAttached1 = 0;
            dealingAttached2 = 0;
            dealingAttached3 = 0;

            if (owner != null)
                owner.GetItemPackage(itemPackage).MarkDirty(this);

            Database.SetDealingInfo(this);
        }

        public void SetTradeQuantity(int quantity)
        {
            tags[0] = 0;
            tagValues[0] = 0;

            dealingVal = 0;
            dealingMode = DEALINGMODE_NONE;
            dealingAttached1 = 0;
            dealingAttached2 = 0;
            dealingAttached3 = quantity;

            if (owner != null)
                owner.GetItemPackage(itemPackage).MarkDirty(this);
        }

        public int GetTradeQuantity()
        {
            return dealingAttached3;
        }

        public InventoryItem GetOfferedTo()
        {
            if (dealingMode != DEALINGMODE_REFERENCED)
                return null;

            ushort attachedItemPackage = (ushort)((dealingAttached1 >> 16) & 0xFF);
            ushort attachedSlot = (ushort)(dealingAttached1 & 0xFF);
            return owner.GetItemPackage(attachedItemPackage).GetItemAtSlot(attachedSlot);
        }

        public bool IsSelling()
        {
            return GetBazaarMode() == MODE_SELL_SINGLE || GetBazaarMode() == MODE_SELL_PSTACK || GetBazaarMode() == MODE_SELL_FSTACK;
        }

        public byte GetBazaarMode()
        {
            if (tags[0] == 0xC9)
                return tagValues[0];
            return 0;
        }

        public ItemData GetItemData()
        {
            return itemData;
        }

        public override string ToString()
        {
            if (itemData != null)
            {
                if (quantity <= 1)
                    return string.Format("{0}+{1} ({2}/{3})", itemData.name, quality-1, quantity, itemData.maxStack);
                else
                    return string.Format("{0} ({1}/{2})", itemData.name, quantity, itemData.maxStack);
            }
            else
                return "Invalid Item";
        }

    }
}

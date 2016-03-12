using FFXIVClassic_Lobby_Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class InventoryItem
    {
        public ulong uniqueId;
        public uint itemId;
        public int quantity = 1;
        public ushort slot;

        public byte itemType;
        public byte quality = 1;

        public int durability = 0;
        public ushort spiritbind = 0;

        public byte materia1 = 0;
        public byte materia2 = 0;
        public byte materia3 = 0;
        public byte materia4 = 0;
        public byte materia5 = 0;

        //Bare Minimum
        public InventoryItem(uint id, uint itemId, ushort slot)
        {
            this.uniqueId = id;
            this.itemId = itemId;
            this.quantity = 1;
            this.slot = slot;

            Item gItem = Server.getItemGamedata(itemId);
            itemType = gItem.isExclusive ? (byte)0x3 : (byte)0x0;
        }

        //For check command
        public InventoryItem(InventoryItem item, ushort equipSlot)
        {
            this.uniqueId = item.uniqueId;
            this.itemId = item.itemId;
            this.quantity = item.quantity;
            this.slot = equipSlot;

            this.itemType = item.itemType;
            this.quality = item.quality;

            this.durability = item.durability;
            this.spiritbind = item.spiritbind;

            this.materia1 = item.materia1;
            this.materia2 = item.materia2;
            this.materia3 = item.materia3;
            this.materia4 = item.materia4;
            this.materia5 = item.materia5;
        }

        public InventoryItem(uint uniqueId, uint itemId, int quantity, ushort slot, byte itemType, byte qualityNumber, int durability, ushort spiritbind, byte materia1, byte materia2, byte materia3, byte materia4, byte materia5)
        {
            this.uniqueId = uniqueId;
            this.itemId = itemId;
            this.quantity = quantity;
            this.slot = slot;
            this.itemType = itemType;
            this.quality = qualityNumber;
            this.durability = durability;
            this.spiritbind = spiritbind;
            this.materia1 = materia1;
            this.materia2 = materia2;
            this.materia3 = materia3;
            this.materia4 = materia4;
            this.materia5 = materia5;
        }

        public byte[] toPacketBytes()
        {           
            byte[] data = new byte[0x70];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)uniqueId);
                    binWriter.Write((Int32)quantity);
                    binWriter.Write((UInt32)itemId);
                    binWriter.Write((UInt16)slot);

                    binWriter.Write((UInt16)0x0001);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);

                    binWriter.Write((UInt32)itemType);

                    binWriter.Write((UInt32)0x00000000);

                    binWriter.Write((byte)quality);
                    binWriter.Write((byte)0x01);
                    binWriter.Write((uint)durability);

                    binWriter.BaseStream.Seek(0x10-0x06, SeekOrigin.Current);

                    binWriter.Write((byte)0x01);
                    binWriter.Write((byte)0x01);
                    binWriter.Write((byte)0x01);
                    binWriter.Write((byte)0x01);

                    binWriter.BaseStream.Seek(0x10, SeekOrigin.Current);

                    binWriter.Write((ushort)spiritbind);

                    binWriter.Write((byte)materia1);
                    binWriter.Write((byte)materia2);
                    binWriter.Write((byte)materia3);
                    binWriter.Write((byte)materia4);
                    binWriter.Write((byte)materia5);
                }                
            }

            return data;        
        }

    }
}

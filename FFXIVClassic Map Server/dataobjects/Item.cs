using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class Item
    {
        public uint uniqueId;
        public uint itemId;
        public int quantity = 1;
        public ushort slot;

        public bool isUntradeable = false;
        public byte quality = 1;

        public uint durability = 0;
        public ushort spiritbind = 0;

        public byte materia1 = 0;
        public byte materia2 = 0;
        public byte materia3 = 0;
        public byte materia4 = 0;
        public byte materia5 = 0;

        //Bare Minimum
        public Item(uint id, uint itemId, ushort slot)
        {
            this.uniqueId = id;
            this.itemId = itemId;
            this.quantity = quantity;
            this.slot = slot;            
        }

        public Item(uint uniqueId, uint itemId, int quantity, ushort slot, bool isUntradeable, byte qualityNumber, uint durability, ushort spiritbind, byte materia1, byte materia2, byte materia3, byte materia4, byte materia5)
        {
            this.uniqueId = uniqueId;
            this.itemId = itemId;
            this.quantity = quantity;
            this.slot = slot;
            this.isUntradeable = isUntradeable;
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
                    binWriter.Write((UInt32)uniqueId);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((Int32)quantity);
                    binWriter.Write((UInt32)itemId);
                    binWriter.Write((UInt16)slot);

                    binWriter.Write((UInt16)0x0000);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);

                    binWriter.Write(isUntradeable ? (UInt32)0x3 : (UInt32)0x0);

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

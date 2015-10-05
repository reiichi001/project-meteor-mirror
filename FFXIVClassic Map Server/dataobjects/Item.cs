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
        public uint slot;

        public bool isUntradeable = false;
        public bool isHighQuality = false;

        public uint durability = 0;
        public ushort spiritbind = 0;

        public byte materia1 = 0;
        public byte materia2 = 0;
        public byte materia3 = 0;
        public byte materia4 = 0;
        public byte materia5 = 0;

        //Bare Minimum
        public Item(uint id, uint itemId, uint slot)
        {
            this.uniqueId = id;
            this.itemId = itemId;
            this.quantity = quantity;
            this.slot = slot;            
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
                    binWriter.Write((UInt32)slot);

                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);
                    binWriter.Write((UInt32)0x00000000);

                    binWriter.Write(isUntradeable ? (UInt32)0x3 : (UInt32)0x0);

                    binWriter.Write((UInt32)0x00000000);

                    binWriter.Write(isHighQuality ? (byte)0x02 : (byte)0x01);
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

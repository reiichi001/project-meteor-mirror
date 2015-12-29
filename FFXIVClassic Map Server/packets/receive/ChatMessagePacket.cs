using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class ChatMessagePacket
    {
        public float posX;
        public float posY;
        public float posZ;
        public float posRot;

        public uint logType;

        public string message;

        public bool invalidPacket = false;

        public ChatMessagePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        binReader.ReadUInt64();
                        posX = binReader.ReadSingle();
                        posY = binReader.ReadSingle();
                        posZ = binReader.ReadSingle();
                        posRot = binReader.ReadSingle();
                        logType = binReader.ReadUInt32();
                        message = Encoding.ASCII.GetString(binReader.ReadBytes(0x200)).Trim(new [] { '\0' });
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

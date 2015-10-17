using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.script
{
    class CommandStartRequestPacket
    {
        bool invalidPacket = false;

        public uint actorID;
        public uint scriptOwnerActorID;
        public uint val1;
        public uint val2;
        public string callbackName;

        public CommandStartRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        scriptOwnerActorID = binReader.ReadUInt32();
                        val1 = binReader.ReadUInt32();
                        val2 = binReader.ReadUInt32();
                        binReader.ReadByte();

                        while (true)
                        {
                            byte inByte = binReader.ReadByte();
                            if (inByte == 0)
                                break;
                            callbackName += (char)inByte;
                        }

                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}

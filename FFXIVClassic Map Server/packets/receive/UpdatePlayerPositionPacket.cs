using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class UpdatePlayerPositionPacket
    {
        bool invalidPacket = false;

        public ulong time;
        public float x, y, z, rot;
        public ushort moveState; //0: Standing, 1: Walking, 2: Running

        public UpdatePlayerPositionPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        time = binReader.ReadUInt64();
                        x = binReader.ReadSingle();
                        y = binReader.ReadSingle();
                        z = binReader.ReadSingle();
                        rot = binReader.ReadSingle();
                        moveState = binReader.ReadUInt16();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }

    }
}

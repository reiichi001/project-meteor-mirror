using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive
{
    class WorldRequestZoneChangePacket
    {
        public uint sessionId;
        public uint destinationZoneId;
        public byte destinationSpawnType;
        public float destinationX;
        public float destinationY;
        public float destinationZ;
        public float destinationRot;

        public bool invalidPacket = false;

        public WorldRequestZoneChangePacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        sessionId = binReader.ReadUInt32();
                        destinationZoneId = binReader.ReadUInt32();
                        destinationSpawnType = (byte)binReader.ReadUInt16();
                        destinationX = binReader.ReadSingle();
                        destinationY = binReader.ReadSingle();
                        destinationZ = binReader.ReadSingle();
                        destinationRot = binReader.ReadSingle();
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }

        }
    }
}

using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Receive
{
    class SessionEndPacket
    {
        public uint destinationZoneId;
        public ushort destinationSpawnType;
        public float destinationX;
        public float destinationY;
        public float destinationZ;
        public float destinationRot;

        public bool invalidPacket = false;

        public SessionEndPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        destinationZoneId = binReader.ReadUInt32();
                        destinationSpawnType = binReader.ReadUInt16();
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

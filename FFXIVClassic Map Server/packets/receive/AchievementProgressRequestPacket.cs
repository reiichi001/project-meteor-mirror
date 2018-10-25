using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class AchievementProgressRequestPacket
    {
        public bool invalidPacket = false;

        public uint achievementID;
        public uint responseType;

        public AchievementProgressRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        achievementID = binReader.ReadUInt32();
                        responseType = binReader.ReadUInt32();

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

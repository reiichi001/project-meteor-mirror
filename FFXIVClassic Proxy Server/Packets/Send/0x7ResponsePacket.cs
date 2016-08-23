using FFXIVClassic.Common;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.Send.Login
{
    class Login0x7ResponsePacket
    {
        public static SubPacket BuildPacket(uint actorID)
        {
            byte[] data = new byte[0x18];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)actorID);
                        binWriter.Write((UInt32)type);
                    }
                    catch (Exception)
                    {                        
                    }
                }
            }

            return BasePacket.CreatePacket(data, false, false);
        }
    }
}

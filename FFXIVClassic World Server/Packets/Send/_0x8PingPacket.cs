using FFXIVClassic.Common;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.Send.Login
{
    class _0x8PingPacket
    {
        public const ushort OPCODE = 0x0008;
        public const uint PACKET_SIZE = 0x18;

        public static SubPacket BuildPacket(uint actorID)
        {
            byte[] data = new byte[PACKET_SIZE];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)actorID);
                        binWriter.Write((UInt32)Utils.UnixTimeStampUTC());
                    }
                    catch (Exception)
                    {}
                }
            }

            return new SubPacket(false, OPCODE, 0, 0, data);
        }
    }
}

using System;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class EndEventPacket
    {
        public const ushort OPCODE = 0x0131;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket BuildPacket(uint sourcePlayerActorId, uint eventOwnerActorID, string eventStarter)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourcePlayerActorId);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((Byte)1);
                    binWriter.Write(Encoding.ASCII.GetBytes(eventStarter), 0, Encoding.ASCII.GetByteCount(eventStarter) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(eventStarter));
                }
            }

            return new SubPacket(OPCODE, sourcePlayerActorId, data);
        }
    }
}

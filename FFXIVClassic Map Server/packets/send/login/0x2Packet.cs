using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.login
{
    class _0x2Packet
    {
        public const ushort OPCODE = 0x0002;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(uint sourceActorId)
        {
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.BaseStream.Seek(0x8, SeekOrigin.Begin);
                    binWriter.Write((uint)sourceActorId);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

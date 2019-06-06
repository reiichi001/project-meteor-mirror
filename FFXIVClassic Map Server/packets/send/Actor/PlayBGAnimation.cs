using FFXIVClassic.Common;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class PlayBGAnimation
    {
        public const ushort OPCODE = 0x00D9;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, string animName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
          
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(animName), 0, Encoding.ASCII.GetByteCount(animName) > 0x8 ? 0x8 : Encoding.ASCII.GetByteCount(animName));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);            
        }

    }
}

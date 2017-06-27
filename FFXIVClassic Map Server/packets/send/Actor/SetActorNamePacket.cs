using System;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorNamePacket
    {
        public const ushort OPCODE = 0x013D;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint sourceActorId, uint displayNameID, string customName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)displayNameID);

                    if (customName != null && (displayNameID == 0 || displayNameID == 0xFFFFFFFF))
                    {
                        binWriter.Write(Encoding.ASCII.GetBytes(customName), 0, Encoding.ASCII.GetByteCount(customName) >= 0x20 ? 0x19 : Encoding.ASCII.GetByteCount(customName));
                    }

                }
            }

            SubPacket packet = new SubPacket(OPCODE, sourceActorId, data);
            return packet;
        }

    }
}

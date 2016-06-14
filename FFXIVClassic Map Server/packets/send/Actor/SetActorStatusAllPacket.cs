using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorStatusAllPacket
    {
        public const ushort OPCODE = 0x0179;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, ushort[] statusIds)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
          
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    for (int i = 0; i < statusIds.Length; i++)
                    {
                        if (i >= 20)
                            break;
                        binWriter.Write((UInt16)statusIds[i]);
                    }
                }
            }

            SubPacket packet = new SubPacket(OPCODE, playerActorID, tarGetActorID, data);
            return packet;
        }
    }
}

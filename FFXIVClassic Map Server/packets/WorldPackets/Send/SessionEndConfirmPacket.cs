using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send
{
    class SessionEndConfirmPacket
    {
        public const ushort OPCODE = 0x1001;
        public const uint PACKET_SIZE = 0x30;

        public static SubPacket BuildPacket(Session session, uint destinationZone, ushort errorCode = 0)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)session.id);
                    binWriter.Write((UInt16)errorCode);
                    binWriter.Write((UInt32)destinationZone);
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }
    }
}

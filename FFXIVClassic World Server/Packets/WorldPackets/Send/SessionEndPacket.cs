using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send
{
    class SessionEndPacket
    {
        public const ushort OPCODE = 0x1001;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(Session session)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)0);
                    }
                    catch (Exception)
                    { }
                }
            }

            return new SubPacket(true, OPCODE, session.sessionId, data);
        }

        public static SubPacket BuildPacket(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)destinationZoneId);
                        binWriter.Write((UInt16)spawnType);
                        binWriter.Write((Single)spawnX);
                        binWriter.Write((Single)spawnY);
                        binWriter.Write((Single)spawnZ);
                        binWriter.Write((Single)spawnRotation);

                    }
                    catch (Exception)
                    { }
                }
            }

            return new SubPacket(true, OPCODE, session.sessionId, data);
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.events
{
    class SetPushEventConditionWithCircle
    {
        public const ushort OPCODE = 0x016F;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            string eventName = "";

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)0.0f);
                    binWriter.Write((UInt32)0);
                    binWriter.Write((Single)0.0f);
                    binWriter.Seek(4, SeekOrigin.Current);
                    binWriter.Write((Byte)0);
                    binWriter.Write((Byte)0);
                    binWriter.Write((Byte)0);
                    binWriter.Write(Encoding.ASCII.GetBytes(eventName), 0, Encoding.ASCII.GetByteCount(eventName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(eventName));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

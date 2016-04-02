using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.events
{
    class SetEventStatus
    {
        public const ushort OPCODE = 0x0136;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket buildPacket(uint playerActorID, uint sourceActorID, bool enabled, byte unknown2, string conditionName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)(enabled ? 1 : 0));
                    binWriter.Write((Byte)unknown2);
                    binWriter.Write(Encoding.ASCII.GetBytes(conditionName), 0, Encoding.ASCII.GetByteCount(conditionName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(conditionName));
                }
            }

            return new SubPacket(OPCODE, sourceActorID, playerActorID, data);
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.events
{
    class SetTalkEventCondition
    {
        public const ushort OPCODE = 0x012E;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket buildPacket(uint playerActorID, uint sourceActorID, EventList.TalkEventCondition condition)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    condition.unknown1 = 4;
                    binWriter.Write((Byte)condition.unknown1);
                    binWriter.Write((Byte)(condition.isDisabled ? 0x1 : 0x0));
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.conditionName), 0, Encoding.ASCII.GetByteCount(condition.conditionName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(condition.conditionName));
                }
            }

            return new SubPacket(OPCODE, sourceActorID, playerActorID, data);
        }
    }
}

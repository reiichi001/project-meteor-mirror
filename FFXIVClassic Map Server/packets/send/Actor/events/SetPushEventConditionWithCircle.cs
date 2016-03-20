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
    class SetPushEventConditionWithCircle
    {
        public const ushort OPCODE = 0x016F;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket buildPacket(uint playerActorID, uint sourceActorID, EventList.PushCircleEventCondition condition)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Single)condition.radius);
                    binWriter.Write((UInt32)0x44533088);
                    binWriter.Write((Single)100.0f);
                    binWriter.Seek(4, SeekOrigin.Current);
                    binWriter.Write((Byte)(condition.outwards ? 0x11 : 0x1)); //If == 0x10, Inverted Bounding Box
                    binWriter.Write((Byte)0);
                    binWriter.Write((Byte)(condition.silent ? 0x1 : 0x0)); //Silent Trigger
                    binWriter.Write(Encoding.ASCII.GetBytes(condition.conditionName), 0, Encoding.ASCII.GetByteCount(condition.conditionName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(condition.conditionName));
                }
            }

            return new SubPacket(OPCODE, sourceActorID, playerActorID, data);
        }
    }
}

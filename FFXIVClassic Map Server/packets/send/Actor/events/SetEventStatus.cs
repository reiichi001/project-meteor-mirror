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

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            string eventName = "";

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)0);                    
                    binWriter.Write(Encoding.ASCII.GetBytes(eventName), 0, Encoding.ASCII.GetByteCount(eventName) >= 0x24 ? 0x24 : Encoding.ASCII.GetByteCount(eventName));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

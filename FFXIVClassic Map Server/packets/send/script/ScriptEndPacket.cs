using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.script
{
    class ScriptEndPacket
    {
        public const ushort OPCODE = 0x0131;
        public const uint PACKET_SIZE = 0x50;

        public static SubPacket buildPacket(uint playerActorID, string startName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((uint)playerActorID);
                    binWriter.Write((uint)0);
                    binWriter.Write((byte)0);
                    binWriter.Write(Encoding.Unicode.GetBytes(startName));
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class SendBlacklistPacket
    {
        public const ushort OPCODE = 0x01CB;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket buildPacket(uint playerActorID, string[] blacklistedNames, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)0);
                    int max;

                    if (blacklistedNames.Length - offset <= 0x32)
                        max = blacklistedNames.Length - offset;
                    else
                        max = 0x32;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++ )
                        binWriter.Write(Encoding.ASCII.GetBytes(blacklistedNames[i]), 0, Encoding.ASCII.GetByteCount(blacklistedNames[i]) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(blacklistedNames[i]));

                    offset += max;
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

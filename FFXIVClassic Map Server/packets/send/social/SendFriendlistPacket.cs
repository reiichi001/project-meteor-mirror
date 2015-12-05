using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class SendFriendlistPacket
    {
        public const ushort OPCODE = 0x01CE;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket buildPacket(uint playerActorID, Tuple<long, string>[] friends, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {                    
                    binWriter.Write((UInt32)0);
                    int max;

                    if (friends.Length - offset <= 0x32)
                        max = friends.Length - offset;
                    else
                        max = 0x32;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write(Encoding.ASCII.GetBytes(friends[i].Item2), 0, Encoding.ASCII.GetByteCount(friends[i].Item2) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(friends[i].Item2));
                        binWriter.Write((UInt64)friends[i].Item1);
                    }

                    offset += max;
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

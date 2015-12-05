using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.social
{
    class FriendStatusPacket
    {
        public const ushort OPCODE = 0x01CF;
        public const uint PACKET_SIZE = 0x686;

        public static SubPacket buildPacket(uint playerActorID, Tuple<long, bool>[] friendStatus)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)0);
                    int max;

                    if (friendStatus != null)
                    {
                        if (friendStatus.Length <= 200)
                            max = friendStatus.Length;
                        else
                            max = 200;
                    }
                    else
                        max = 0;

                    binWriter.Write((UInt32)max);

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt64)friendStatus[i].Item1);
                        binWriter.Write((UInt64)(friendStatus[i].Item2 ? 1 : 0));
                    }

                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

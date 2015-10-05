using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.Actor
{
    class SetActorNamePacket
    {
        public const ushort OPCODE = 0x013D;
        public const uint PACKET_SIZE = 0x48;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, uint displayNameID, string customName)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)displayNameID);

                    if (displayNameID == 0xFFFFFFFF)
                    {
                        if (customName.Length <= 0x20)
                            binWriter.Write(Encoding.ASCII.GetBytes(customName));
                        else
                            binWriter.Write(Encoding.ASCII.GetBytes("ERROR: NAME TO BIG"));
                    }

                }
                data = mem.GetBuffer();
            }

            SubPacket packet = new SubPacket(OPCODE, playerActorID, targetActorID, data);
            return packet;
        }

    }
}

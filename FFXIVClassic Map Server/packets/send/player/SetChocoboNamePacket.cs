using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetChocoboNamePacket
    {
        public const ushort OPCODE = 0x0198;
        public const uint PACKET_SIZE = 0x40;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID, string name)
        {
            if (Encoding.Unicode.GetByteCount(name) >= 0x20)
                name = "ERR: Too Long";
            return new SubPacket(OPCODE, playerActorID, targetActorID, Encoding.ASCII.GetBytes(name));
        }
    }
}

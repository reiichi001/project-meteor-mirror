using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorStatePacket
    {
        public const int MAIN_STATE_PASSIVE = 0;
        public const int MAIN_STATE_DEAD = 1;
        public const int MAIN_STATE_ACTIVE = 2;
        public const int MAIN_STATE_DEAD2 = 3;

        public const int MAIN_STATE_SITTING_OBJECT = 11;
        public const int MAIN_STATE_SITTING_FLOOR = 13;

        public const int MAIN_STATE_MOUNTED = 15;

        public const int MAIN_STATE_UNKNOWN1 = 0x0E;
        public const int MAIN_STATE_UNKNOWN2 = 0x1E;
        public const int MAIN_STATE_UNKNOWN3 = 0x1F;
        public const int MAIN_STATE_UNKNOWN4 = 0x20;

        //What is this for?
        public const int SUB_STATE_NONE = 0x00;
        public const int SUB_STATE_PLAYER = 0xBF;
        public const int SUB_STATE_MONSTER = 0x03;

        public const ushort OPCODE = 0x134;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, uint targetID, uint mainState, uint subState)
        {            
            ulong combined = (mainState & 0xFF) | ((subState & 0xFF) << 8);
            return new SubPacket(OPCODE, playerActorID, targetID, BitConverter.GetBytes(combined));
        }
    }
}

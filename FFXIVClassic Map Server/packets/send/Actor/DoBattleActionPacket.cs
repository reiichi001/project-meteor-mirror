using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class DoBattleActionPacket
    {
        public const ushort OPCODE = 0x0139;
        public const uint PACKET_SIZE = 0x58;

        public static SubPacket buildPacket(uint playerActorID, uint targetActorID)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                   /*
                    uint32	actionSourceId = 0;
		            uint32	animationId = 0;
		            uint32	unknown0 = 0;
		            uint32	unknown1 = 0;
		            uint32	unknown2 = 0;
		            uint32	unknown3 = 0;
		            uint32	unknown4 = 0;
		            float	unknown5 = 1.0f;
		            uint32	unknown6 = 1;
		            uint32	descriptionId = 0;
		            uint32	actionTargetId = 0;
		            uint16	damage = 0;
		            uint16	damageType = 0;
		            uint32	feedbackId = 0;
		            uint32	attackSide = 0;
                    */
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}

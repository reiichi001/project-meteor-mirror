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

        public static SubPacket buildPacket(uint playerActorID, uint sourceActorId, uint targetActorId, uint animationId, uint effectId, ushort worldMasterTextId, ushort commandId, ushort amount, byte dirOrBody)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt16)1); //? Crashes if changed
                    binWriter.Write((UInt16)0); //?
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)810); //?

                    binWriter.Write((UInt32)targetActorId);

                    binWriter.Write((UInt16)amount);
                    binWriter.Write((UInt16)worldMasterTextId);

                    binWriter.Write((UInt32)effectId);

                    binWriter.Write((Byte)dirOrBody);
                    binWriter.Write((Byte)1); //?
                }
            }

            return new SubPacket(OPCODE, sourceActorId, playerActorID, data);
        }
    }
}

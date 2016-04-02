using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class KickEventPacket
    {
        public const ushort OPCODE = 0x012F;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket buildPacket(uint playerActorId, uint targetActorId, string conditionName, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)playerActorId);
                    binWriter.Write((UInt32)targetActorId);
                    binWriter.Write((Byte)0x5);
                    binWriter.Write((Byte)0x87);
                    binWriter.Write((Byte)0xDC);
                    binWriter.Write((Byte)0x75);
                    binWriter.Write((UInt32)0x30400000);
                    binWriter.Write(Encoding.ASCII.GetBytes(conditionName), 0, Encoding.ASCII.GetByteCount(conditionName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(conditionName));

                    binWriter.Seek(0x30, SeekOrigin.Begin);

                    LuaUtils.writeLuaParams(binWriter, luaParams);
                }
            }

            return new SubPacket(OPCODE, playerActorId, playerActorId, data);
        }
    }

}

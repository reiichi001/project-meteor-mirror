﻿using FFXIVClassic_Map_Server.lua;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class InfoRequestResponsePacket
    {
        public const ushort OPCODE = 0x0133;
        public const uint PACKET_SIZE = 0xE0;

        public static SubPacket BuildPacket(uint playerActorID, uint tarGetActorID, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    LuaUtils.WriteLuaParams(binWriter, luaParams);
                }
            }

            return new SubPacket(OPCODE, playerActorID, tarGetActorID, data);
        }
    }
}

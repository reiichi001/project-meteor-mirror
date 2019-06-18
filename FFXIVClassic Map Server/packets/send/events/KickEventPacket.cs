/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class KickEventPacket
    {
        public const ushort OPCODE = 0x012F;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket BuildPacket(uint sourcePlayerActorId, uint targetEventActorId, uint unknown, string conditionName, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)sourcePlayerActorId);
                    binWriter.Write((UInt32)targetEventActorId);
                    binWriter.Write((UInt32)unknown);
                    binWriter.Write((UInt32)0x30400000);
                    binWriter.Write(Encoding.ASCII.GetBytes(conditionName), 0, Encoding.ASCII.GetByteCount(conditionName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(conditionName));

                    binWriter.Seek(0x30, SeekOrigin.Begin);

                    LuaUtils.WriteLuaParams(binWriter, luaParams);
                }
            }

            return new SubPacket(OPCODE, sourcePlayerActorId, data);
        }
    }

}

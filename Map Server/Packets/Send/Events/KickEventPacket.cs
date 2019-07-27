/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.Map.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Meteor.Common;

namespace Meteor.Map.packets.send.events
{
    class KickEventPacket
    {
        public const ushort OPCODE = 0x012F;
        public const uint PACKET_SIZE = 0x90;

        public static SubPacket BuildPacket(uint triggerActorId, uint ownerActorId, string eventName, byte eventType, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)triggerActorId);
                    binWriter.Write((UInt32)ownerActorId);
                    binWriter.Write((Byte)eventType);
                    binWriter.Write((Byte)0x17); //?
                    binWriter.Write((UInt16)0x75DC); //?
                    binWriter.Write((UInt32)0x30400000); //ServerCodes
                    Utils.WriteNullTermString(binWriter, eventName);

                    binWriter.Seek(0x30, SeekOrigin.Begin);

                    LuaUtils.WriteLuaParams(binWriter, luaParams);
                }
            }

            return new SubPacket(OPCODE, triggerActorId, data);
        }
    }

}

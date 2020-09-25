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

namespace  Meteor.Map.packets.send.actor
{
    class ActorInstantiatePacket
    {
        public const ushort OPCODE = 0x00CC;
        public const uint PACKET_SIZE = 0x128;

        public static SubPacket BuildPacket(uint sourceActorId, string objectName, string className, List<LuaParam> initParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int value1 = 0x00; //Instance ID?
                    int value2 = 0x3040;
                    binWriter.Write((Int16)value1);
                    binWriter.Write((Int16)value2);
                    binWriter.Write(Encoding.ASCII.GetBytes(objectName), 0, Encoding.ASCII.GetByteCount(objectName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(objectName));
                    binWriter.BaseStream.Seek(0x24, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(className), 0, Encoding.ASCII.GetByteCount(className) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(className));
                    binWriter.BaseStream.Seek(0x44, SeekOrigin.Begin);
                    LuaUtils.WriteLuaParams(binWriter, initParams);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

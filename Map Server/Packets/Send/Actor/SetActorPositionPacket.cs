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

using System;
using System.IO;

using Meteor.Common;

namespace  Meteor.Map.packets.send.actor
{
    class SetActorPositionPacket
    {
        public const ushort OPCODE = 0x00CE;
        public const uint PACKET_SIZE = 0x48;

        public const ushort SPAWNTYPE_FADEIN = 0;
        public const ushort SPAWNTYPE_PLAYERWAKE = 1;
        public const ushort SPAWNTYPE_WARP_DUTY = 2;
        public const ushort SPAWNTYPE_WARP2 = 3;
        public const ushort SPAWNTYPE_WARP3 = 4;
        public const ushort SPAWNTYPE_WARP_YELLOW = 5;
        public const ushort SPAWNTYPE_WARP_DUTY2 = 6;
        public const ushort SPAWNTYPE_WARP_LIGHT = 7;
        
        public static SubPacket BuildPacket(uint sourceActorId, uint actorId, float x, float y, float z, float rotation, ushort spawnType, bool isZoningPlayer)
        {
            byte[] data = new byte[PACKET_SIZE-0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((Int32)0);                    
                    binWriter.Write((Int32)actorId);
                    binWriter.Write((Single)x);
                    binWriter.Write((Single)y);
                    binWriter.Write((Single)z);
                    binWriter.Write((Single)rotation);

                    binWriter.BaseStream.Seek(0x24, SeekOrigin.Begin);

                    binWriter.Write((UInt16)spawnType);
                    binWriter.Write((UInt16)(isZoningPlayer ? 1 : 0));
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

    }
}

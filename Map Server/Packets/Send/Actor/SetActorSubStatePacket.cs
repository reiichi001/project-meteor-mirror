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

using System.IO;

using Meteor.Common;
using Meteor.Map.actors.chara;

namespace Meteor.Map.packets.send.actor
{
    class SetActorSubStatePacket
    {
        public const ushort OPCODE = 0x144;
        public const uint PACKET_SIZE = 0x28;

        enum SubStat : int
        {
            Breakage          = 0x00, // (index goes high to low, bitflags)
            Chant             = 0x01, // [Nibbles: left / right hand = value]) (AKA SubStatObject)
            Guard             = 0x02, // [left / right hand = true] 0,1,2,3) ||| High byte also defines how many bools to use as flags for byte 0x4. 
            Waste             = 0x03, // (High Nibble)
            Mode              = 0x04, // ???
            Unknown           = 0x05, // ???
            SubStatMotionPack = 0x06,
            Unknown2          = 0x07,
        }
        public static SubPacket BuildPacket(uint sourceActorId, SubState substate)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                   binWriter.Write((byte)substate.breakage);
                   binWriter.Write((byte)substate.chantId);
                   binWriter.Write((byte)(substate.guard & 0xF));
                   binWriter.Write((byte)(substate.waste));
                   binWriter.Write((byte)(substate.mode));
                   binWriter.Write((byte)0);
                   binWriter.Write((ushort)substate.motionPack);
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}

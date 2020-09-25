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

using Meteor.Common;
using System;

namespace Meteor.Map.packets.send.actor
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

        public static SubPacket BuildPacket(uint sourceActorId, uint mainState, uint subState)
        {            
            ulong combined = (mainState & 0xFF) | ((subState & 0xFF) << 8);
            return new SubPacket(OPCODE, sourceActorId, BitConverter.GetBytes(combined));
        }
    }
}

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

using System;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetMusicPacket
    {
        public const ushort OPCODE = 0x000C;
        public const uint PACKET_SIZE = 0x28;

        public const ushort EFFECT_IMMEDIATE           = 0x1;
        public const ushort EFFECT_CROSSFADE           = 0x2; //??
        public const ushort EFFECT_LAYER               = 0x3; //??
        public const ushort EFFECT_FADEIN              = 0x4;
        public const ushort EFFECT_PLAY_NORMAL_CHANNEL = 0x5; //Only works for multi channeled music
        public const ushort EFFECT_PLAY_BATTLE_CHANNEL = 0x6;

        public static SubPacket BuildPacket(uint sourceActorId, ushort musicID, ushort musicTrackMode)
        {
            ulong combined = (ulong)(musicID | (musicTrackMode << 16));
            return new SubPacket(OPCODE, 0, BitConverter.GetBytes(combined));
        }
    }
}

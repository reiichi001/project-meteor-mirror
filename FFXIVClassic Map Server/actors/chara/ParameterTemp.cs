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

namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class ParameterTemp
    {
        public short tp = 0;

        public int targetInformation = 0;

        public ushort[] maxCommandRecastTime = new ushort[40];

        public float[] forceControl_float_forClientSelf = { 1.0f, 1.0f, 0.0f, 0.0f};
        public short[] forceControl_int16_forClientSelf = { -1, -1 };

        public byte[] otherClassAbilityCount = new byte[2];
        public byte[] giftCount = new byte[2];
    }
}

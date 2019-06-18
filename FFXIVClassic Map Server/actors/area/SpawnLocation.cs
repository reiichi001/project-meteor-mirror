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

namespace FFXIVClassic_Map_Server.actors.area
{
    class SpawnLocation
    {
        public uint classId;
        public string uniqueId;
        public uint zoneId;
        public string privAreaName;
        public uint privAreaLevel;
        public float x;
        public float y;
        public float z;
        public float rot;
        public ushort state;
        public uint animId;

        public SpawnLocation(uint classId, string uniqueId, uint zoneId, string privAreaName, uint privAreaLevel, float x, float y, float z, float rot, ushort state, uint animId)
        {
            this.classId = classId;
            this.uniqueId = uniqueId;
            this.zoneId = zoneId;
            this.privAreaName = privAreaName;
            this.privAreaLevel = privAreaLevel;
            this.x = x;
            this.y = y;
            this.z = z;
            this.rot = rot;
            this.state = state;
            this.animId = animId;
        }
    }
}

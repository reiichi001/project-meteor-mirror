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

using FFXIVClassic_Map_Server.actors.director;

namespace FFXIVClassic_Map_Server.actors.group
{
    class GLContentGroup : ContentGroup
    {
        public GLContentGroup(ulong groupIndex, Director director, uint[] initialMembers)
            : base(groupIndex, director, initialMembers)
        {
        }

        public override uint GetTypeId()
        {
            return Group.ContentGroup_GuildleveGroup;
        }
    }
}

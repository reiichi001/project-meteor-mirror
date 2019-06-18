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

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class LinkshellMember : IComparable<LinkshellMember>
    {
        public readonly uint charaId;
        public readonly ulong lsId;
        public byte rank;
        
        public LinkshellMember(uint charaId, ulong lsId, byte rank)
        {
            this.charaId = charaId;
            this.lsId = lsId;
            this.rank = rank;
        }

        public int CompareTo(LinkshellMember other)
        {
            return Server.GetServer().GetNameForId(charaId).CompareTo(Server.GetServer().GetNameForId(other.charaId));
        }
    }
}

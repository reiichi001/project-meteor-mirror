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

namespace Meteor.Map.packets.send.group
{
    class GroupMember
    {
        public uint actorId;
        public int localizedName;
        public uint unknown2;
        public bool flag1;
        public bool isOnline;
        public string name;

        public GroupMember(uint actorId, int localizedName, uint unknown2, bool flag1, bool isOnline, string name)
        {
            this.actorId = actorId;
            this.localizedName = localizedName;
            this.unknown2 = unknown2;
            this.flag1 = flag1;
            this.isOnline = isOnline;
            this.name = name == null ? "" : name;
        }
    }
}

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

namespace Meteor.World.DataObjects.Group
{
    class RetainerGroupMember
    {
        public uint id;
        public string name;
        public uint classActorId;
        public byte cdIDOffset;
        public ushort placeName;
        public byte conditions;
        public byte level;

        public RetainerGroupMember(uint id, string name, uint classActorId, byte cdIDOffset, ushort placeName, byte conditions, byte level)
        {
            this.id = id;
            this.name = name;
            this.classActorId = classActorId;
            this.cdIDOffset = cdIDOffset;
            this.placeName = placeName;
            this.conditions = conditions;
            this.level = level;
        }
    }
}

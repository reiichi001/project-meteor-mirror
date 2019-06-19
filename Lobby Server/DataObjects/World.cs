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

namespace Meteor.Lobby.DataObjects
{
    class World
    {
        public readonly ushort id;
        public readonly string address;
        public readonly ushort port;
        public readonly ushort listPosition;
        public readonly ushort population;
        public readonly string name;
        public readonly bool isActive;

        public World(
            ushort id,
            string address,
            ushort port,
            ushort listPosition,
            ushort population,
            string name,
            bool isActive)
        {
            this.id = id;
            this.address = address;
            this.port = port;
            this.listPosition = listPosition;
            this.population = population;
            this.name = name;
            this.isActive = isActive;
        }
    }
}

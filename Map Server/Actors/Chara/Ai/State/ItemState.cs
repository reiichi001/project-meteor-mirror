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

using Meteor.Map.Actors;
using Meteor.Map.dataobjects;

namespace Meteor.Map.actors.chara.ai.state
{
    class ItemState : State
    {
        ItemData item;
        new Player owner;
        public ItemState(Player owner, Character target, ushort slot, uint itemId) :
            base(owner, target)
        {
            this.owner = owner;
            this.target = target;
        }
    }
}

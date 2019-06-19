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

using Meteor.Map.actors.chara.player;
using Meteor.Map.Actors;
using System;

namespace Meteor.Map.actors.chara.npc
{
    class Retainer : Npc
    {
        public const int MAXSIZE_INVENTORY_NORMAL = 150;
        public const int MAXSIZE_INVENTORY_CURRANCY = 320;
        public const int MAXSIZE_INVENTORY_BAZAAR = 10;

        private uint retainerId;
        private Player ownerPlayer;

        public Retainer(uint retainerId, ActorClass actorClass, Player player, float posX, float posY, float posZ, float rot)
            : base(0, actorClass, "myretainer", player.GetZone(), posX, posY, posZ, rot, 0, 0, null)
        {
            this.retainerId = retainerId;
            this.ownerPlayer = player;
            this.actorName = String.Format("_rtnre{0:x7}", actorId);

            itemPackages[ItemPackage.NORMAL] = new ItemPackage(this, MAXSIZE_INVENTORY_NORMAL, ItemPackage.NORMAL);
            itemPackages[ItemPackage.CURRENCY_CRYSTALS] = new ItemPackage(this, MAXSIZE_INVENTORY_CURRANCY, ItemPackage.CURRENCY_CRYSTALS);
            itemPackages[ItemPackage.BAZAAR] = new ItemPackage(this, MAXSIZE_INVENTORY_BAZAAR, ItemPackage.BAZAAR);

            itemPackages[ItemPackage.NORMAL].InitList(Database.GetItemPackage(this, 0, ItemPackage.NORMAL));
            itemPackages[ItemPackage.CURRENCY_CRYSTALS].InitList(Database.GetItemPackage(this, 0, ItemPackage.CURRENCY_CRYSTALS));
            itemPackages[ItemPackage.BAZAAR].InitList(Database.GetItemPackage(this, 0, ItemPackage.BAZAAR));
        }

        public uint GetRetainerId()
        {
            return retainerId;
        }
    }
}

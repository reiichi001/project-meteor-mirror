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

using System.Collections.Generic;
using Meteor.Map.Actors;

namespace Meteor.Map.actors.chara.ai
{
    // todo: actually implement enmity properly
    class HateEntry
    {
        public Character actor;
        public uint cumulativeEnmity;
        public uint volatileEnmity;
        public bool isActive;

        public HateEntry(Character actor, uint cumulativeEnmity = 0, uint volatileEnmity = 0, bool isActive = false)
        {
            this.actor = actor;
            this.cumulativeEnmity = cumulativeEnmity;
            this.volatileEnmity = volatileEnmity;
            this.isActive = isActive;
        }
    }

    class HateContainer
    {
        private Dictionary<Character, HateEntry> hateList;
        private Character owner;

        public HateContainer(Character owner)
        {
            this.owner = owner;
            this.hateList = new Dictionary<Character, HateEntry>();
        }

        public void AddBaseHate(Character target)
        {
            if (!HasHateForTarget(target))
                hateList.Add(target, new HateEntry(target, 1, 0, true));
        }

        public void UpdateHate(Character target, int damage)
        {
            AddBaseHate(target);
            //hateList[target].volatileEnmity += (uint)damage;
            hateList[target].cumulativeEnmity += (uint)damage;
        }

        public void ClearHate(Character target = null)
        {
            if (target != null)
                hateList.Remove(target);
            else
                hateList.Clear();
        }

        private void UpdateHate(HateEntry entry)
        {

        }

        public Dictionary<Character, HateEntry> GetHateList()
        {
            // todo: return unmodifiable collection?
            return hateList;
        }

        public bool HasHateForTarget(Character target)
        {
            return hateList.ContainsKey(target);
        }

        public Character GetMostHatedTarget()
        {
            uint enmity = 0;
            Character target = null;

            foreach(var entry in hateList.Values)
            {
                if (entry.cumulativeEnmity > enmity && entry.isActive)
                {
                    enmity = entry.cumulativeEnmity;
                    target = entry.actor;
                }
            }
            return target;
        }
    }
}

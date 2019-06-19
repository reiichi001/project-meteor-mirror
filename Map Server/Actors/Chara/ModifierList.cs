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

using System;
using System.Collections.Generic;

namespace Meteor.Map.actors.chara
{
    class ModifierListEntry
    {
        public uint id;
        public Int64 value;

        public ModifierListEntry(uint id, Int64 value)
        {
            this.id = id;
            this.value = value;
        }
    }

    class ModifierList
    {
        public Dictionary<uint, ModifierListEntry> modList;
        public Dictionary<uint, ModifierListEntry> mobModList;

        public ModifierList(uint id)
        {
            modList = new Dictionary<uint, ModifierListEntry>();
            mobModList = new Dictionary<uint, ModifierListEntry>();
        }

        public void AddModifier(uint id, Int64 val, bool isMobMod)
        {
            var list = isMobMod ? mobModList : modList;
            list.Add(id, new ModifierListEntry(id, val));
        }

        public void SetModifier(uint id, Int64 val, bool isMobMod)
        {
            var list = isMobMod ? mobModList : modList;
            if (list.ContainsKey(id))
                list[id].value = val;
            else
                list.Add(id, new ModifierListEntry(id, val));
        }

        public Int64 GetModifier(uint id, bool isMobMod)
        {
            ModifierListEntry retVal;
            var list = isMobMod ? mobModList : modList;
            if (!list.TryGetValue(id, out retVal))
                return 0;

            return retVal.value;
        }
    }
}

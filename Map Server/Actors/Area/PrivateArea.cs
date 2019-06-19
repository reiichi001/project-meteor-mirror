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


using Meteor.Common;
using Meteor.Map.Actors;
using Meteor.Map.lua;
using Meteor.Map.packets.send.actor;
using System.Collections.Generic;

namespace Meteor.Map.actors.area
{
    class PrivateArea : Area    
    {
        private Zone parentZone;
        private string privateAreaName;
        private uint privateAreaType;

        public PrivateArea(Zone parent, uint id, string classPath, string privateAreaName, uint privateAreaType, ushort bgmDay, ushort bgmNight, ushort bgmBattle)
            : base(id, parent.zoneName, parent.regionId, classPath, bgmDay, bgmNight, bgmBattle, parent.isIsolated, parent.isInn, parent.canRideChocobo, parent.canStealth, true)
        {
            this.parentZone = parent;
            this.zoneName = parent.zoneName;
            this.privateAreaName = privateAreaName;
            this.privateAreaType = privateAreaType;
        }

        public string GetPrivateAreaName()
        {
            return privateAreaName;
        }

        public uint GetPrivateAreaType()
        {
            return privateAreaType;
        }

        public Zone GetParentZone()
        {
            return parentZone;
        }

        public override SubPacket CreateScriptBindPacket()
        {
            List<LuaParam> lParams;

            string path = className;

            string realClassName = className.Substring(className.LastIndexOf("/") + 1);

            lParams = LuaUtils.CreateLuaParamList(classPath, false, true, zoneName, privateAreaName, privateAreaType, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, false, false, false);
            ActorInstantiatePacket.BuildPacket(actorId, actorName, realClassName, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, actorName, realClassName, lParams);
        }


        public void AddSpawnLocation(SpawnLocation spawn)
        {
            mSpawnLocations.Add(spawn);
        }

        public void SpawnAllActors()
        {
            foreach (SpawnLocation spawn in mSpawnLocations)
                SpawnActor(spawn);
        }
    }
}

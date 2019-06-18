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
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using System;

namespace FFXIVClassic_Map_Server.actors.area
{

    class PrivateAreaContent : PrivateArea
    {
        private Director currentDirector;
        private bool isContentFinished = false;

        public static PrivateAreaContent CreateContentArea(String scriptPath)
        {
            return null;
        }

        public PrivateAreaContent(Zone parent, string classPath, string privateAreaName, uint privateAreaType, Director director, Player contentStarter) //TODO: Make it a list
            : base(parent, parent.actorId, classPath, privateAreaName, privateAreaType, 0, 0, 0)
        {
            currentDirector = director;
            LuaEngine.GetInstance().CallLuaFunction(contentStarter, this, "onCreate", false, currentDirector);
        }
        
        public Director GetContentDirector()
        {
            return currentDirector;
        }

        public void ContentFinished()
        {
            isContentFinished = true;
        }

        public void CheckDestroy()
        {
            lock (mActorList)
            {
                if (isContentFinished)
                {
                    bool noPlayersLeft = true;
                    foreach (Actor a in mActorList.Values)
                    {
                        if (a is Player)
                            noPlayersLeft = false;
                    }
                    if (noPlayersLeft)
                        GetParentZone().DeleteContentArea(this);
                }
            }
        }

    }
}

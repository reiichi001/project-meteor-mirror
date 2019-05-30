using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

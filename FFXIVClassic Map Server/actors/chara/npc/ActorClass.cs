using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class ActorClass
    {
        public readonly uint actorClassId;
        public readonly string classPath;
        public readonly uint displayNameId;
        public readonly string eventConditions;

        public ActorClass(uint id, string classPath, uint nameId, string eventConditions)
        {
            this.actorClassId = id;
            this.classPath = classPath;
            this.displayNameId = nameId;
            this.eventConditions = eventConditions;
        }
    }
}

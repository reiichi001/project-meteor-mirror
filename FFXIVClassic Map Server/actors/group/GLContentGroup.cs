using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.actors.group.Work;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using FFXIVClassic_Map_Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class GLContentGroup : ContentGroup
    {
        public GLContentGroup(ulong groupIndex, Director director, uint[] initialMembers)
            : base(groupIndex, director, initialMembers)
        {
        }

        public override uint GetTypeId()
        {
            return Group.ContentGroup_GuildleveGroup;
        }
    }
}

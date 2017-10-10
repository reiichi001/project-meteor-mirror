using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Actor.Group.Work
{
    class GroupGlobalTemp
    {
        public ulong owner;

        //For content group
        public ulong director;

        //For relation group
        public ulong host;
        public uint variableCommand;
    }
}

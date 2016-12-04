using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
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

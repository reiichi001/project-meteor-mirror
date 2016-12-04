using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class LinkshellWork
    {
        public GroupGlobalSave _globalSave = new GroupGlobalSave();
        public GroupMemberSave[] _memberSave = new GroupMemberSave[128];        
    }
}

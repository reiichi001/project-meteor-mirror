using FFXIVClassic_Map_Server.actors.group.work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class LinkshellGroup : Group
    {
        private LinkshellWork linkshellWork;

        public LinkshellGroup(ulong id) : base(id, Group.CompanyGroup, null)
        {
            linkshellWork = new LinkshellWork();            
        }

        public void setMaster(uint actorId)
        {
            linkshellWork._globalSave.master = (ulong)((0xB36F92 << 8) | actorId);
        }

        public void setCrest(ushort crestId)
        {
            linkshellWork._globalSave.crestIcon[0] = crestId;
        }

        public void setRank(byte rank = 1)
        {
            linkshellWork._globalSave.rank = rank;
        }

        public void setMemberRank(int index, byte rank)        
        {
            if (members.Count >= index)
                return;
            linkshellWork._memberSave[index].rank = rank;
        }

    }
}

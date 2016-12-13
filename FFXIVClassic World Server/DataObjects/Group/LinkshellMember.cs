using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class LinkshellMember
    {
        public readonly uint charaId;
        public readonly ulong lsId;
        public readonly ushort slot;
        public readonly ushort rank;
        
        public LinkshellMember(uint charaId, ulong lsId, ushort slot, ushort rank)
        {
            this.charaId = charaId;
            this.lsId = lsId;
            this.slot = slot;
            this.rank = rank;
        }
    }
}

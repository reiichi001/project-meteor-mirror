using System;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class LinkshellMember : IComparable<LinkshellMember>
    {
        public readonly uint charaId;
        public readonly ulong lsId;
        public byte rank;
        
        public LinkshellMember(uint charaId, ulong lsId, byte rank)
        {
            this.charaId = charaId;
            this.lsId = lsId;
            this.rank = rank;
        }

        public int CompareTo(LinkshellMember other)
        {
            return Server.GetServer().GetNameForId(charaId).CompareTo(Server.GetServer().GetNameForId(other.charaId));
        }
    }
}

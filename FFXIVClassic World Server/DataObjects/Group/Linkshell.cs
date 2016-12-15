using FFXIVClassic_World_Server.Actor.Group.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Linkshell : Group
    {
        public ulong dbId;
        public string name;

        public LinkshellWork linkshellWork = new LinkshellWork();

        public Dictionary<ulong, LinkshellMember> members = new Dictionary<ulong, LinkshellMember>();

        public Linkshell(ulong dbId,  ulong groupIndex, string name, ushort crestId, uint master, byte rank) : base(groupIndex)
        {
            this.dbId = dbId;
            this.name = name;
            linkshellWork._globalSave.crestIcon[0] = crestId;
            linkshellWork._globalSave.master = master;
            linkshellWork._globalSave.rank = rank;
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

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override string GetGroupName()
        {
            return name;
        }

        public override uint GetTypeId()
        {
            return Group.CompanyGroup;
        }

        public override List<GroupMember> BuildMemberList()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (LinkshellMember member in members.Values)            
                groupMembers.Add(new GroupMember(member.charaId, -1, 0, false, Server.GetServer().GetSession(member.charaId) != null, Server.GetServer().GetNameForId(member.charaId)));            
            return groupMembers;
        }
    }
}

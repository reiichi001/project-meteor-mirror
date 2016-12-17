using FFXIVClassic_World_Server.Actor.Group.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using FFXIVClassic.Common;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Linkshell : Group
    {
        public ulong dbId;
        public string name;

        public LinkshellWork linkshellWork = new LinkshellWork();

        private List<LinkshellMember> members = new List<LinkshellMember>();

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

        public void AddMember(uint charaId)
        {
            members.Add(new LinkshellMember(charaId, dbId, 0xa));
            members.Sort();
        }

        public void RemoveMember(uint charaId)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].charaId == charaId)
                {
                    members.Remove(members[i]);
                    members.Sort();
                    break;
                }
            }                
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
            foreach (LinkshellMember member in members)            
                groupMembers.Add(new GroupMember(member.charaId, -1, 0, false, Server.GetServer().GetSession(member.charaId) != null, Server.GetServer().GetNameForId(member.charaId)));
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {

            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "linkshellWork._globalSave.master");
            groupWork.addProperty(this, "linkshellWork._globalSave.crestIcon[0]");
            groupWork.addProperty(this, "linkshellWork._globalSave.rank");

            for (int i = 0; i < members.Count; i++)
            {
                linkshellWork._memberSave[i].rank = members[i].rank;
                groupWork.addProperty(this, String.Format("linkshellWork._memberSave[{0}].rank", i));
            }

            groupWork.setTarget("/_init");            
            SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
            test.DebugPrintSubPacket();
            session.clientConnection.QueuePacket(test, true, false);
        }

        public void LoadMembers()
        {
            members = Database.GetLSMembers(this);
        }
    }
}

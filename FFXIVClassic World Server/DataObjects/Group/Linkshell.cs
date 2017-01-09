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

        public LinkshellWork work = new LinkshellWork();

        private List<LinkshellMember> members = new List<LinkshellMember>();

        public Linkshell(ulong dbId,  ulong groupIndex, string name, ushort crestId, uint master, byte rank) : base(groupIndex)
        {
            this.dbId = dbId;
            this.name = name;
            work._globalSave.crestIcon[0] = crestId;
            work._globalSave.master = master;
            work._globalSave.rank = rank;
        }

        public void setMaster(uint actorId)
        {
            work._globalSave.master = (ulong)((0xB36F92 << 8) | actorId);
        }

        public void setCrest(ushort crestId)
        {
            work._globalSave.crestIcon[0] = crestId;
        }

        public void setRank(byte rank = 1)
        {
            work._globalSave.rank = rank;
        }

        public void setMemberRank(int index, byte rank)
        {
            if (members.Count >= index)
                return;
            work._memberSave[index].rank = rank;
        }

        public void AddMember(uint charaId)
        {
            members.Add(new LinkshellMember(charaId, dbId, 0x4));
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

        public override List<GroupMember> BuildMemberList(uint id)
        {
            lock (members)
            {
                List<GroupMember> groupMembers = new List<GroupMember>();
                foreach (LinkshellMember member in members)
                    groupMembers.Add(new GroupMember(member.charaId, -1, 0, false, true, Server.GetServer().GetNameForId(member.charaId)));
                return groupMembers;
            }
        }

        public uint[] GetMemberIds()
        {
            lock (members)
            {
                uint[] memberIds = new uint[members.Count];
                for (int i = 0; i < memberIds.Length; i++)
                    memberIds[i] = members[i].charaId;
                return memberIds;
            }
        }

        public override void SendInitWorkValues(Session session)
        {

            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "work._globalSave.master");
            groupWork.addProperty(this, "work._globalSave.crestIcon[0]");
            groupWork.addProperty(this, "work._globalSave.rank");

            for (int i = 0; i < members.Count; i++)
            {
                work._memberSave[i].rank = members[i].rank;
                groupWork.addProperty(this, String.Format("work._memberSave[{0}].rank", i));
            }

            groupWork.setTarget("/_init");            
            SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
            test.DebugPrintSubPacket();
            session.clientConnection.QueuePacket(test, true, false);
        }

        public void ResendWorkValues()
        {

            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "work._globalSave.master");
            groupWork.addProperty(this, "work._globalSave.crestIcon[0]");
            groupWork.addProperty(this, "work._globalSave.rank");

            for (int i = 0; i < members.Count; i++)
            {
                work._memberSave[i].rank = members[i].rank;
                groupWork.addProperty(this, String.Format("work._memberSave[{0}].rank", i));
            }

            groupWork.setTarget("memberRank");

            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Session session = Server.GetServer().GetSession(members[i].charaId);
                    if (session != null)
                    {
                        SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
                        session.clientConnection.QueuePacket(test, true, false);
                    }
                }
            }
            
        }

        public void LoadMembers()
        {
            members = Database.GetLSMembers(this);
        }

        public void OnPlayerJoin(Session inviteeSession)
        {
            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i].charaId);
                if (session == null)
                    continue;

                if (inviteeSession.Equals(session))
                    session.SendGameMessage(25157, 0x20, (object) 0, (object)inviteeSession, name);
                else
                    session.SendGameMessage(25284, 0x20, (object) 0, (object)Server.GetServer().GetNameForId(inviteeSession.sessionId), name);
            }
        }

        public bool HasMember(uint id)
        {
            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    if (members[i].charaId == id)
                        return true;
                }
                return false;
            }
        }

        public void DisbandRequest(Session session)
        {
            throw new NotImplementedException();
        }

        public void LeaveRequest(Session requestSession)
        {
            uint leaver = requestSession.sessionId;

            //Check if ls contains this person
            if (!HasMember(leaver))
            {
                return;
            }

            //Send you are leaving message
            requestSession.SendGameMessage(25162, 0x20, (Object)1, (Object)Server.GetServer().GetNameForId(leaver));

            //All good, remove
            Server.GetServer().GetWorldManager().GetLinkshellManager().RemoveMemberFromLinkshell(requestSession.sessionId, name);
            SendGroupPacketsAll(GetMemberIds());
            ResendWorkValues();
        }

        public void RankChangeRequest(Session requestSession, string name, byte rank)
        {
            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    if (Server.GetServer().GetNameForId(members[i].charaId).Equals(name))
                    {
                        members[i].rank = rank;
                        ResendWorkValues();
                        requestSession.SendGameMessage(25277, 0x20, (object)(100000 + rank), (object)name);
                        return;
                    }
                }                
            }


        }

    }
}

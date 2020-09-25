/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.Collections.Generic;

using Meteor.Common;
using Meteor.World.Actor.Group.Work;
using Meteor.World.Packets.Send.Subpackets.Groups;

namespace Meteor.World.DataObjects.Group
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

        public void AddMember(uint charaId, byte rank = LinkshellManager.RANK_MEMBER)
        {
            members.Add(new LinkshellMember(charaId, dbId, rank));
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
            SubPacket test = groupWork.buildPacket(session.sessionId);
            test.DebugPrintSubPacket();
            session.clientConnection.QueuePacket(test);
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
                        SubPacket test = groupWork.buildPacket(session.sessionId);
                        session.clientConnection.QueuePacket(test);
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

        public LinkshellMember GetMember(string name)
        {
            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    if (Server.GetServer().GetNameForId((members[i].charaId)).Equals(name))
                        return members[i];
                }
                return null;
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
            requestSession.SendGameMessage(25162, 0x20, (Object)1, name);

            //All good, remove
            Server.GetServer().GetWorldManager().GetLinkshellManager().RemoveMemberFromLinkshell(requestSession.sessionId, name);
            SendGroupPacketsAll(GetMemberIds());
            ResendWorkValues();

            //If active, remove it
            if (requestSession.activeLinkshellName.Equals(name))
            {
                SubPacket activeLsPacket = SetActiveLinkshellPacket.BuildPacket(requestSession.sessionId, 0);
                requestSession.clientConnection.QueuePacket(activeLsPacket);
                requestSession.SetActiveLS("");
            }

            //Delete group for kicked guy
            SendDeletePacket(requestSession);
        }

        public void KickRequest(Session requestSession, string kickedName)
        {
            LinkshellMember kicked = GetMember(kickedName);
            Session kickedSession = Server.GetServer().GetSession(kicked.charaId);

            //Check if ls contains this person
            if (!HasMember(kicked.charaId))
            {
                requestSession.SendGameMessage(25281, 0x20, (Object)1, (Object)kickedName, (Object)name);
                return;
            }
            
            //Send you are exiled message
            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    Session session = Server.GetServer().GetSession(members[i].charaId);

                    if (session == null)
                        continue;
                    
                    if (session.sessionId == kicked.charaId)
                        session.SendGameMessage(25184, 0x20, (Object)1, (Object)name);                    
                    else
                        session.SendGameMessage(25280, 0x20, (Object)1, (Object)kickedName, (Object)name);           
                }
            }            

            //All good, remove
            Server.GetServer().GetWorldManager().GetLinkshellManager().RemoveMemberFromLinkshell(kicked.charaId, name);
            SendGroupPacketsAll(GetMemberIds());
            ResendWorkValues();

            //If active, remove it
            if (requestSession.activeLinkshellName.Equals(name))
            {
                SubPacket activeLsPacket = SetActiveLinkshellPacket.BuildPacket(requestSession.sessionId, 0);
                requestSession.clientConnection.QueuePacket(activeLsPacket);
                requestSession.SetActiveLS("");
            }

            //Delete group for kicked guy
            SendDeletePacket(kickedSession);
            
        }

        public void RankChangeRequest(Session requestSession, string name, byte rank)
        {
            lock (members)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    if (Server.GetServer().GetNameForId(members[i].charaId).Equals(name))
                    {
                        if (Database.LinkshellChangeRank(members[i].charaId, rank))
                        {
                            members[i].rank = rank;
                            ResendWorkValues();
                            requestSession.SendGameMessage(25277, 0x20, (object)(100000 + rank), (object)name);
                        }
                        return;
                    }
                }                
            }


        }

    }
}

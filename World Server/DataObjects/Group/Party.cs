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
    class Party : Group
    {
        public PartyWork partyGroupWork = new PartyWork();
        public List<uint> members = new List<uint>();

        public Party(ulong groupId, uint leaderCharaId) : base(groupId)
        {
            partyGroupWork._globalTemp.owner = (ulong)(((ulong)leaderCharaId << 32) | 0xB36F92);
            members.Add(leaderCharaId);
        }

        public void SetLeaderPlayerRequest(Session requestSession, string name)
        {
            SetLeaderPlayerRequest(requestSession, GetIdForName(name));
        }

        public void SetLeaderPlayerRequest(Session requestSession, uint actorId)
        {
            if (GetLeader() != requestSession.sessionId)
            {
                requestSession.SendGameMessage(30428, 0x20, Server.GetServer().GetNameForId(requestSession.sessionId));
                return;
            }

            uint newLeader = actorId;

            if (!members.Contains(actorId))
            {
                requestSession.SendGameMessage(30567, 0x20);
                return;
            }
            else if (newLeader == GetLeader())
            {
                requestSession.SendGameMessage(30559, 0x20, (Object)Server.GetServer().GetNameForId(actorId));
                return;
            }

            SetLeader(newLeader);
            SendLeaderWorkToAllMembers();

            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;
                session.SendGameMessage(30429, 0x20, (Object)Server.GetServer().GetNameForId(actorId));
            }

            Server.GetServer().GetWorldManager().SendPartySync(this);     
        }

        public void KickPlayerRequest(Session requestSession, string name)
        {
            KickPlayerRequest(requestSession, GetIdForName(name));
        }

        public void KickPlayerRequest(Session requestSession, uint actorId)
        {
            if (GetLeader() != requestSession.sessionId)
            {
                requestSession.SendGameMessage(30428, 0x20, Server.GetServer().GetNameForId(requestSession.sessionId));
                return;
            }

            uint kickedMemberId = actorId;

            if (!members.Contains(actorId))
            {
                requestSession.SendGameMessage(30575, 0x20);
                return;
            }           

            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;

                if (members[i] == kickedMemberId)
                    session.SendGameMessage(30410, 0x20);
                else
                    session.SendGameMessage(30428, 0x20, (Object)Server.GetServer().GetNameForId(actorId));
            }

            //All good, remove
            Server.GetServer().GetWorldManager().GetPartyManager().RemoveFromParty(groupIndex, kickedMemberId);
            SendGroupPacketsAll(members);
            Server.GetServer().GetWorldManager().SendPartySync(this);

            //Set the kicked guy to a new party
            Session kickedSession = Server.GetServer().GetSession(kickedMemberId);
            if (kickedSession != null)
            {
                Party kickedPlayersNewParty = Server.GetServer().GetWorldManager().GetPartyManager().CreateParty(kickedMemberId);
                kickedPlayersNewParty.SendGroupPackets(kickedSession);
                Server.GetServer().GetWorldManager().SendPartySync(kickedPlayersNewParty);
                kickedPlayersNewParty.SendInitWorkValues(kickedSession);
            }
        }

        public void LeavePlayerRequest(Session requestSession)
        {
            uint leaver = requestSession.sessionId;

            //Check if party contains this person
            if (!members.Contains(leaver))
            {
                
                return;
            }

            //Send you are leaving messages
            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;

                session.SendGameMessage(30431, 0x20, (Object)Server.GetServer().GetNameForId(leaver));
            }

            //All good, remove
            Server.GetServer().GetWorldManager().GetPartyManager().RemoveFromParty(groupIndex, leaver);
            SendGroupPacketsAll(members);
            Server.GetServer().GetWorldManager().SendPartySync(this);

            //Set the left guy to a new party            
            if (requestSession != null)
            {
                Party kickedPlayersNewParty = Server.GetServer().GetWorldManager().GetPartyManager().CreateParty(leaver);
                kickedPlayersNewParty.SendGroupPackets(requestSession);
                Server.GetServer().GetWorldManager().SendPartySync(kickedPlayersNewParty);
                kickedPlayersNewParty.SendInitWorkValues(requestSession);
            }

        }

        public void DisbandPlayerRequest(Session requestSession)
        {
            uint disbander = requestSession.sessionId;

            //Check if leader
            if (GetLeader() != disbander)
            {
                requestSession.SendGameMessage(30428, 0x20, Server.GetServer().GetNameForId(requestSession.sessionId));
                return;
            }

            Server.GetServer().GetWorldManager().GetPartyManager().DeleteParty(groupIndex);

            //Send game messages and set new parties
            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;

                session.SendGameMessage(30401, 0x20);

                //Set char to new party    
                Party newParty = Server.GetServer().GetWorldManager().GetPartyManager().CreateParty(members[i]);
                newParty.SendGroupPackets(session);
                Server.GetServer().GetWorldManager().SendPartySync(newParty);
                newParty.SendInitWorkValues(session);                
            }

            Server.GetServer().GetWorldManager().SendPartySync(this);
        }

        public void SendLeaderWorkToAllMembers()
        {
            for (int i = 0; i < members.Count; i++)
            {
                SynchGroupWorkValuesPacket leaderUpdate = new SynchGroupWorkValuesPacket(groupIndex);
                leaderUpdate.addProperty(this, "partyGroupWork._globalTemp.owner");
                leaderUpdate.setTarget("partyGroupWork/leader");
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;
                else                
                    session.clientConnection.QueuePacket(leaderUpdate.buildPacket(session.sessionId));                
            }
        }

        public void SetLeader(uint actorId)
        {
            partyGroupWork._globalTemp.owner = (ulong)(((ulong)actorId << 32) | 0xB36F92);
        }

        public uint GetLeader()
        {
            return (uint)(((ulong)partyGroupWork._globalTemp.owner >> 32) & 0xFFFFFFFF);
        }
        
        public uint GetIdForName(string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (Server.GetServer().GetNameForId(members[i]).Equals(name))
                {
                    return members[i];
                }
            }
            return 0;
        }

        public bool IsInParty(uint charaId)
        {
            return members.Contains(charaId);
        }        

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "partyGroupWork._globalTemp.owner");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId);
            session.clientConnection.QueuePacket(test);
            test.DebugPrintSubPacket();
        }        

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override uint GetTypeId()
        {
            return Group.PlayerPartyGroup;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetServer().GetNameForId(id)));
            foreach (uint charaId in members)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetServer().GetNameForId(charaId)));
            }
            return groupMembers;
        }

        public void OnPlayerJoin(Session inviteeSession)
        {
            for (int i = 0; i < members.Count; i++)
            {
                Session session = Server.GetServer().GetSession(members[i]);
                if (session == null)
                    continue;

                session.SendGameMessage(30427, 0x20, (Object)Server.GetServer().GetNameForId(inviteeSession.sessionId));
            }
        }

    }
}

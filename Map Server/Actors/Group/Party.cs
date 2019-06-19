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

using Meteor.Map.actors.group.Work;
using Meteor.Map.packets.send.group;
using System.Collections.Generic;

namespace Meteor.Map.actors.group
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
                if (Server.GetWorldManager().GetActorInWorld(members[i]).customDisplayName.Equals(name))
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
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(id).customDisplayName));
            foreach (uint charaId in members)
            {                
                var chara = Server.GetWorldManager().GetActorInWorld(charaId);
                if (charaId != id && chara != null)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, chara.customDisplayName));
            }
            return groupMembers;
        }

        public void AddMember(uint memberId)
        {
            members.Add(memberId);
            SendGroupPacketsAll(members);
        }

        public void RemoveMember(uint memberId)
        {
            members.Remove(memberId);
            SendGroupPacketsAll(members);
            if (members.Count == 0)
                Server.GetWorldManager().NoMembersInParty(this);
        }
    }
}

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

using Meteor.Common;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.group;
using Meteor.Map.packets.send.groups;
using System.Collections.Generic;

namespace Meteor.Map.actors.group
{
    class MonsterParty : Group
    {
        private List<uint> monsterMembers = new List<uint>();

        public MonsterParty(ulong groupIndex, uint[] initialMonsterMembers)
            : base(groupIndex)
        {
            if(initialMonsterMembers != null)
                for (int i = 0; i < initialMonsterMembers.Length; i++)
                    monsterMembers.Add(initialMonsterMembers[i]);
        }

        public void AddMember(uint memberId)
        {
            monsterMembers.Add(memberId);
            SendGroupPacketsAll(monsterMembers);
        }

        public void RemoveMember(uint memberId)
        {
            monsterMembers.Remove(memberId);
            SendGroupPacketsAll(monsterMembers);
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(id).customDisplayName));
            foreach (uint charaId in monsterMembers)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(charaId).customDisplayName));
            }
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id);
            session.QueuePacket(test);
        }

        public override uint GetTypeId()
        {
            return Group.MonsterPartyGroup;
        }

    }
}

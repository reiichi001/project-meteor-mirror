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
using Meteor.Map.actors.group.Work;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.group;
using Meteor.Map.packets.send.groups;
using System.Collections.Generic;

namespace Meteor.Map.actors.group
{
    class TradeGroup : Group
    {
        public RelationWork work = new RelationWork();
        private uint charaOther;
        private ulong topicGroup;

        public TradeGroup(ulong groupIndex, uint host, uint other)
            : base(groupIndex)
        {
            this.charaOther = other;
            work._globalTemp.host = ((ulong)host << 32) | (0xc17909);
            work._globalTemp.variableCommand = 30001;
        }

        public uint GetHost()
        {
            return (uint)(((ulong)work._globalTemp.host >> 32) & 0xFFFFFFFF);
        }

        public uint GetOther()
        {
            return charaOther;
        }

        public override int GetMemberCount()
        {
            return 2;
        }

        public override uint GetTypeId()
        {
            return Group.TradeRelationGroup;
        }

        public ulong GetTopicGroupIndex()
        {
            return topicGroup;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            uint hostId = (uint)((work._globalTemp.host >> 32) & 0xFFFFFFFF);

            groupMembers.Add(new GroupMember(hostId, -1, 0, false, Server.GetServer().GetSession(hostId) != null, Server.GetWorldManager().GetActorInWorld(hostId).customDisplayName));
            groupMembers.Add(new GroupMember(charaOther, -1, 0, false, Server.GetServer().GetSession(charaOther) != null, Server.GetWorldManager().GetActorInWorld(charaOther).customDisplayName));
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "work._globalTemp.host");
            groupWork.addProperty(this, "work._globalTemp.variableCommand");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id);
            test.DebugPrintSubPacket();
            session.QueuePacket(test);
        }

    }
}

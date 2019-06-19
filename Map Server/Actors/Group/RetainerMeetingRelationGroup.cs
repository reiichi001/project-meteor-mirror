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
using Meteor.Map.actors.chara.npc;
using Meteor.Map.Actors;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.group;
using Meteor.Map.packets.send.groups;
using System.Collections.Generic;

namespace Meteor.Map.actors.group
{
    class RetainerMeetingRelationGroup : Group
    {
        Player player;
        Retainer retainer;

        public RetainerMeetingRelationGroup(ulong groupIndex, Player player, Retainer retainer)
            : base(groupIndex)
        {
            this.player = player;
            this.retainer = retainer;
        }

        public override int GetMemberCount()
        {
            return 2;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            groupMembers.Add(new GroupMember(player.actorId, -1, 0x83, false, true, player.customDisplayName));
            groupMembers.Add(new GroupMember(retainer.actorId, -1, 0x83, false, true, retainer.customDisplayName));
            
            return groupMembers;
        }

        public override uint GetTypeId()
        {
            return 50003;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id);
            test.DebugPrintSubPacket();
            session.QueuePacket(test);
        }

    }
}

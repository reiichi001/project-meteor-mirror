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
    class RetainerGroup : Group
    {
        public RetainerWork work = new RetainerWork();
        public uint owner;
        public List<RetainerGroupMember> members = new List<RetainerGroupMember>();

        public RetainerGroup(ulong groupId, uint owner) : base(groupId)
        {
            this.owner = owner;
        }

        public void setRetainerProperties(int index, byte cdIDOffset, ushort placeName, byte condition, byte level)
        {
            if (members.Count >= index)
                return;
            work._memberSave[index].cdIDOffset = cdIDOffset;
            work._memberSave[index].placeName = placeName;
            work._memberSave[index].conditions = condition;
            work._memberSave[index].level = level;
        }        
        
        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);

            for (int i = 0; i < members.Count; i++)
            {
                work._memberSave[i].cdIDOffset = members[i].cdIDOffset;
                work._memberSave[i].placeName = members[i].placeName;
                work._memberSave[i].conditions = members[i].conditions;
                work._memberSave[i].level = members[i].level;

                groupWork.addProperty(this, String.Format("work._memberSave[{0}].cdIDOffset", i));
                groupWork.addProperty(this, String.Format("work._memberSave[{0}].placeName", i));
                groupWork.addProperty(this, String.Format("work._memberSave[{0}].conditions", i));
                groupWork.addProperty(this, String.Format("work._memberSave[{0}].level", i));
            }
            
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId);
            session.clientConnection.QueuePacket(test);
        }
        
        public override int GetMemberCount()
        {
            return members.Count + 1;
        }

        public override uint GetTypeId()
        {
            return Group.RetainerGroup;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();                   

            //Add retainers
            foreach (RetainerGroupMember member in members)
                groupMembers.Add(new GroupMember(member.id, -1, 0, false, true, member.name));

            //Add player
            groupMembers.Add(new GroupMember(owner, -1, 0, false, true, Server.GetServer().GetNameForId(owner)));

            return groupMembers;
        }
    }
}

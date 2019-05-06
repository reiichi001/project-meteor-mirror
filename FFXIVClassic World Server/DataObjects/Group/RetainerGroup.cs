using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Actor.Group.Work;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server.DataObjects.Group
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

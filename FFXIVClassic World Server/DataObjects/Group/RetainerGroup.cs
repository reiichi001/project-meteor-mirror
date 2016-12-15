using FFXIVClassic_World_Server.Actor.Group.Work;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class RetainerGroup : Group
    {
        public RetainerWork work = new RetainerWork();
        public uint owner;
        public Dictionary<uint, RetainerGroupMember> members = new Dictionary<uint, RetainerGroupMember>();

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

        /*
        public override void sendWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupId);
            groupWork.addProperty(this, "work._memberSave[0].cdIDOffset");
            groupWork.addProperty(this, "work._memberSave[0].placeName");
            groupWork.addProperty(this, "work._memberSave[0].conditions");
            groupWork.addProperty(this, "work._memberSave[0].level");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
            session.clientConnection.QueuePacket(test, true, false);
        }
        */

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override uint GetTypeId()
        {
            return Group.RetainerGroup;
        }

        public override List<GroupMember> BuildMemberList()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (RetainerGroupMember member in members.Values)
                groupMembers.Add(new GroupMember(member.retainerId, -1, 0, false, true, member.name));
            return groupMembers;
        }
    }
}

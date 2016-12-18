using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Actor.Group.Work;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Relation : Group
    {
        public RelationWork work = new RelationWork();
        public uint charaOther;

        public Relation(ulong groupIndex, uint host, uint other, uint command) : base (groupIndex)
        {
            this.charaOther = other;
            work._globalTemp.host = ((ulong)host << 32) | (0xc17909);
            work._globalTemp.variableCommand = command;
        }

        public override int GetMemberCount()
        {
            return 2;
        }

        public override uint GetTypeId()
        {
            return Group.GroupInvitationRelationGroup;
        }

        public override List<GroupMember> BuildMemberList()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            uint hostId = (uint)((work._globalTemp.host >> 32) & 0xFFFFFFFF);

            groupMembers.Add(new GroupMember(hostId, -1, 0, false, Server.GetServer().GetSession(hostId) != null, Server.GetServer().GetNameForId(hostId)));
            groupMembers.Add(new GroupMember(charaOther, -1, 0, false, Server.GetServer().GetSession(charaOther) != null, Server.GetServer().GetNameForId(charaOther)));
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "work._globalTemp.host");
            groupWork.addProperty(this, "work._globalTemp.variableCommand");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
            test.DebugPrintSubPacket();
            session.clientConnection.QueuePacket(test, true, false);
        }

    }
}

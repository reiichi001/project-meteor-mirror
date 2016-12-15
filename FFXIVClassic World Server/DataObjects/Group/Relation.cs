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
        public uint charaHost, charaOther;
        public uint command;

        public Relation(ulong groupIndex, uint host, uint other, uint command) : base (groupIndex)
        {
            this.charaHost = host;
            this.charaOther = other;
            this.command = command;
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
            groupMembers.Add(new GroupMember(charaHost, -1, 0, false, Server.GetServer().GetSession(charaHost) != null, Server.GetServer().GetNameForId(charaHost)));
            groupMembers.Add(new GroupMember(charaOther, -1, 0, false, Server.GetServer().GetSession(charaOther) != null, Server.GetServer().GetNameForId(charaOther)));
            return groupMembers;
        }

    }
}

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.work;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
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
            return null;
        }

        public override void SendInitWorkValues(Session session)
        {
           
        }

    }
}

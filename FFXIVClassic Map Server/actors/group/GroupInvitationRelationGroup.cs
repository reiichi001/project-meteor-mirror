using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.work;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class GroupInvitationRelationGroup : Group
    {
        RelationWork work;

        public GroupInvitationRelationGroup(uint id, uint hostActorId, uint commandType) : base(id, Group.GroupInvitationRelationGroup)
        {
            work = new RelationWork();
            work._globalTemp.host = hostActorId;
            work._globalTemp.variableCommand = commandType;
        }

        public override void sendWorkValues(Player player)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupId);
            groupWork.addProperty(this, "work._globalTemp.host");
            groupWork.addProperty(this, "work._globalTemp.variableCommand");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(player.actorId, player.actorId);
            test.DebugPrintSubPacket();
            player.QueuePacket(test);
        }

    }
}

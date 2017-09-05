﻿using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Actor.Group.Work;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class RetainerMeetingRelationGroup : Relation
    {
        public RetainerMeetingRelationGroup(ulong groupIndex, uint host, uint other, uint command, ulong topicGroup)
            : base(groupIndex, host, other, command, topicGroup)
        {

        }

        public override uint GetTypeId()
        {
            return Group.RetainerMeetingRelationGroup;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId);
            test.DebugPrintSubPacket();
            session.clientConnection.QueuePacket(test);
        }

    }
}
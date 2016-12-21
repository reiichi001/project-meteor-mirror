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
    class Party : Group
    {
        public PartyWork partyGroupWork = new PartyWork();
        public List<uint> members = new List<uint>();

        public Party(ulong groupId, uint leaderCharaId) : base(groupId)
        {
            partyGroupWork._globalTemp.owner = (ulong)(((ulong)leaderCharaId << 32) | 0xB36F92);
            members.Add(leaderCharaId);
        }

        public void SetLeader(uint actorId)
        {
            partyGroupWork._globalTemp.owner = (ulong)((actorId << 32) | 0xB36F92);
        }

        public uint GetLeader()
        {
            return (uint)((partyGroupWork._globalTemp.owner >> 32) & 0xFFFFFFFF);
        }
        
        public bool IsInParty(uint charaId)
        {
            return members.Contains(charaId);
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "partyGroupWork._globalTemp.owner");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.sessionId, session.sessionId);
            session.clientConnection.QueuePacket(test, true, false);
            test.DebugPrintSubPacket();
        }        

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override uint GetTypeId()
        {
            return Group.PlayerPartyGroup;
        }

        public override List<GroupMember> BuildMemberList()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (uint charaId in members)
                groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetServer().GetNameForId(charaId)));
            return groupMembers;
        }

    }
}

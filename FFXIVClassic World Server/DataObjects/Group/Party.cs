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
            partyGroupWork._globalTemp.owner = (ulong)((0xB36F92 << 8) | leaderCharaId);
        }

        public void SetLeader(uint actorId)
        {
            partyGroupWork._globalTemp.owner = (ulong)((0xB36F92 << 8) | actorId);
        }

        public uint GetLeader()
        {
            return (uint)(partyGroupWork._globalTemp.owner & 0xFFFFFF);
        }

        /*
        public override void sendWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupId);
            groupWork.addProperty(this, "partyGroupWork._globalTemp.owner");
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
            return Group.PlayerPartyGroup;
        }

        public override List<GroupMember> BuildMemberList()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            foreach (uint charaId in members)
                groupMembers.Add(new GroupMember(charaId, -1, 0, false, Server.GetServer().GetSession(charaId) != null, Server.GetServer().GetNameForId(charaId)));
            return groupMembers;
        }

    }
}

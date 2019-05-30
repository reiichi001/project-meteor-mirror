using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class MonsterParty : Group
    {
        private List<uint> monsterMembers = new List<uint>();

        public MonsterParty(ulong groupIndex, uint[] initialMonsterMembers)
            : base(groupIndex)
        {
            if(initialMonsterMembers != null)
                for (int i = 0; i < initialMonsterMembers.Length; i++)
                    monsterMembers.Add(initialMonsterMembers[i]);
        }

        public void AddMember(uint memberId)
        {
            monsterMembers.Add(memberId);
            SendGroupPacketsAll(monsterMembers);
        }

        public void RemoveMember(uint memberId)
        {
            monsterMembers.Remove(memberId);
            SendGroupPacketsAll(monsterMembers);
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(id).customDisplayName));
            foreach (uint charaId in monsterMembers)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(charaId).customDisplayName));
            }
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id);
            session.QueuePacket(test);
        }

        public override uint GetTypeId()
        {
            return Group.MonsterPartyGroup;
        }

    }
}

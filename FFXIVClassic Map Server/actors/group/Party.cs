using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.Work;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
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
            partyGroupWork._globalTemp.owner = (ulong)(((ulong)actorId << 32) | 0xB36F92);
        }

        public uint GetLeader()
        {
            return (uint)(((ulong)partyGroupWork._globalTemp.owner >> 32) & 0xFFFFFFFF);
        }
        
        public uint GetIdForName(string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (Server.GetWorldManager().GetActorInWorld(members[i]).customDisplayName.Equals(name))
                {
                    return members[i];
                }
            }
            return 0;
        }

        public bool IsInParty(uint charaId)
        {
            return members.Contains(charaId);
        }        

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override uint GetTypeId()
        {
            return Group.PlayerPartyGroup;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(id).customDisplayName));
            foreach (uint charaId in members)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(charaId).customDisplayName));
            }
            return groupMembers;
        }

    }
}

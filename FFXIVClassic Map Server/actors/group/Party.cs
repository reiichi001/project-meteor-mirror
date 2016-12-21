using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.work;
using FFXIVClassic_Map_Server.dataobjects;
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

        public void SetLeader(uint leaderCharaId)
        {
            partyGroupWork._globalTemp.owner = (ulong)(((ulong)leaderCharaId << 32) | 0xB36F92);
        }

        public uint GetLeader()
        {
            return (uint)((ulong)(partyGroupWork._globalTemp.owner >> 32) & 0xFFFFFFFF);
        }
        
        public bool IsInParty(uint charaId)
        {
            return members.Contains(charaId);
        }

        public override void SendInitWorkValues(Session session)
        {           
        }        

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override uint GetTypeId()
        {
            return Group.PlayerPartyGroup;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Party : Group
    {
        public uint leader;
        public List<uint> members = new List<uint>();

        public Party(ulong groupId, uint leaderCharaId) : base(groupId)
        {
            this.leader = leaderCharaId;
        }
    }
}

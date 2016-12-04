using FFXIVClassic_Map_Server.actors.group.work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class PartyGroup : Group
    {
        private PartyWork partyWork;

        public PartyGroup(ulong id) : base(id, Group.PlayerPartyGroup, null)
        {
            partyWork = new PartyWork();            
        }

        public void setPartyOwner(uint actorId)
        {
            partyWork._globalTemp.owner = (ulong)((0xB36F92 << 8) | actorId);
        }
    }
}

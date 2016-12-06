using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.work;
using FFXIVClassic_Map_Server.packets.send.groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class PartyGroup : Group
    {
        private PartyWork partyGroupWork;

        public PartyGroup(ulong id, uint owner) : base(id, Group.PlayerPartyGroup)
        {
            partyGroupWork = new PartyWork();
            partyGroupWork._globalTemp.owner = (ulong)((0xB36F92 << 8) | owner);
        }

        public void setPartyOwner(uint actorId)
        {
            partyGroupWork._globalTemp.owner = (ulong)((0xB36F92 << 8) | actorId);
        }

        public override void sendWorkValues(Actors.Player player)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupId);
            groupWork.addProperty(this, "partyGroupWork._globalTemp.owner");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(player.actorId, player.actorId);
            player.QueuePacket(test);
        }
    }
}

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
    class RetainerGroup : Group
    {
        private RetainerWork work;

        public RetainerGroup(ulong id) : base(id, Group.RetainerGroup)
        {
            work = new RetainerWork();            
        }

        public void setRetainerProperties(int index, byte cdIDOffset, ushort placeName, byte condition, byte level)
        {
            if (members.Count >= index)
                return;
            work._memberSave[index].cdIDOffset = cdIDOffset;
            work._memberSave[index].placeName = placeName;
            work._memberSave[index].conditions = condition;
            work._memberSave[index].level = level;
        }

        public override void sendWorkValues(Actors.Player player)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupId);
            groupWork.addProperty(this, "work._memberSave[0].cdIDOffset");
            groupWork.addProperty(this, "work._memberSave[0].placeName");
            groupWork.addProperty(this, "work._memberSave[0].conditions");
            groupWork.addProperty(this, "work._memberSave[0].level");
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(player.actorId, player.actorId);
            player.QueuePacket(test);
        }
    }
}

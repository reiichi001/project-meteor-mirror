using FFXIVClassic_Map_Server.actors.group.work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class RetainerGroup : Group
    {
        private RetainerWork retainerWork;

        public RetainerGroup(ulong id) : base(id, Group.RetainerGroup, null)
        {
            retainerWork = new RetainerWork();            
        }

        public void setRetainerProperties(int index, byte cdIDOffset, ushort placeName, byte condition, byte level)
        {
            if (members.Count >= index)
                return;
            retainerWork._memberSave[index].cdIDOffset = cdIDOffset;
            retainerWork._memberSave[index].placeName = placeName;
            retainerWork._memberSave[index].conditions = condition;
            retainerWork._memberSave[index].level = level;
        }
    }
}

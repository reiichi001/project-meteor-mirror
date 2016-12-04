using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group.work
{
    class GroupMemberSave
    {
        //For LS
        public byte rank;

        //For Retainers
        public byte cdIDOffset;
        public ushort placeName;
        public byte conditions;
        public byte level;
    }
}

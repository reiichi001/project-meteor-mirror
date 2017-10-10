using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.Actor.Group.Work
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

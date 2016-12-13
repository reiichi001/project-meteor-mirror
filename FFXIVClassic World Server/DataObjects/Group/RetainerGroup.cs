using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class RetainerGroup : Group
    {
        public uint owner;
        public Dictionary<uint, RetainerGroupMember> members = new Dictionary<uint, RetainerGroupMember>();

        public RetainerGroup(ulong groupId, uint owner) : base(groupId)
        {
            this.owner = owner;
        }
    }
}

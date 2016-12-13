using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Group
    {
        public readonly ulong groupIndex;

        public Group(ulong groupIndex)
        {
            this.groupIndex = groupIndex;
        }
    }
}

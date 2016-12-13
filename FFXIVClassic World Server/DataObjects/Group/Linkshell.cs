using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Linkshell : Group
    {
        public ulong dbId;
        public string name;
        public ushort crestId;
        public uint master;
        public ushort rank;

        public Dictionary<ulong, LinkshellMember> members = new Dictionary<ulong, LinkshellMember>();

        public Linkshell(ulong dbId,  ulong groupIndex, string name, ushort crestId, uint master, ushort rank) : base(groupIndex)
        {
            this.dbId = dbId;
            this.name = name;
            this.crestId = crestId;
            this.master = master;
            this.rank = rank;
        }
    }
}

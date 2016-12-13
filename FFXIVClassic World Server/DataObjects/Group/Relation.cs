using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Relation : Group
    {
        public uint charaHost, charaOther;
        public uint command;

        public Relation(ulong groupIndex, uint host, uint other, uint command) : base (groupIndex)
        {
            this.charaHost = host;
            this.charaOther = other;
            this.command = command;
        }
    }
}

using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Judge : Actor
    {
        public Judge(uint actorID, string name) : base(actorID)
        {
            actorName = name;
        }
    }
}

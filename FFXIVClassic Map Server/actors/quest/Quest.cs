using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Quest : Actor
    {

        public Quest(uint actorID, string name)
            : base(actorID)
        {
            actorName = name;
        }

    }
}

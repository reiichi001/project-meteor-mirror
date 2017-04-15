using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.area
{
    class PrivateAreaContent : PrivateArea
    {
        public PrivateAreaContent(Zone parent, uint id, string className, string privateAreaName, uint privateAreaType)
            : base(parent, id, className, privateAreaName, privateAreaType, 0, 0, 0)
        {         
        }
    }
}

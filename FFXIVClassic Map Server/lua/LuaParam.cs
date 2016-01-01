using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.lua
{
    class LuaParam
    {
        public int typeID;
        public Object value;

        public LuaParam(int type, Object value)
        {
            this.typeID = type;
            this.value = value;
        }
    }
}

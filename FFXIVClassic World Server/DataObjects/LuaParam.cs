using System;

namespace FFXIVClassic_World_Server.DataObjects
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

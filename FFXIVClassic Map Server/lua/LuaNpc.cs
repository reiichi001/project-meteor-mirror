using FFXIVClassic_Map_Server.Actors;
using MoonSharp.Interpreter;

namespace FFXIVClassic_Map_Server.lua
{

    [MoonSharpUserData]
    class LuaNpc
    {
        private Npc npc;

        public LuaNpc(Npc npc)
        {
            this.npc = npc;
        }

      
    }
}

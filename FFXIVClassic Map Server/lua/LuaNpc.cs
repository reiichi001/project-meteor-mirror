using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

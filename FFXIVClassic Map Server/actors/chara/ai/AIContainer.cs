using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;

// port of ai code in dsp by kjLotus
namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    // todo: actually implement stuff
    class AIContainer
    {
        private Character owner;
        private Controller controller;
        private List<State> states;
        private DateTime latestUpdate;
        private DateTime prevUpdate;

        public AIContainer(Actors.Character actor)
        {
            this.owner = actor;
        }

        public void ChangeController(Controller controller)
        {
            this.controller = controller;
        }
    }
}

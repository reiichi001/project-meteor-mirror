using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class AttackState : State
    {
        public AttackState(Character owner, Character target) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
        }

        public override void OnStart()
        {

        }

        public override void Update(DateTime time)
        {

        }

        public override void OnInterrupt()
        {

        }

        public override void OnComplete()
        {

        }

        public override void TryInterrupt()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class State
    {
        protected Character owner;
        protected Character target;

        protected bool canInterrupt;
        protected bool interrupt;

        protected DateTime startTime;

        protected SubPacket errorPacket;

        public State(Character owner, Character target)
        {
            this.owner = owner;
            this.target = target;
            this.canInterrupt = true;
            this.interrupt = false;
        }

        public virtual void Update(ref DateTime time) { }
        public virtual void OnStart() { }
        public virtual void OnInterrupt() { }
        public virtual void OnComplete() { }

        public virtual void TryInterrupt() { }

        public bool CanInterrupt()
        {
            return canInterrupt;
        }

        public void SetInterrupted(bool interrupt)
        {
            this.interrupt = interrupt;
        }
    }
}

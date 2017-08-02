﻿using System;
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

        protected bool isCompleted;

        public State(Character owner, Character target)
        {
            this.owner = owner;
            this.target = target;
            this.canInterrupt = true;
            this.interrupt = false;
        }

        public virtual bool Update(DateTime tick) { return true; }
        public virtual void OnStart() {  }
        public virtual void OnInterrupt() { }
        public virtual void OnComplete() { isCompleted = true; }
        public virtual bool CanChangeState() { return false; }
        public virtual void TryInterrupt() { }

        public virtual void Cleanup() { }

        public bool CanInterrupt()
        {
            return canInterrupt;
        }

        public void SetInterrupted(bool interrupt)
        {
            this.interrupt = interrupt;
        }

        public bool IsCompleted()
        {
            return isCompleted;
        }

        public void ChangeTarget(Character target)
        {
            this.target = target;
        }

    }
}

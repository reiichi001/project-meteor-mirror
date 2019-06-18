/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor.battle;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class State
    {
        protected Character owner;
        protected Character target;

        protected bool canInterrupt;
        protected bool interrupt = false;

        protected DateTime startTime;

        protected CommandResult errorResult;

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

        public Character GetTarget()
        {
            return target;
        }

    }
}

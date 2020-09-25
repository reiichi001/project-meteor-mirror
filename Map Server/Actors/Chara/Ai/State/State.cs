/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using Meteor.Map.Actors;
using Meteor.Map.packets.send.actor.battle;

namespace Meteor.Map.actors.chara.ai.state
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

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

namespace Meteor.Map.actors.chara.ai.state
{
    class InactiveState : State
    {
        private DateTime endTime;
        private uint durationMs;
        public InactiveState(Character owner, uint durationMs, bool canChangeState) :
            base(owner, null)
        {
            if (!canChangeState)
                owner.aiContainer.InterruptStates();
            this.durationMs = durationMs;
            endTime = DateTime.Now.AddMilliseconds(durationMs);
        }

        public override bool Update(DateTime tick)
        {
            if (durationMs == 0)
            {
                if (owner.IsDead())
                    return true;

                if (!owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventMovement))
                    return true;
            }

            if (durationMs != 0 && tick > endTime)
            {
                return true;
            }

            return false;
        }
    }
}

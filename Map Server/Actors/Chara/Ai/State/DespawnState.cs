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
using Meteor.Map.packets.send.actor;

namespace Meteor.Map.actors.chara.ai.state
{
    class DespawnState : State
    {
        private DateTime respawnTime;
        public DespawnState(Character owner, uint respawnTimeSeconds) :
            base(owner, null)
        {
            startTime = Program.Tick;
            respawnTime = startTime.AddSeconds(respawnTimeSeconds);
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD2);
            owner.OnDespawn();
        }

        public override bool Update(DateTime tick)
        {
            if (tick >= respawnTime)
            {
                owner.ResetTempVars();
                owner.Spawn(tick);
                return true;
            }
            return false;
        }
    }
}

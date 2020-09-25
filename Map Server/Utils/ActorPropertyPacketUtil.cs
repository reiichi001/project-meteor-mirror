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


using System.Collections.Generic;
using Meteor.Map.packets.send.actor;
using Meteor.Map.Actors;
using Meteor.Common;

namespace Meteor.Map.utils
{
    class ActorPropertyPacketUtil
    {
        private Actor forActor;
        private List<SubPacket> subPackets = new List<SubPacket>();
        private SetActorPropetyPacket currentActorPropertyPacket;
        private string currentTarget;

        public ActorPropertyPacketUtil(string firstTarget, Actor forActor)
        {
            currentActorPropertyPacket = new SetActorPropetyPacket(firstTarget);
            this.forActor = forActor;
            this.currentTarget = firstTarget;
        }

        public void AddProperty(string property)
        {
            if (!currentActorPropertyPacket.AddProperty(forActor, property))
            {
                currentActorPropertyPacket.SetIsMore(true);
                currentActorPropertyPacket.AddTarget();
                subPackets.Add(currentActorPropertyPacket.BuildPacket(forActor.actorId));
                currentActorPropertyPacket = new SetActorPropetyPacket(currentTarget);
                currentActorPropertyPacket.AddProperty(forActor, property);
            }
        }

        public void NewTarget(string target)
        {
            currentActorPropertyPacket.AddTarget();
            currentTarget = target;
            currentActorPropertyPacket.SetTarget(target);            
        }

        public List<SubPacket> Done()
        {
            currentActorPropertyPacket.AddTarget();
            currentActorPropertyPacket.SetIsMore(false);
            subPackets.Add(currentActorPropertyPacket.BuildPacket(forActor.actorId));
            return subPackets;
        }

    }
}

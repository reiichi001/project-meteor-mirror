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


using System.Collections.Generic;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.utils
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

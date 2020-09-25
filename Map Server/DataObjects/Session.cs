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

using Meteor.Common;

using Meteor.Map.Actors;
using Meteor.Map.packets.send.actor;
using System.Collections.Generic;
using Meteor.Map.actors.chara.npc;

namespace Meteor.Map.dataobjects
{
    class Session
    {
        public uint id = 0;
        Player playerActor;
        public List<Actor> actorInstanceList = new List<Actor>();
        public uint languageCode = 1;        
        private uint lastPingPacket = Utils.UnixTimeStampUTC();

        public bool isUpdatesLocked = true;

        public string errorMessage = "";

        public Session(uint sessionId)
        {
            this.id = sessionId;
            playerActor = new Player(this, sessionId);
        }

        public void QueuePacket(List<SubPacket> packets)
        {
            foreach (SubPacket s in packets)
                QueuePacket(s);
        }

        public void QueuePacket(SubPacket subPacket)
        {
            subPacket.SetTargetId(id);
            Server.GetWorldConnection().QueuePacket(subPacket);
        }

        public Player GetActor()
        {
            return playerActor;
        }

        public void Ping()
        {
            lastPingPacket = Utils.UnixTimeStampUTC();
        }

        public bool CheckIfDCing()
        {
            uint currentTime = Utils.UnixTimeStampUTC();
            if (currentTime - lastPingPacket >= 5000) //Show D/C flag
                playerActor.SetDCFlag(true);
            else if (currentTime - lastPingPacket >= 30000) //DCed
                return true;
            else
                playerActor.SetDCFlag(false);
            return false;
        }

        public void UpdatePlayerActorPosition(float x, float y, float z, float rot, ushort moveState)
        {
            if (isUpdatesLocked)
                return;

            if (playerActor.positionX == x && playerActor.positionY == y && playerActor.positionZ == z && playerActor.rotation == rot)
                return;

            /*
            playerActor.oldPositionX = playerActor.positionX;
            playerActor.oldPositionY = playerActor.positionY;
            playerActor.oldPositionZ = playerActor.positionZ;
            playerActor.oldRotation = playerActor.rotation;
            
            playerActor.positionX = x;
            playerActor.positionY = y;
            playerActor.positionZ = z;
            */
            playerActor.rotation = rot;
            playerActor.moveState = moveState;

            //GetActor().GetZone().UpdateActorPosition(GetActor());
            playerActor.QueuePositionUpdate(new Vector3(x,y,z));
        }

        public void UpdateInstance(List<Actor> list, bool force = false)
        {
            if (isUpdatesLocked && !force)
                return;

            List<BasePacket> basePackets = new List<BasePacket>();
            List<SubPacket> RemoveActorSubpackets = new List<SubPacket>();
            List<SubPacket> posUpdateSubpackets = new List<SubPacket>();

            //Remove missing actors
            for (int i = 0; i < actorInstanceList.Count; i++)
            {
                //Retainer Instance
                if (actorInstanceList[i] is Retainer && playerActor.currentSpawnedRetainer == null)
                {
                    QueuePacket(RemoveActorPacket.BuildPacket(actorInstanceList[i].actorId));
                    actorInstanceList.RemoveAt(i);
                }
                else if (!list.Contains(actorInstanceList[i]) && !(actorInstanceList[i] is Retainer))
                {
                    QueuePacket(RemoveActorPacket.BuildPacket(actorInstanceList[i].actorId));
                    actorInstanceList.RemoveAt(i);
                }
            }

            //Retainer Instance
            if (playerActor.currentSpawnedRetainer != null && !playerActor.sentRetainerSpawn)
            {
                Actor actor = playerActor.currentSpawnedRetainer;
                QueuePacket(actor.GetSpawnPackets(playerActor, 1));
                QueuePacket(actor.GetInitPackets());
                QueuePacket(actor.GetSetEventStatusPackets());
                actorInstanceList.Add(actor);
                ((Npc)actor).DoOnActorSpawn(playerActor);
                playerActor.sentRetainerSpawn = true;
            }

            //Add new actors or move
            for (int i = 0; i < list.Count; i++)
            {
                Actor actor = list[i];

                if (actor.actorId == playerActor.actorId)
                    continue;

                if (actorInstanceList.Contains(actor))
                {

                }
                else
                {   
                    QueuePacket(actor.GetSpawnPackets(playerActor, 1));

                    QueuePacket(actor.GetInitPackets());
                    QueuePacket(actor.GetSetEventStatusPackets());
                    actorInstanceList.Add(actor);

                    if (actor is Npc)
                    {
                        ((Npc)actor).DoOnActorSpawn(playerActor);
                    }
                }
            }

        }


        public void ClearInstance()
        {
            actorInstanceList.Clear();
        }


        public void LockUpdates(bool f)
        {
            isUpdatesLocked = f;
        }
    }
}

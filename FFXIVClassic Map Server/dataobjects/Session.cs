using FFXIVClassic_Map_Server;
using FFXIVClassic.Common;

using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;

namespace FFXIVClassic_Map_Server.dataobjects
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
            Server.GetWorldConnection()?.QueuePacket(subPacket);
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

        long lastMilis = 0;
        public void UpdateInstance(List<Actor> list)
        {
            if (isUpdatesLocked)
                return;

            List<BasePacket> basePackets = new List<BasePacket>();
            List<SubPacket> RemoveActorSubpackets = new List<SubPacket>();
            List<SubPacket> posUpdateSubpackets = new List<SubPacket>();

            //Remove missing actors
            for (int i = 0; i < actorInstanceList.Count; i++)
            {
                if (!list.Contains(actorInstanceList[i]))
                {
                    QueuePacket(RemoveActorPacket.BuildPacket(actorInstanceList[i].actorId));
                    actorInstanceList.RemoveAt(i);
                }                
            }

            //Add new actors or move
            for (int i = 0; i < list.Count; i++)
            {
                Actor actor = list[i];

                if (actor.actorId == playerActor.actorId)
                    continue;

                if (actorInstanceList.Contains(actor))
                {
                    //Don't send for static characters (npcs)
                    // todo: this is retarded, need actual mob class
                    //if (actor is Character && ((Character)actor).isStatic)
                    //    continue;

                    //var packet = actor.CreatePositionUpdatePacket();
                    //if (packet != null)
                    //    QueuePacket(packet);
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

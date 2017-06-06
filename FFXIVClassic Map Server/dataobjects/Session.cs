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
            actorInstanceList.Add(playerActor);
        }

        public void QueuePacket(BasePacket basePacket)
        {
            Server.GetWorldConnection().QueuePacket(basePacket);
        }

        public void QueuePacket(SubPacket subPacket, bool isAuthed, bool isEncrypted)
        {
            Server.GetWorldConnection().QueuePacket(subPacket, isAuthed, isEncrypted);
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

            playerActor.oldPositionX = playerActor.positionX;
            playerActor.oldPositionY = playerActor.positionY;
            playerActor.oldPositionZ = playerActor.positionZ;
            playerActor.oldRotation = playerActor.rotation;

            playerActor.positionX = x;
            playerActor.positionY = y;
            playerActor.positionZ = z;
            playerActor.rotation = rot;
            playerActor.moveState = moveState;

            GetActor().zone.UpdateActorPosition(GetActor());

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
                if (list.Contains(actorInstanceList[i]) && actorInstanceList[i] is Npc)
                {
                    Npc npc = (Npc)actorInstanceList[i];
                    

                       long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;


                    if (npc.GetUniqueId().Equals("1") && milliseconds - lastMilis > 1000)
                    {
                        lastMilis = milliseconds;
                        GetActor().QueuePacket(RemoveActorPacket.BuildPacket(playerActor.actorId, actorInstanceList[i].actorId));
                        actorInstanceList.RemoveAt(i);
                        continue;
                    }
                }

                if (!list.Contains(actorInstanceList[i]))
                {
                    GetActor().QueuePacket(RemoveActorPacket.BuildPacket(playerActor.actorId, actorInstanceList[i].actorId));
                    actorInstanceList.RemoveAt(i);
                }
                
            }

            // todo: this is retarded (checking moved crap demo added)
            bool checkedThisTick = false;
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
                    if (actor is Character && !actor.hasMoved)
                        continue;

                    // todo: again, this is retarded but debug stuff
                    var zone = (actors.area.Zone)actor.zone;
                    if(zone != null && !checkedThisTick)
                    {
                        if (zone.pathCalls > 0)
                        {
                            checkedThisTick = true;
                            Program.Log.Error("Number of pathfinding calls {0} average time {1}", zone.pathCalls, zone.pathCallTime / zone.pathCalls);
                        }
                    }

                    var packet = actor.CreatePositionUpdatePacket(playerActor.actorId);

                    if (packet != null)
                        GetActor().QueuePacket(packet);
                }
                else
                {
                    GetActor().QueuePacket(actor.GetSpawnPackets(playerActor.actorId, 1));
                    GetActor().QueuePacket(actor.GetInitPackets(playerActor.actorId));
                    GetActor().QueuePacket(actor.GetSetEventStatusPackets(playerActor.actorId));
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

using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
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
    class ConnectedPlayer
    {
        public uint actorID = 0;
        Player playerActor;
        public List<Actor> actorInstanceList = new List<Actor>();

        public uint languageCode = 1;

        private ClientConnection zoneConnection;
        private ClientConnection chatConnection;

        private uint lastPingPacket = Utils.UnixTimeStampUTC();

        public string errorMessage = "";

        public ConnectedPlayer(uint actorId)
        {
            this.actorID = actorId;
            playerActor = new Player(this, actorId);
            actorInstanceList.Add(playerActor);
        }

        public void setConnection(int type, ClientConnection conn)
        {
            conn.connType = type;
            switch (type)
            {
                case BasePacket.TYPE_ZONE:
                    zoneConnection = conn;
                    break;
                case BasePacket.TYPE_CHAT:
                    chatConnection = conn;
                    break;
            }
        }
       
        public bool isClientConnectionsReady()
        {
            return (zoneConnection != null && chatConnection != null);
        }

        public void disconnect()
        {
            zoneConnection.disconnect();
            chatConnection.disconnect();
        }      

        public bool isDisconnected()
        {
            return (!zoneConnection.isConnected() && !chatConnection.isConnected());
        }

        public void queuePacket(BasePacket basePacket)
        {
            zoneConnection.queuePacket(basePacket);
        }

        public void queuePacket(SubPacket subPacket, bool isAuthed, bool isEncrypted)
        {
                zoneConnection.queuePacket(subPacket, isAuthed, isEncrypted);
        }

        public Player getActor()
        {
            return playerActor;
        }

        public void ping()
        {
            lastPingPacket = Utils.UnixTimeStampUTC();
        }

        public bool checkIfDCing()
        {
            uint currentTime = Utils.UnixTimeStampUTC();
            if (currentTime - lastPingPacket >= 5000) //Show D/C flag
                playerActor.setDCFlag(true);
            else if (currentTime - lastPingPacket >= 30000) //DCed
                return true;
            else
                playerActor.setDCFlag(false);
            return false;
        }

        public void updatePlayerActorPosition(float x, float y, float z, float rot, ushort moveState)
        {            
            playerActor.oldPositionX = playerActor.positionX;
            playerActor.oldPositionY = playerActor.positionY;
            playerActor.oldPositionZ = playerActor.positionZ;
            playerActor.oldRotation = playerActor.rotation;

            playerActor.positionX = x;
            playerActor.positionY = y;
            playerActor.positionZ = z;
            playerActor.rotation = rot;
            playerActor.moveState = moveState;

            getActor().zone.updateActorPosition(getActor());
             
        }            
        
        public void updateInstance(List<Actor> list)
        {            
            List<BasePacket> basePackets = new List<BasePacket>();
            List<SubPacket> removeActorSubpackets = new List<SubPacket>();
            List<SubPacket> posUpdateSubpackets = new List<SubPacket>();

            //Remove missing actors
            for (int i = 0; i < actorInstanceList.Count; i++)
            {
                if (!list.Contains(actorInstanceList[i]))
                {
                    getActor().queuePacket(RemoveActorPacket.buildPacket(playerActor.actorId, actorInstanceList[i].actorId));
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
                    getActor().queuePacket(actor.createPositionUpdatePacket(playerActor.actorId));
                }
                else
                {
                    getActor().queuePacket(actor.getSpawnPackets(playerActor.actorId, 1));
                    getActor().queuePacket(actor.getInitPackets(playerActor.actorId));
                    getActor().queuePacket(actor.getSetEventStatusPackets(playerActor.actorId));                   
                    actorInstanceList.Add(actor);

                    if (actor is Npc)
                    {
                        LuaEngine.doActorOnSpawn(getActor(), (Npc)actor);
                    }
                }
            }

        }


        public void clearInstance()
        {
            actorInstanceList.Clear();
        }

    }
}

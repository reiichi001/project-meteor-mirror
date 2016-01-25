using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.Actors;
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

        public uint eventCurrentOwner = 0;
        public string eventCurrentStarter = "";

        private ClientConnection zoneConnection;
        private ClientConnection chatConnection;

        bool isDisconnected;

        public ConnectedPlayer(uint actorId)
        {
            this.actorID = actorId;
            playerActor = new Player(this, actorId);
            actorInstanceList.Add(playerActor);
        }

        public void setConnection(int type, ClientConnection conn)
        {
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
            isDisconnected = true;
            zoneConnection.disconnect();
            chatConnection.disconnect();
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

        public void sendMotd()
        {
            
        }

        public void sendChat(ConnectedPlayer sender, string message, int mode)
        {

        }

        public List<BasePacket> updateInstance(List<Actor> list)
        {            
            List<BasePacket> basePackets = new List<BasePacket>();
            List<SubPacket> removeActorSubpackets = new List<SubPacket>();
            List<SubPacket> posUpdateSubpackets = new List<SubPacket>();

            //Remove missing actors
            for (int i = 0; i < actorInstanceList.Count; i++)
            {
                if (!list.Contains(actorInstanceList[i]))
                {
                    removeActorSubpackets.Add(RemoveActorPacket.buildPacket(playerActor.actorId, actorInstanceList[i].actorId));
                    actorInstanceList.RemoveAt(i);                    
                }
            }

            if (removeActorSubpackets.Count != 0)
                basePackets.Add(BasePacket.createPacket(removeActorSubpackets, true, false));

            //Add new actors or move
            for (int i = 0; i < list.Count; i++)
            {
                Actor actor = list[i];

                if (actor.actorId == playerActor.actorId)
                    continue;

                if (actorInstanceList.Contains(actor))
                {
                    posUpdateSubpackets.Add(actor.createPositionUpdatePacket(playerActor.actorId));
                }
                else
                {
                    basePackets.Add(actor.getSpawnPackets(playerActor.actorId, 1));
                    basePackets.Add(actor.getInitPackets(playerActor.actorId));
                    basePackets.Add(actor.getSetEventStatusPackets(playerActor.actorId));
                    actorInstanceList.Add(actor);
                }
            }

            if (posUpdateSubpackets.Count > 0)
                basePackets.Add(BasePacket.createPacket(posUpdateSubpackets, true, false));

            return basePackets;
        }

    }
}

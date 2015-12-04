using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects.chara;
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
        PlayerActor playerActor;
        List<Actor> actorInstanceList = new List<Actor>();

        ClientConnection conn1;
        ClientConnection conn2;

        bool isDisconnected;

        public ConnectedPlayer(uint actorId)
        {
            this.actorID = actorId;
            DBCharacter chara = Database.getCharacter(actorId);
            createPlayerActor(actorId, chara);
        }

        public void addConnection(ClientConnection conn)
        {
            if (conn1 == null && conn2 != null)
                conn1 = conn;
            else if (conn2 == null && conn1 != null)
                conn2 = conn;
            else
                conn1 = conn;
        }

        public bool isClientConnectionsReady()
        {
            return (conn1 != null && conn2 != null);
        }

        public void disconnect()
        {
            isDisconnected = true;
            conn1.disconnect();
            conn2.disconnect();
        }

        public void setConnection1(ClientConnection conn)
        {
            conn1 = conn;
        }

        public void setConnection2(ClientConnection conn)
        {
            conn2 = conn;
        }

        public ClientConnection getConnection1()
        {
            return conn1;
        }

        public ClientConnection getConnection2()
        {
            return conn2;
        }

        public Actor getActor()
        {
            return playerActor;
        }

        public void createPlayerActor(uint actorId, DBCharacter chara)
        {
            playerActor = new PlayerActor(actorId);

            playerActor.displayNameID = 0xFFFFFFFF;
            playerActor.customDisplayName = chara.name;
            playerActor.setPlayerAppearance();
            actorInstanceList.Add(playerActor);
        }

        public void updatePlayerActorPosition(float x, float y, float z, float rot, ushort moveState)
        {
            playerActor.positionX = x;
            playerActor.positionY = y;
            playerActor.positionZ = z;
            playerActor.rotation = rot;
            playerActor.moveState = moveState;
        }            

        public void sendMotd()
        {
            DBWorld world = Database.getServer(ConfigConstants.DATABASE_WORLDID);
            //sendChat(world.motd);
        }

        public void sendChat(ConnectedPlayer sender, string message, int mode)
        {

        }


        public List<BasePacket> updateInstance(List<Actor> list)
        {            
            List<BasePacket> basePackets = new List<BasePacket>();
            List<SubPacket> posUpdateSubpackets = new List<SubPacket>();

            for (int i = 0; i < list.Count; i++)
            {
                Actor actor = list[i];

                if (actor.actorID == playerActor.actorID)
                    continue;

                if (actorInstanceList.Contains(actor))
                {
                    posUpdateSubpackets.Add(actor.createPositionUpdatePacket(playerActor.actorID));
                }
                else
                {
                    BasePacket p = actor.createActorSpawnPackets(playerActor.actorID);
                    p.replaceActorID(0x29b27d3, playerActor.actorID);
                    basePackets.Add(p);
                    actorInstanceList.Add(actor);
                }
            }

            if (posUpdateSubpackets.Count > 0)
                basePackets.Add(BasePacket.createPacket(posUpdateSubpackets, true, false));

            return basePackets;
        }
    }
}

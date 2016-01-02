using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class Actor
    {        
        public uint actorId;
        public string actorName;

        public uint displayNameId = 0xFFFFFFFF;
        public string customDisplayName;

        public uint animationId;

        public float positionX, positionY, positionZ, rotation;
        public float oldPositionX, oldPositionY, oldPositionZ, oldRotation;
        public ushort moveState, oldMoveState;

        public uint currentZoneId;

        public string className;
        public List<LuaParam> classParams;

        public Actor(uint Id)
        {
            actorId = Id;
        }

        public SubPacket createNamePacket(uint playerActorId)
        {            
            return SetActorNamePacket.buildPacket(actorId, playerActorId,  displayNameId, displayNameId == 0xFFFFFFFF ? customDisplayName : "");
        }        

        public SubPacket createSpeedPacket(uint playerActorId)
        {
            return SetActorSpeedPacket.buildPacket(actorId, playerActorId);
        }

        public SubPacket createSpawnPositonPacket(uint playerActorId, uint spawnType)
        {
            return SetActorPositionPacket.buildPacket(actorId, playerActorId, SetActorPositionPacket.INNPOS_X, SetActorPositionPacket.INNPOS_Y, SetActorPositionPacket.INNPOS_Z, SetActorPositionPacket.INNPOS_ROT, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            //return SetActorPositionPacket.buildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
        }

        public SubPacket createPositionUpdatePacket(uint playerActorId)
        {
            return MoveActorToPositionPacket.buildPacket(actorId, playerActorId, positionX, positionY, positionZ, rotation, moveState);
        }        

        public SubPacket createScriptBindPacket(uint playerActorId)
        {
            return null;
        }

        public BasePacket createActorSpawnPackets(uint playerActorId)
        {
            if (this is Character)
                return ((Character)this).createActorSpawnPackets(playerActorId);
            else
                return null;
        }
        
        public override bool Equals(Object obj)
        {
            Actor actorObj = obj as Actor;
            if (actorObj == null)
                return false;
            else
                return actorId == actorObj.actorId;
        }

        public string getName()
        {
            return actorName;
        }

        public string getClassName()
        {
            return className;
        }

        public List<LuaParam> getLuaParams()
        {
            return classParams;
        }

    }
}

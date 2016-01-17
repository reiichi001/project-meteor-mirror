using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
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

        public ushort currentMainState = SetActorStatePacket.MAIN_STATE_PASSIVE;
        public ushort currentSubState = SetActorStatePacket.SUB_STATE_NONE;
        public float positionX, positionY, positionZ, rotation;
        public float oldPositionX, oldPositionY, oldPositionZ, oldRotation;
        public ushort moveState, oldMoveState;

        public uint zoneId;
        public Zone zone = null;
        public bool isZoning = false;

        public bool spawnedFirstTime = false;

        public string className;
        public List<LuaParam> classParams;

        public Actor(uint Id)
        {
            actorId = Id;
        }

        public SubPacket createAddActorPacket(uint playerActorId)
        {
            return AddActorPacket.buildPacket(actorId, playerActorId, 0);
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
            SubPacket spawnPacket;
            if (!spawnedFirstTime && playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0, positionX, positionY, positionZ, rotation, spawnType, false);
            else if (playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0xFFFFFFFF, positionX, positionY, positionZ, rotation, spawnType, true);
            else
                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, actorId, positionX, positionY, positionZ, rotation, spawnType, false);

            //return SetActorPositionPacket.buildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            spawnedFirstTime = true;
            return spawnPacket;
        }

        public SubPacket createPositionUpdatePacket(uint playerActorId)
        {
            return MoveActorToPositionPacket.buildPacket(actorId, playerActorId, positionX, positionY, positionZ, rotation, moveState);
        }

        public SubPacket createStatePacket(uint playerActorID)
        {
            return SetActorStatePacket.buildPacket(actorId, playerActorID, currentMainState, currentSubState);
        }

        public SubPacket createIsZoneingPacket(uint playerActorId)
        {
            return SetActorIsZoningPacket.buildPacket(actorId, playerActorId, false);
        }

        public virtual SubPacket createScriptBindPacket(uint playerActorId)
        {
            return null;
        }

        public virtual BasePacket getSpawnPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0x1));            
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            return BasePacket.createPacket(subpackets, true, false);
        }

        public virtual BasePacket getInitPackets(uint playerActorId)
        {
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.addTarget();
            return BasePacket.createPacket(initProperties.buildPacket(playerActorId, actorId), true, false);
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


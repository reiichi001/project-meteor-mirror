using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FFXIVClassic_Map_Server.Actors
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
        public float[] moveSpeeds = new float[5];

        public uint zoneId;
        public Area zone = null;
        public bool isZoning = false;

        public bool spawnedFirstTime = false;

        public string className;
        public List<LuaParam> classParams;

        public EventList eventConditions;

        public Actor(uint actorId)
        {
            this.actorId = actorId;
        }

        public Actor(uint actorId, string actorName, string className, List<LuaParam> classParams)
        {
            this.actorId = actorId;
            this.actorName = actorName;
            this.className = className;
            this.classParams = classParams;

            this.moveSpeeds[0] = SetActorSpeedPacket.DEFAULT_STOP;
            this.moveSpeeds[1] = SetActorSpeedPacket.DEFAULT_WALK;
            this.moveSpeeds[2] = SetActorSpeedPacket.DEFAULT_RUN;
            this.moveSpeeds[3] = SetActorSpeedPacket.DEFAULT_RUN;
        }

        public SubPacket createAddActorPacket(uint playerActorId, byte val)
        {
            return AddActorPacket.buildPacket(actorId, playerActorId, val);
        } 

        public SubPacket createNamePacket(uint playerActorId)
        {
            return SetActorNamePacket.buildPacket(actorId, playerActorId, displayNameId, displayNameId == 0xFFFFFFFF | displayNameId == 0x0 ? customDisplayName : "");
        }        

        public SubPacket createSpeedPacket(uint playerActorId)
        {
            return SetActorSpeedPacket.buildPacket(actorId, playerActorId);
        }

        public SubPacket createSpawnPositonPacket(uint playerActorId, uint spawnType)
        {
            SubPacket spawnPacket;
            if (!spawnedFirstTime && playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0, positionX, positionY, positionZ, rotation, 0x1, false);
            else if (playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0xFFFFFFFF, positionX, positionY, positionZ, rotation, spawnType, true);
            else
            {
                if (this is Player)
                    spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0, positionX, positionY, positionZ, rotation, spawnType, false);
                else
                    spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, actorId, positionX, positionY, positionZ, rotation, spawnType, false);
            }

            //return SetActorPositionPacket.buildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            spawnedFirstTime = true;

            return spawnPacket;
        }

        public SubPacket createSpawnTeleportPacket(uint playerActorId, uint spawnType)
        {
            SubPacket spawnPacket;

                spawnPacket = SetActorPositionPacket.buildPacket(actorId, playerActorId, 0xFFFFFFFF, positionX, positionY, positionZ, rotation, spawnType, false);
       
            //return SetActorPositionPacket.buildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            
            spawnPacket.debugPrintSubPacket();

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

        public List<SubPacket> getEventConditionPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            //Return empty list
            if (eventConditions == null)
                return subpackets;

            if (eventConditions.talkEventConditions != null)
            {
                foreach (EventList.TalkEventCondition condition in eventConditions.talkEventConditions)                
                    subpackets.Add(SetTalkEventCondition.buildPacket(playerActorId, actorId, condition));                
            }

            if (eventConditions.noticeEventConditions != null)
            {
                foreach (EventList.NoticeEventCondition condition in eventConditions.noticeEventConditions)
                    subpackets.Add(SetNoticeEventCondition.buildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.emoteEventConditions != null)
            {
                foreach (EventList.EmoteEventCondition condition in eventConditions.emoteEventConditions)
                    subpackets.Add(SetEmoteEventCondition.buildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithCircleEventConditions != null)
            {
                foreach (EventList.PushCircleEventCondition condition in eventConditions.pushWithCircleEventConditions)
                    subpackets.Add(SetPushEventConditionWithCircle.buildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithFanEventConditions != null)
            {
                foreach (EventList.PushFanEventCondition condition in eventConditions.pushWithFanEventConditions)
                    subpackets.Add(SetPushEventConditionWithFan.buildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithBoxEventConditions != null)
            {
                foreach (EventList.PushBoxEventCondition condition in eventConditions.pushWithBoxEventConditions)
                    subpackets.Add(SetPushEventConditionWithTriggerBox.buildPacket(playerActorId, actorId, condition));
            }

            return subpackets;
        }

        public BasePacket getSetEventStatusPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            //Return empty list
            if (eventConditions == null)
                return BasePacket.createPacket(subpackets, true, false);

            if (eventConditions.talkEventConditions != null)
            {
                foreach (EventList.TalkEventCondition condition in eventConditions.talkEventConditions)                
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 1, condition.conditionName)); 
            }

            if (eventConditions.noticeEventConditions != null)
            {
                foreach (EventList.NoticeEventCondition condition in eventConditions.noticeEventConditions)
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 1, condition.conditionName));
            }

            if (eventConditions.emoteEventConditions != null)
            {
                foreach (EventList.EmoteEventCondition condition in eventConditions.emoteEventConditions)
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 3, condition.conditionName));
            }

            if (eventConditions.pushWithCircleEventConditions != null)
            {
                foreach (EventList.PushCircleEventCondition condition in eventConditions.pushWithCircleEventConditions)
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            if (eventConditions.pushWithFanEventConditions != null)
            {
                foreach (EventList.PushFanEventCondition condition in eventConditions.pushWithFanEventConditions)
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            if (eventConditions.pushWithBoxEventConditions != null)
            {
                foreach (EventList.PushBoxEventCondition condition in eventConditions.pushWithBoxEventConditions)
                    subpackets.Add(SetEventStatus.buildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            return BasePacket.createPacket(subpackets, true, false);
        }

        public SubPacket createIsZoneingPacket(uint playerActorId)
        {
            return SetActorIsZoningPacket.buildPacket(actorId, playerActorId, false);
        }

        public virtual SubPacket createScriptBindPacket(uint playerActorId)
        {
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, classParams);
        }

        public virtual BasePacket getSpawnPackets(uint playerActorId)
        {            
            return getSpawnPackets(playerActorId, 0x1);
        }

        public virtual BasePacket getSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId, 8));
            subpackets.AddRange(getEventConditionPackets(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, spawnType));            
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));
            return BasePacket.createPacket(subpackets, true, false);
        }        

        public virtual BasePacket getInitPackets(uint playerActorId)
        {
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.addByte(0xE14B0CA8, 1);
            initProperties.addByte(0x2138FD71, 1);
            initProperties.addByte(0xFBFBCFB1, 1);
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

        public ushort getState()
        {
            return currentMainState;
        }

        public List<LuaParam> getLuaParams()
        {
            return classParams;
        }

        public void changeState(ushort newState)
        {
            currentMainState = newState;
            SubPacket changeStatePacket = SetActorStatePacket.buildPacket(actorId, actorId, newState, currentSubState);
            SubPacket battleActionPacket = BattleAction1Packet.buildPacket(actorId, actorId);
            zone.broadcastPacketAroundActor(this, changeStatePacket);
            zone.broadcastPacketAroundActor(this, battleActionPacket);
        }

        public void changeSpeed(int type, float value)
        {
            moveSpeeds[type] = value;
            SubPacket changeSpeedPacket = SetActorSpeedPacket.buildPacket(actorId, actorId, moveSpeeds[0], moveSpeeds[1], moveSpeeds[2]);
            zone.broadcastPacketAroundActor(this, changeSpeedPacket);
        }

        public void changeSpeed(float speedStop, float speedWalk, float speedRun)
        {
            moveSpeeds[0] = speedStop;
            moveSpeeds[1] = speedWalk;
            moveSpeeds[2] = speedRun;
            moveSpeeds[3] = speedRun;
            SubPacket changeSpeedPacket = SetActorSpeedPacket.buildPacket(actorId, actorId, moveSpeeds[0], moveSpeeds[1], moveSpeeds[2]);
            zone.broadcastPacketAroundActor(this, changeSpeedPacket);
        }

    }
}


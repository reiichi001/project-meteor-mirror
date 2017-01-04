
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.events;
using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using FFXIVClassic_Map_Server.actors.area;

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
        public float[] moveSpeeds = new float[4];

        public uint zoneId, zoneId2;
        public Area zone = null;
        public Area zone2 = null;
        public bool isZoning = false;

        public bool spawnedFirstTime = false;

        public string classPath;
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
            this.moveSpeeds[3] = SetActorSpeedPacket.DEFAULT_ACTIVE;
        }

        public SubPacket CreateAddActorPacket(uint playerActorId, byte val)
        {
            return AddActorPacket.BuildPacket(actorId, playerActorId, val);
        }

        public SubPacket CreateNamePacket(uint playerActorId)
        {
            return SetActorNamePacket.BuildPacket(actorId, playerActorId, displayNameId, displayNameId == 0xFFFFFFFF | displayNameId == 0x0 ? customDisplayName : "");
        }

        public SubPacket CreateSpeedPacket(uint playerActorId)
        {
            return SetActorSpeedPacket.BuildPacket(actorId, playerActorId);
        }

        public SubPacket CreateSpawnPositonPacket(uint playerActorId, ushort spawnType)
        {
            SubPacket spawnPacket;
            if (!spawnedFirstTime && playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.BuildPacket(actorId, playerActorId, 0, positionX, positionY, positionZ, rotation, 0x1, false);
            else if (playerActorId == actorId)
                spawnPacket = SetActorPositionPacket.BuildPacket(actorId, playerActorId, 0xFFFFFFFF, positionX, positionY, positionZ, rotation, spawnType, true);
            else
            {
                if (this is Player)
                    spawnPacket = SetActorPositionPacket.BuildPacket(actorId, playerActorId, 0, positionX, positionY, positionZ, rotation, spawnType, false);
                else
                    spawnPacket = SetActorPositionPacket.BuildPacket(actorId, playerActorId, actorId, positionX, positionY, positionZ, rotation, spawnType, false);
            }

            //return SetActorPositionPacket.BuildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            spawnedFirstTime = true;

            return spawnPacket;
        }

        public SubPacket CreateSpawnTeleportPacket(uint playerActorId, ushort spawnType)
        {
            SubPacket spawnPacket;

            spawnPacket = SetActorPositionPacket.BuildPacket(actorId, playerActorId, 0xFFFFFFFF, positionX, positionY, positionZ, rotation, spawnType, false);

            //return SetActorPositionPacket.BuildPacket(actorId, playerActorId, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);

            //spawnPacket.DebugPrintSubPacket();

            return spawnPacket;
        }

        public SubPacket CreatePositionUpdatePacket(uint playerActorId)
        {
            return MoveActorToPositionPacket.BuildPacket(actorId, playerActorId, positionX, positionY, positionZ, rotation, moveState);
        }

        public SubPacket CreateStatePacket(uint playerActorID)
        {
            return SetActorStatePacket.BuildPacket(actorId, playerActorID, currentMainState, currentSubState);
        }

        public List<SubPacket> GetEventConditionPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            //Return empty list
            if (eventConditions == null)
                return subpackets;

            if (eventConditions.talkEventConditions != null)
            {
                foreach (EventList.TalkEventCondition condition in eventConditions.talkEventConditions)
                    subpackets.Add(SetTalkEventCondition.BuildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.noticeEventConditions != null)
            {
                foreach (EventList.NoticeEventCondition condition in eventConditions.noticeEventConditions)
                    subpackets.Add(SetNoticeEventCondition.BuildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.emoteEventConditions != null)
            {
                foreach (EventList.EmoteEventCondition condition in eventConditions.emoteEventConditions)
                    subpackets.Add(SetEmoteEventCondition.BuildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithCircleEventConditions != null)
            {
                foreach (EventList.PushCircleEventCondition condition in eventConditions.pushWithCircleEventConditions)
                    subpackets.Add(SetPushEventConditionWithCircle.BuildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithFanEventConditions != null)
            {
                foreach (EventList.PushFanEventCondition condition in eventConditions.pushWithFanEventConditions)
                    subpackets.Add(SetPushEventConditionWithFan.BuildPacket(playerActorId, actorId, condition));
            }

            if (eventConditions.pushWithBoxEventConditions != null)
            {
                foreach (EventList.PushBoxEventCondition condition in eventConditions.pushWithBoxEventConditions)
                    subpackets.Add(SetPushEventConditionWithTriggerBox.BuildPacket(playerActorId, actorId, condition));
            }

            return subpackets;
        }

        public BasePacket GetSetEventStatusPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            //Return empty list
            if (eventConditions == null)
                return BasePacket.CreatePacket(subpackets, true, false);

            if (eventConditions.talkEventConditions != null)
            {
                foreach (EventList.TalkEventCondition condition in eventConditions.talkEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 1, condition.conditionName));
            }

            if (eventConditions.noticeEventConditions != null)
            {
                foreach (EventList.NoticeEventCondition condition in eventConditions.noticeEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 1, condition.conditionName));
            }

            if (eventConditions.emoteEventConditions != null)
            {
                foreach (EventList.EmoteEventCondition condition in eventConditions.emoteEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 3, condition.conditionName));
            }

            if (eventConditions.pushWithCircleEventConditions != null)
            {
                foreach (EventList.PushCircleEventCondition condition in eventConditions.pushWithCircleEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            if (eventConditions.pushWithFanEventConditions != null)
            {
                foreach (EventList.PushFanEventCondition condition in eventConditions.pushWithFanEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            if (eventConditions.pushWithBoxEventConditions != null)
            {
                foreach (EventList.PushBoxEventCondition condition in eventConditions.pushWithBoxEventConditions)
                    subpackets.Add(SetEventStatus.BuildPacket(playerActorId, actorId, true, 2, condition.conditionName));
            }

            return BasePacket.CreatePacket(subpackets, true, false);
        }

        public SubPacket CreateIsZoneingPacket(uint playerActorId)
        {
            return SetActorIsZoningPacket.BuildPacket(actorId, playerActorId, false);
        }

        public virtual SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, classParams);
        }

        public virtual BasePacket GetSpawnPackets(uint playerActorId)
        {
            return GetSpawnPackets(playerActorId, 0x1);
        }

        public virtual BasePacket GetSpawnPackets(uint playerActorId, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(playerActorId, 8));
            subpackets.AddRange(GetEventConditionPackets(playerActorId));
            subpackets.Add(CreateSpeedPacket(playerActorId));
            subpackets.Add(CreateSpawnPositonPacket(playerActorId, spawnType));
            subpackets.Add(CreateNamePacket(playerActorId));
            subpackets.Add(CreateStatePacket(playerActorId));
            subpackets.Add(CreateIsZoneingPacket(playerActorId));
            subpackets.Add(CreateScriptBindPacket(playerActorId));
            return BasePacket.CreatePacket(subpackets, true, false);
        }

        public virtual BasePacket GetInitPackets(uint playerActorId)
        {
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.AddByte(0xE14B0CA8, 1);
            initProperties.AddByte(0x2138FD71, 1);
            initProperties.AddByte(0xFBFBCFB1, 1);
            initProperties.AddTarget();
            return BasePacket.CreatePacket(initProperties.BuildPacket(playerActorId, actorId), true, false);
        }

        public override bool Equals(Object obj)
        {
            Actor actorObj = obj as Actor;
            if (actorObj == null)
                return false;
            else
                return actorId == actorObj.actorId;
        }

        public string GetName()
        {
            return actorName;
        }

        public string GetClassName()
        {
            return className;
        }

        public ushort GetState()
        {
            return currentMainState;
        }

        public List<LuaParam> GetLuaParams()
        {
            return classParams;
        }

        public void ChangeState(ushort newState)
        {
            currentMainState = newState;
            SubPacket ChangeStatePacket = SetActorStatePacket.BuildPacket(actorId, actorId, newState, currentSubState);
            SubPacket battleActionPacket = BattleAction1Packet.BuildPacket(actorId, actorId);
            zone.BroadcastPacketAroundActor(this, ChangeStatePacket);
            zone.BroadcastPacketAroundActor(this, battleActionPacket);
        }

        public void ChangeSpeed(int type, float value)
        {
            moveSpeeds[type] = value;
            SubPacket ChangeSpeedPacket = SetActorSpeedPacket.BuildPacket(actorId, actorId, moveSpeeds[0], moveSpeeds[1], moveSpeeds[2], moveSpeeds[3]);
            zone.BroadcastPacketAroundActor(this, ChangeSpeedPacket);
        }

        public void ChangeSpeed(float speedStop, float speedWalk, float speedRun, float speedActive)
        {
            moveSpeeds[0] = speedStop;
            moveSpeeds[1] = speedWalk;
            moveSpeeds[2] = speedRun;
            moveSpeeds[3] = speedActive;
            SubPacket ChangeSpeedPacket = SetActorSpeedPacket.BuildPacket(actorId, actorId, moveSpeeds[0], moveSpeeds[1], moveSpeeds[2], moveSpeeds[3]);
            zone.BroadcastPacketAroundActor(this, ChangeSpeedPacket);
        }

        public void GenerateActorName(int actorNumber)
        {
            //Format Class Name
            string className = this.className.Replace("Populace", "Ppl")
                                             .Replace("Monster", "Mon")
                                             .Replace("Crowd", "Crd")
                                             .Replace("MapObj", "Map")
                                             .Replace("Object", "Obj")
                                             .Replace("Retainer", "Rtn")
                                             .Replace("Standard", "Std");
            className = Char.ToLowerInvariant(className[0]) + className.Substring(1);

            //Format Zone Name
            string zoneName = zone.zoneName.Replace("Field", "Fld")
                                           .Replace("Dungeon", "Dgn")
                                           .Replace("Town", "Twn")
                                           .Replace("Battle", "Btl")
                                           .Replace("Test", "Tes")
                                           .Replace("Event", "Evt")
                                           .Replace("Ship", "Shp")
                                           .Replace("Office", "Ofc");
            if (zone is PrivateArea)
            {
                //Check if "normal"
                zoneName = zoneName.Remove(zoneName.Length - 1, 1) + "P";
            }
            zoneName = Char.ToLowerInvariant(zoneName[0]) + zoneName.Substring(1);

            try
            {
                className = className.Substring(0, 20 - zoneName.Length);
            }
            catch (ArgumentOutOfRangeException e)
            { }

            //Convert actor number to base 63
            string classNumber = Utils.ToStringBase63(actorNumber);

            //Get stuff after @
            uint zoneId = zone.actorId;
            uint privLevel = 0;
            if (zone is PrivateArea)
                privLevel = ((PrivateArea)zone).GetPrivateAreaLevel();

            actorName = String.Format("{0}_{1}_{2}@{3:X3}{4:X2}", className, zoneName, classNumber, zoneId, privLevel);
        }

        public List<float> GetPos()
        {
            List<float> pos = new List<float>();

            pos.Add(positionX);
            pos.Add(positionY);
            pos.Add(positionZ);
            pos.Add(rotation);
            pos.Add(zoneId);

            return pos;
        }

        public void SetPos(float x, float y, float z, float rot = 0, uint zoneId = 0)
        {
            oldPositionX = positionX;
            oldPositionY = positionY;
            oldPositionZ = positionZ;
            oldRotation = rotation;

            positionX = x;
            positionY = y;
            positionZ = z;
            rotation = rot;

            // todo: handle zone?
            zone.BroadcastPacketAroundActor(this, MoveActorToPositionPacket.BuildPacket(this.actorId, this.actorId, x, y, z, rot, moveState));
        }

        public Area GetZone()
        {
            return zone;
        }

        public uint GetZoneID()
        {
            return zoneId;
        }
    }
}


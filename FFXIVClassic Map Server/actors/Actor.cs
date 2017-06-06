
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.events;
using FFXIVClassic.Common;
using System;
using System.Collections.Generic;
using FFXIVClassic_Map_Server.actors.area;
using System.Reflection;
using System.ComponentModel;

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
        public string privateArea;
        public uint privateAreaType;
        public Area zone = null;
        public Area zone2 = null;
        public bool isZoning = false;

        public bool spawnedFirstTime = false;

        public string classPath;
        public string className;
        public List<LuaParam> classParams;

        public List<utils.Vector3> positionUpdates = new List<utils.Vector3>();
        public DateTime lastAiUpdate;
        public DateTime lastMoveUpdate;
        public Actor target;

        public bool hasMoved = false;
        public bool isAtSpawn = true;
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

        public void SetPushCircleRange(string triggerName, float size)
        {
            if (eventConditions == null || eventConditions.pushWithCircleEventConditions == null)
                return;

            foreach (EventList.PushCircleEventCondition condition in eventConditions.pushWithCircleEventConditions)
            {
                if (condition.conditionName.Equals(triggerName))
                {
                    condition.radius = size;
                    break;
                }
            }
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
            return SetActorSpeedPacket.BuildPacket(actorId, playerActorId, moveSpeeds[0], moveSpeeds[1], moveSpeeds[2], moveSpeeds[3]);
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
            int updateMs = 300;
            var diffTime = (DateTime.Now - lastMoveUpdate);

            if (this.target != null)
            {
                updateMs = 150;
            }
            if (diffTime.Milliseconds >= updateMs && hasMoved)
            {
                hasMoved = (this.positionUpdates != null && this.positionUpdates.Count > 0);
                if (hasMoved)
                {
                    var pos = positionUpdates[0];
                    positionUpdates.Remove(pos);

                    positionX = pos.X;
                    positionY = pos.Y;
                    positionZ = pos.Z;

                    if (target != null)
                        LookAt(target);

                    //Program.Server.GetInstance().mLuaEngine.OnPath(actor, position, positionUpdates)
                }
                lastMoveUpdate = DateTime.Now;
                return MoveActorToPositionPacket.BuildPacket(actorId, playerActorId, positionX, positionY, positionZ, rotation, moveState);
            }
            return null;
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

        public void Update(double deltaTime)
        {
            // todo: this is retarded
            if (this is Zone || this is Area || this is Player)
                return;

            var diffTime = (DateTime.Now - lastAiUpdate);

            // todo: this too
            if (diffTime.Milliseconds >= deltaTime)
            {
                bool foundActor = false;
                foreach (var actor in ((Area)zone).GetActorsAroundActor(this, 50))
                {
                    if (actor is Player && actor != this)
                    {
                        var player = actor as Player;

                        var distance = Utils.Distance(positionX, positionY, positionZ, player.positionX, player.positionY, player.positionZ);

                        int maxDistance = player == target ? 25 : 15;

                        if (distance <= maxDistance)
                        {
                            foundActor = true;

                            if (!hasMoved)
                            {
                                if (distance >= 3)
                                {
                                    FollowTarget(player, 2.0f);
                                }
                                // too close, spread out
                                else if (distance <= 0.64f)
                                {
                                    var minRadius = 0.65f;
                                    var maxRadius = 0.85f;

                                    var angle = Program.Random.NextDouble() * Math.PI * 2;
                                    var radius = Math.Sqrt(Program.Random.NextDouble() * (maxRadius - minRadius)) + minRadius;

                                    float x = (float)(radius * Math.Cos(angle));
                                    float z = (float)(radius * Math.Sin(angle));

                                    positionUpdates.Add(new utils.Vector3(positionX + x, positionY, positionZ + z));

                                    hasMoved = true;
                                }

                                if (target != null)
                                {
                                    LookAt(target);
                                }
                            }
                        }
                        break;
                    }
                }
                // 5 seconds before back to spawn
                if ((DateTime.Now - lastMoveUpdate).Seconds >= 5 && !foundActor)
                {
                    // 10 yalms spawn radius just because
                    this.isAtSpawn = Utils.Distance(positionX, positionY, positionZ, oldPositionX, oldPositionY, oldPositionZ) <= 18.0f;

                    if (this.isAtSpawn != true && this.target == null && oldPositionX != 0.0f && oldPositionY != 0.0f && oldPositionZ != 0.0f)
                        PathTo(oldPositionX, oldPositionY, oldPositionZ, 3.0f);

                    lastMoveUpdate = DateTime.Now;
                    this.target = null;
                }
                lastAiUpdate = DateTime.Now;
            }
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
                privLevel = ((PrivateArea)zone).GetPrivateAreaType();

            actorName = String.Format("{0}_{1}_{2}@{3:X3}{4:X2}", className, zoneName, classNumber, zoneId, privLevel);
        }

        public bool SetWorkValue(Player player, string name, string uiFunc, object value)
        {
            string[] split = name.Split('.');
            int arrayIndex = 0;

            if (!(split[0].Equals("work") || split[0].Equals("charaWork") || split[0].Equals("playerWork") || split[0].Equals("npcWork")))
                return false;

            Object parentObj = null;
            Object curObj = this;
            for (int i = 0; i < split.Length; i++)
            {
                //For arrays
                if (split[i].Contains("["))
                {
                    if (split[i].LastIndexOf(']') - split[i].IndexOf('[') <= 0)
                        return false;

                    arrayIndex = Convert.ToInt32(split[i].Substring(split[i].IndexOf('[') + 1, split[i].LastIndexOf(']') - split[i].LastIndexOf('[') - 1));
                    split[i] = split[i].Substring(0, split[i].IndexOf('['));
                }

                FieldInfo field = curObj.GetType().GetField(split[i]);
                if (field == null)
                    return false;

                if (i == split.Length - 1)
                    parentObj = curObj;
                curObj = field.GetValue(curObj);
                if (curObj == null)
                    return false;
            }

            if (curObj == null)
                return false;
            else
            {
                //Array, we actually care whats inside
                if (curObj.GetType().IsArray)
                {
                    if (((Array)curObj).Length <= arrayIndex)
                        return false;

                    if (value.GetType() == ((Array)curObj).GetType().GetElementType() || TypeDescriptor.GetConverter(value.GetType()).CanConvertTo(((Array)curObj).GetType().GetElementType()))
                    {
                        if (value.GetType() == ((Array)curObj).GetType().GetElementType())
                            ((Array)curObj).SetValue(value, arrayIndex);
                        else
                            ((Array)curObj).SetValue(TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, curObj.GetType().GetElementType()), arrayIndex);

                        SetActorPropetyPacket changeProperty = new SetActorPropetyPacket(uiFunc);
                        changeProperty.AddProperty(this, name);
                        changeProperty.AddTarget();
                        SubPacket subpacket = changeProperty.BuildPacket(player.actorId, player.actorId);
                        player.playerSession.QueuePacket(subpacket, true, false);
                        subpacket.DebugPrintSubPacket();
                        return true;
                    }
                }
                else
                {
                    if (value.GetType() == curObj.GetType() || TypeDescriptor.GetConverter(value.GetType()).CanConvertTo(curObj.GetType()))
                    {
                        if (value.GetType() == curObj.GetType())
                            parentObj.GetType().GetField(split[split.Length - 1]).SetValue(parentObj, value);
                        else
                            parentObj.GetType().GetField(split[split.Length-1]).SetValue(parentObj, TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, curObj.GetType()));

                        SetActorPropetyPacket changeProperty = new SetActorPropetyPacket(uiFunc);
                        changeProperty.AddProperty(this, name);
                        changeProperty.AddTarget();
                        SubPacket subpacket = changeProperty.BuildPacket(player.actorId, player.actorId);
                        player.playerSession.QueuePacket(subpacket, true, false);
                        subpacket.DebugPrintSubPacket();
                        return true;
                    }
                }
                return false;
            }
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

        public bool IsFacing(float x, float y, byte checkAngle = 45)
        {
            var rot1 = this.rotation;

            var dX = this.positionX - x;
            var dY = this.positionY - y;

            var rot2 = Math.Atan2(dY, dX);

            var dRot = Math.PI - rot2 + Math.PI / 2;

            return rot1 == (float)dRot;
        }

        public void LookAt(Actor actor)
        {
            if (actor != null)
            {
                LookAt(actor.positionX, actor.positionY);
            }
            else
            {
                Program.Log.Error("Actor.LookAt() unable to find actor!");
            }
        }

        public void LookAt(float x, float y)
        {
            var rot1 = this.rotation;

            var dX = this.positionX - x;
            var dY = this.positionY - y;

            var rot2 = Math.Atan2(dY, dX);

            var dRot = Math.PI - rot2 + Math.PI / 2;

            // pending move, dont need to unset it
            if (!hasMoved)
                hasMoved = rot1 != (float)dRot;

            rotation = (float)dRot;
        }

        public void PathTo(float x, float y, float z, float stepSize = 0.70f, int maxPath = 40)
        {
            var pos = new utils.Vector3(positionX, positionY, positionZ);
            var dest = new utils.Vector3(x, y, z);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var path = utils.NavmeshUtils.GetPath(((Zone)GetZone()), pos, dest, stepSize, maxPath);

            if (path != null)
            {
                if (oldPositionX == 0.0f && oldPositionY == 0.0f && oldPositionZ == 0.0f)
                {
                    oldPositionX = positionX;
                    oldPositionY = positionY;
                    oldPositionZ = positionZ;
                }

                // todo: something went wrong
                if (path.Count == 0)
                {
                    positionX = oldPositionX;
                    positionY = oldPositionY;
                    positionZ = oldPositionZ;
                }

                positionUpdates = path;

                this.hasMoved = true;
                this.isAtSpawn = false;

                sw.Stop();
                ((Zone)zone).pathCalls++;
                ((Zone)zone).pathCallTime += sw.ElapsedMilliseconds;
                Program.Log.Error("[{0}][{1}] Created {2} points in {3} milliseconds", actorId, actorName, path.Count, sw.ElapsedMilliseconds);
            }   
        }

        public void FollowTarget(Actor target, float stepSize = 1.2f, int maxPath = 25)
        {
            if (target != null)
            {
                var player = target as Player;

                if (this.target != target)
                    this.target = target;

                this.moveState = player.moveState;
                this.moveSpeeds = player.moveSpeeds;

                PathTo(player.positionX, player.positionY, player.positionZ, stepSize, maxPath);
            }
        }

        public void OnPath()
        {
            // todo: lua function onPath in mob script
        }
    }
}


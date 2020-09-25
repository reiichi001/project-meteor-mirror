/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.Common;
using Meteor.Map.actors;
using Meteor.Map.actors.chara.npc;
using Meteor.Map.Actors.Chara;
using Meteor.Map.lua;
using Meteor.Map.packets.send.actor;
using Meteor.Map.utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Meteor.Map.actors.chara.ai;

namespace Meteor.Map.Actors
{
    [Flags]
    enum NpcSpawnType : ushort
    {
        Normal    = 0x00,
        Scripted  = 0x01,
        Nighttime = 0x02,
        Evening   = 0x04,
        Daytime   = 0x08,
        Weather   = 0x10,
    }

    class Npc : Character
    {
        private uint actorClassId;
        private string uniqueIdentifier;

        private bool isMapObj = false;
        private uint layout, instance;

        public NpcWork npcWork = new NpcWork();
        public NpcSpawnType npcSpawnType;

        public Npc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot, ushort actorState, uint animationId, string customDisplayName)
            : base((4 << 28 | spawnedArea.actorId << 19 | (uint)actorNumber))  
        {
            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;
            this.rotation = rot;
            this.currentMainState = actorState;
            this.animationId = animationId;

            this.displayNameId = actorClass.displayNameId;
            this.customDisplayName = customDisplayName;

            this.uniqueIdentifier = uniqueId;

            this.zoneId = spawnedArea.actorId;
            this.zone = spawnedArea;

            this.actorClassId = actorClass.actorClassId;

            this.currentSubState.motionPack = (ushort) animationId;

            LoadNpcAppearance(actorClass.actorClassId);

            className = actorClass.classPath.Substring(actorClass.classPath.LastIndexOf("/") + 1);
            this.classPath = String.Format("{0}/{1}", actorClass.classPath.Substring(0, actorClass.classPath.LastIndexOf('/')).ToLower(), className);

            charaWork.battleSave.potencial = 1.0f;

            // todo: these really need to be read from db etc
            {
                charaWork.parameterSave.state_mainSkill[0] = 3;
                charaWork.parameterSave.state_mainSkill[2] = 3;
                charaWork.parameterSave.state_mainSkillLevel = 1;

                charaWork.parameterSave.hp[0] = 80;
                charaWork.parameterSave.hpMax[0] = 80;
            }
            for (int i = 0; i < 32; i++ )
                charaWork.property[i] = (byte)(((int)actorClass.propertyFlags >> i) & 1);

            npcWork.pushCommand = actorClass.pushCommand;
            npcWork.pushCommandSub = actorClass.pushCommandSub;
            npcWork.pushCommandPriority = actorClass.pushCommandPriority;

            if (actorClassId == 1080078 || actorClassId == 1080079 || actorClassId == 1080080 || (actorClassId >= 1080123 && actorClassId <= 1080135) || (actorClassId >= 5000001 && actorClassId <= 5000090) || (actorClassId >= 5900001 && actorClassId <= 5900038))
            {
                isMapObj = true;
                List<LuaParam> lParams = LuaEngine.GetInstance().CallLuaFunctionForReturn(null, this, "init", false);
                if (lParams == null || lParams.Count < 6)
                    isMapObj = false;
                else
                {                   
                    layout = (uint)(Int32)lParams[4].value;
                    instance = (uint)(Int32)lParams[5].value;
                    isStatic = true;
                }
            }
            GenerateActorName((int)actorNumber);
            this.aiContainer = new AIContainer(this, null, new PathFind(this), new TargetFind(this));
        }

        public Npc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot, uint layout, uint instance)
            : base((4 << 28 | spawnedArea.actorId << 19 | (uint)actorNumber))
        {
            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;
            this.rotation = rot;
            this.currentMainState = 0;
            this.animationId = 0;

            this.displayNameId = actorClass.displayNameId;

            this.uniqueIdentifier = uniqueId;

            this.zoneId = spawnedArea.actorId;
            this.zone = spawnedArea;

            this.actorClassId = actorClass.actorClassId;

            LoadNpcAppearance(actorClass.actorClassId);

            this.classPath = actorClass.classPath;
            className = classPath.Substring(classPath.LastIndexOf("/") + 1);

            for (int i = 0; i < 32; i++)
                charaWork.property[i] = (byte)(((int)actorClass.propertyFlags >> i) & 1);

            npcWork.pushCommand = actorClass.pushCommand;
            npcWork.pushCommandSub = actorClass.pushCommandSub;
            npcWork.pushCommandPriority = actorClass.pushCommandPriority;

            this.isMapObj = true;
            this.layout = layout;
            this.instance = instance;

            GenerateActorName((int)actorNumber);
            this.aiContainer = new AIContainer(this, null, new PathFind(this), new TargetFind(null));
        }

        public SubPacket CreateAddActorPacket()
        {
            return AddActorPacket.BuildPacket(actorId, 8);
        }

        // actorClassId, [], [], numBattleCommon, [battleCommon], numEventCommon, [eventCommon], args for either initForBattle/initForEvent
        public override SubPacket CreateScriptBindPacket(Player player)
        {
            List<LuaParam> lParams;
            
            lParams = LuaEngine.GetInstance().CallLuaFunctionForReturn(player, this, "init", false);

            if (lParams != null && lParams.Count >= 3 && lParams[2].typeID == 0 && (int)lParams[2].value == 0)
                isStatic = true;
            else
            {
                //charaWork.property[2] = 1;
                //npcWork.hateType = 1;
            }

            if (lParams == null)
            {
                string classPathFake = "/Chara/Npc/Populace/PopulaceStandard";
                string classNameFake = "PopulaceStandard";
                lParams = LuaUtils.CreateLuaParamList(classPathFake, false, false, false, false, false, 0xF47F6, false, false, 0, 0);
                isStatic = true;
                //ActorInstantiatePacket.BuildPacket(actorId, actorName, classNameFake, lParams).DebugPrintSubPacket();
                return ActorInstantiatePacket.BuildPacket(actorId, actorName, classNameFake, lParams);
            }
            else
            {
                lParams.Insert(0, new LuaParam(2, classPath));
                lParams.Insert(1, new LuaParam(4, 4));
                lParams.Insert(2, new LuaParam(4, 4));
                lParams.Insert(3, new LuaParam(4, 4));
                lParams.Insert(4, new LuaParam(4, 4));
                lParams.Insert(5, new LuaParam(4, 4));
                lParams.Insert(6, new LuaParam(0, (int)actorClassId));
            }

            //ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams);
        }

        public override List<SubPacket> GetSpawnPackets(Player player, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket());
            subpackets.AddRange(GetEventConditionPackets());
            subpackets.Add(CreateSpeedPacket());            
            subpackets.Add(CreateSpawnPositonPacket(0x0));

            if (isMapObj)
                subpackets.Add(SetActorBGPropertiesPacket.BuildPacket(actorId, instance, layout));
            else
                subpackets.Add(CreateAppearancePacket());

            subpackets.Add(CreateNamePacket());
            subpackets.Add(CreateStatePacket());
            subpackets.Add(CreateSubStatePacket());
            subpackets.Add(CreateInitStatusPacket());
            subpackets.Add(CreateSetActorIconPacket());
            subpackets.Add(CreateIsZoneingPacket());           
            subpackets.Add(CreateScriptBindPacket(player));            

            return subpackets;
        }

        public override List<SubPacket> GetInitPackets()
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("/_init", this);

            //Potential
            propPacketUtil.AddProperty("charaWork.battleSave.potencial");

            //Properties
            for (int i = 0; i < charaWork.property.Length; i++)
            {
                if (charaWork.property[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.property[{0}]", i));
            }

            //Parameters
            propPacketUtil.AddProperty("charaWork.parameterSave.hp[0]");
            propPacketUtil.AddProperty("charaWork.parameterSave.hpMax[0]");
            propPacketUtil.AddProperty("charaWork.parameterSave.mp");
            propPacketUtil.AddProperty("charaWork.parameterSave.mpMax");
            propPacketUtil.AddProperty("charaWork.parameterTemp.tp");

            if (charaWork.parameterSave.state_mainSkill[0] != 0)
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
            if (charaWork.parameterSave.state_mainSkill[1] != 0)
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[1]");
            if (charaWork.parameterSave.state_mainSkill[2] != 0)
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[2]");
            if (charaWork.parameterSave.state_mainSkill[3] != 0)
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[3]");

            propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkillLevel");

            //Status Times
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
            {
                if (charaWork.statusShownTime[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.statusShownTime[{0}]", i));
            }

            //General Parameters
            for (int i = 3; i < charaWork.battleTemp.generalParameter.Length; i++)
            {
                if (charaWork.battleTemp.generalParameter[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.generalParameter[{0}]", i));
            }

            propPacketUtil.AddProperty("npcWork.hateType");

            if (npcWork.pushCommand != 0)
            {
                propPacketUtil.AddProperty("npcWork.pushCommand");
                if (npcWork.pushCommandSub != 0)
                    propPacketUtil.AddProperty("npcWork.pushCommandSub");
                propPacketUtil.AddProperty("npcWork.pushCommandPriority");
            }

            return propPacketUtil.Done();
        }

        public string GetUniqueId()
        {
            return uniqueIdentifier;
        }

        public uint GetActorClassId()
        {
            return actorClassId;
        }
        
        public void ChangeNpcAppearance(uint id)
        {
            LoadNpcAppearance(id);
            zone.BroadcastPacketAroundActor(this, CreateAppearancePacket());
        }

        public void LoadNpcAppearance(uint id)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT                 
                                    base,
                                    size,
                                    hairStyle,
                                    hairHighlightColor,
                                    hairVariation,
                                    faceType,   
                                    characteristics,
                                    characteristicsColor,
                                    faceEyebrows,
                                    faceIrisSize,
                                    faceEyeShape,
                                    faceNose,
                                    faceFeatures,
                                    faceMouth,
                                    ears,
                                    hairColor,
                                    skinColor,
                                    eyeColor,
                                    voice,
                                    mainHand,
                                    offHand,
                                    spMainHand,
                                    spOffHand,
                                    throwing,
                                    pack,
                                    pouch,
                                    head,
                                    body,
                                    legs,
                                    hands,
                                    feet,
                                    waist,
                                    neck,
                                    leftEar,
                                    rightEar,
                                    leftIndex,
                                    rightIndex,
                                    leftFinger,
                                    rightFinger
                                    FROM gamedata_actor_appearance
                                    WHERE id = @templateId
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@templateId", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {                            
                            //Handle Appearance
                            modelId = reader.GetUInt32("base");
                            appearanceIds[SIZE] = reader.GetUInt32("size");
                            appearanceIds[COLORINFO] = (uint)(reader.GetUInt32("skinColor") | (reader.GetUInt32("hairColor") << 10) | (reader.GetUInt32("eyeColor") << 20));
                            appearanceIds[FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.GetFaceInfo(
                                reader.GetByte("characteristics"),
                                reader.GetByte("characteristicsColor"),
                                reader.GetByte("faceType"),
                                reader.GetByte("ears"),
                                reader.GetByte("faceMouth"),
                                reader.GetByte("faceFeatures"),
                                reader.GetByte("faceNose"),
                                reader.GetByte("faceEyeShape"),
                                reader.GetByte("faceIrisSize"),
                                reader.GetByte("faceEyebrows")));
                            appearanceIds[HIGHLIGHT_HAIR] = (uint)(reader.GetUInt32("hairHighlightColor") | reader.GetUInt32("hairVariation") << 5 | reader.GetUInt32("hairStyle") << 10);
                            appearanceIds[VOICE] = reader.GetUInt32("voice");
                            appearanceIds[MAINHAND] = reader.GetUInt32("mainHand");
                            appearanceIds[OFFHAND] = reader.GetUInt32("offHand");
                            appearanceIds[SPMAINHAND] = reader.GetUInt32("spMainHand");
                            appearanceIds[SPOFFHAND] = reader.GetUInt32("spOffHand");
                            appearanceIds[THROWING] = reader.GetUInt32("throwing");
                            appearanceIds[PACK] = reader.GetUInt32("pack");
                            appearanceIds[POUCH] = reader.GetUInt32("pouch");
                            appearanceIds[MAINHAND] = reader.GetUInt32("mainHand");
                            appearanceIds[OFFHAND] = reader.GetUInt32("offHand");
                            appearanceIds[HEADGEAR] = reader.GetUInt32("head");
                            appearanceIds[BODYGEAR] = reader.GetUInt32("body");
                            appearanceIds[LEGSGEAR] = reader.GetUInt32("legs");
                            appearanceIds[HANDSGEAR] = reader.GetUInt32("hands");
                            appearanceIds[FEETGEAR] = reader.GetUInt32("feet");
                            appearanceIds[WAISTGEAR] = reader.GetUInt32("waist");
                            appearanceIds[NECKGEAR] = reader.GetUInt32("neck");
                            appearanceIds[R_EAR] = reader.GetUInt32("rightEar");
                            appearanceIds[L_EAR] = reader.GetUInt32("leftEar");
                            appearanceIds[R_INDEXFINGER] = reader.GetUInt32("leftIndex");
                            appearanceIds[L_INDEXFINGER] = reader.GetUInt32("rightIndex");
                            appearanceIds[R_RINGFINGER] = reader.GetUInt32("rightFinger");
                            appearanceIds[L_RINGFINGER] = reader.GetUInt32("leftFinger");
                        }
                    }

                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public void LoadEventConditions(string eventConditions)
        {
            EventList conditions = JsonConvert.DeserializeObject<EventList>(eventConditions);
            this.eventConditions = conditions;
        }

        public void DoOnActorSpawn(Player player)
        {
            LuaEngine.GetInstance().CallLuaFunction(player, this, "onSpawn", true);           
        }

        public void PlayMapObjAnimation(Player player, string animationName)
        {
            player.QueuePacket(PlayBGAnimation.BuildPacket(actorId, animationName));
        }

        public void Despawn()
        {
            zone.DespawnActor(this);
        }

        public override void Update(DateTime tick)
        {
            // todo: can normal npcs have status effects?
            aiContainer.Update(tick);
        }

        public override void PostUpdate(DateTime tick, List<SubPacket> packets = null)
        {
            packets = packets ?? new List<SubPacket>();

            if ((updateFlags & ActorUpdateFlags.Work) != 0)
            {

            }
            base.PostUpdate(tick, packets);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }

        public override void OnDespawn()
        {
            zone.BroadcastPacketAroundActor(this, RemoveActorPacket.BuildPacket(this.actorId));
            QueuePositionUpdate(spawnX, spawnY, spawnZ);
            LuaEngine.CallLuaBattleFunction(this, "onDespawn", this);
        }
        //A party member list packet came, set the party
        /* public void SetParty(MonsterPartyGroup group)
         {
             if (group is MonsterPartyGroup)
                 currentParty = group;
         }
         */

    }
}

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using MoonSharp.Interpreter;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Npc : Character
    {
        private uint actorClassId;
        private string uniqueIdentifier;

        public NpcWork npcWork = new NpcWork();

        public Npc(int actorNumber, uint classId, string uniqueId, uint zoneId, float posX, float posY, float posZ, float rot, ushort actorState, uint animationId, uint displayNameId, string customDisplayName, string classPath)
            : base((4 << 28 | zoneId << 19 | (uint)actorNumber))  
        {
            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;
            this.rotation = rot;
            this.animationId = animationId;

            this.displayNameId = displayNameId;
            this.customDisplayName = customDisplayName;

            this.uniqueIdentifier = uniqueId;

            this.zoneId = zoneId;
            this.zone = Server.GetWorldManager().GetZone(zoneId);

            this.actorClassId = classId;

            loadNpcAppearance(classId);

            this.classPath = classPath;
            className = classPath.Substring(classPath.LastIndexOf("/")+1);

            charaWork.battleSave.potencial = 1.0f;

            charaWork.parameterSave.state_mainSkill[0] = 3;
            charaWork.parameterSave.state_mainSkill[2] = 3;
            charaWork.parameterSave.state_mainSkillLevel = 2;

            charaWork.parameterSave.hp[0] = 500;
            charaWork.parameterSave.hpMax[0] = 500;
            charaWork.property[0] = 1;
            charaWork.property[1] = 1;

            if (className.Equals("JellyfishScenarioLimsaLv00"))
            {
                charaWork.property[2] = 1;
                npcWork.hateType = 1;
            }

            charaWork.property[3] = 1;
            charaWork.property[4] = 1;

            npcWork.pushCommand = 0x271D;
            npcWork.pushCommandPriority = 1;

            generateActorName((int)actorNumber);            
        }

        public SubPacket CreateAddActorPacket(uint playerActorId)
        {
            return AddActorPacket.BuildPacket(actorId, playerActorId, 8);
        }

        // actorClassId, [], [], numBattleCommon, [battleCommon], numEventCommon, [eventCommon], args for either initForBattle/initForEvent
        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            Player player = Server.GetWorldManager().GetPCInWorld(playerActorId);
            lParams = DoActorInit(player);            

            if (lParams == null)
            {
                string classPathFake = "/Chara/Npc/Populace/PopulaceStandard";
                string classNameFake = "PopulaceStandard";
                lParams = LuaUtils.CreateLuaParamList(classPathFake, false, false, false, false, false, 0xF47F6, false, false, 0, 0);
                ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, classNameFake, lParams).DebugPrintSubPacket();
                return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, classNameFake, lParams);
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

            ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);
        }

        public override BasePacket GetSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(playerActorId));
            subpackets.AddRange(GetEventConditionPackets(playerActorId));
            subpackets.Add(CreateSpeedPacket(playerActorId));            
            subpackets.Add(CreateSpawnPositonPacket(playerActorId, 0x0));            
            subpackets.Add(CreateAppearancePacket(playerActorId));
            subpackets.Add(CreateNamePacket(playerActorId));
            subpackets.Add(CreateStatePacket(playerActorId));
            subpackets.Add(CreateIdleAnimationPacket(playerActorId));
            subpackets.Add(CreateInitStatusPacket(playerActorId));
            subpackets.Add(CreateSetActorIconPacket(playerActorId));
            subpackets.Add(CreateIsZoneingPacket(playerActorId));           
            subpackets.Add(CreateScriptBindPacket(playerActorId));            

            return BasePacket.CreatePacket(subpackets, true, false);
        }

        public override BasePacket GetInitPackets(uint playerActorId)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("/_init", this, playerActorId);

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
                if (charaWork.statusShownTime[i] != 0xFFFFFFFF)
                    propPacketUtil.AddProperty(String.Format("charaWork.statusShownTime[{0}]", i));
            }

            //General Parameters
            for (int i = 3; i < charaWork.battleTemp.generalParameter.Length; i++)
            {
                if (charaWork.battleTemp.generalParameter[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.generalParameter[{0}]", i));
            }

            propPacketUtil.AddProperty("npcWork.hateType");
            propPacketUtil.AddProperty("npcWork.pushCommand");
            propPacketUtil.AddProperty("npcWork.pushCommandPriority");

            return BasePacket.CreatePacket(propPacketUtil.Done(), true, false);
        }

        public uint GetActorClassId()
        {
            return actorClassId;
        }

        public void loadNpcAppearance(uint id)
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
                            modelId = reader.GetUInt32(0);
                            appearanceIds[Character.SIZE] = reader.GetUInt32(1);
                            appearanceIds[Character.COLORINFO] = (uint)(reader.GetUInt32(16) | (reader.GetUInt32(15) << 10) | (reader.GetUInt32(17) << 20)); //17 - Skin Color, 16 - Hair Color, 18 - Eye Color
                            appearanceIds[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.GetFaceInfo(reader.GetByte(6), reader.GetByte(7), reader.GetByte(5), reader.GetByte(14), reader.GetByte(13), reader.GetByte(12), reader.GetByte(11), reader.GetByte(10), reader.GetByte(9), reader.GetByte(8)));
                            appearanceIds[Character.HIGHLIGHT_HAIR] = (uint)(reader.GetUInt32(3) | reader.GetUInt32(2) << 10); //5- Hair Highlight, 4 - Hair Style
                            appearanceIds[Character.VOICE] = reader.GetUInt32(17);
                            appearanceIds[Character.MAINHAND] = reader.GetUInt32(19);
                            appearanceIds[Character.OFFHAND] = reader.GetUInt32(20);
                            appearanceIds[Character.SPMAINHAND] = reader.GetUInt32(21);
                            appearanceIds[Character.SPOFFHAND] = reader.GetUInt32(22);
                            appearanceIds[Character.THROWING] = reader.GetUInt32(23);
                            appearanceIds[Character.PACK] = reader.GetUInt32(24);
                            appearanceIds[Character.POUCH] = reader.GetUInt32(25);
                            appearanceIds[Character.HEADGEAR] = reader.GetUInt32(26);
                            appearanceIds[Character.BODYGEAR] = reader.GetUInt32(27);
                            appearanceIds[Character.LEGSGEAR] = reader.GetUInt32(28);
                            appearanceIds[Character.HANDSGEAR] = reader.GetUInt32(29);
                            appearanceIds[Character.FEETGEAR] = reader.GetUInt32(30);
                            appearanceIds[Character.WAISTGEAR] = reader.GetUInt32(31);
                            appearanceIds[Character.NECKGEAR] = reader.GetUInt32(32);
                            appearanceIds[Character.R_EAR] = reader.GetUInt32(33);
                            appearanceIds[Character.L_EAR] = reader.GetUInt32(34);
                            appearanceIds[Character.R_INDEXFINGER] = reader.GetUInt32(35);
                            appearanceIds[Character.L_INDEXFINGER] = reader.GetUInt32(36);
                            appearanceIds[Character.R_RINGFINGER] = reader.GetUInt32(37);
                            appearanceIds[Character.L_RINGFINGER] = reader.GetUInt32(38);

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

        public void loadEventConditions(string eventConditions)
        {
            EventList conditions = JsonConvert.DeserializeObject<EventList>(eventConditions);
            this.eventConditions = conditions;
        }

        public List<LuaParam> DoActorInit(Player player)
        {
            Script parent = null, child = null;

            if (File.Exists("./scripts/base/" + classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + classPath + ".lua");
            if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier)))
                child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier));

            if (parent == null && child == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", GetName()));
                return null;
            }

            DynValue result;
                            
            if (child != null && child.Globals["init"] != null)
                result = child.Call(child.Globals["init"], this);
            else if (parent != null && parent.Globals["init"] != null)
                result = parent.Call(parent.Globals["init"], this);
            else
                return null;

            List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
            return lparams;          
        }

        public void DoEventStart(Player player, EventStartPacket eventStart)
        {
            Script parent = null, child = null;

            if (File.Exists("./scripts/base/" + classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + classPath + ".lua");
            if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier)))
                child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier));

            if (parent == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", GetName()));
                return;
            }

            //Have to do this to combine LuaParams
            List<Object> objects = new List<Object>();
            objects.Add(player);
            objects.Add(this);
            objects.Add(eventStart.triggerName);

            if (eventStart.luaParams != null)
                objects.AddRange(LuaUtils.CreateLuaParamObjectList(eventStart.luaParams));

            //Run Script
            DynValue result;

            if (child != null && !child.Globals.Get("onEventStarted").IsNil())
                result = child.Call(child.Globals["onEventStarted"], objects.ToArray());
            else if (!parent.Globals.Get("onEventStarted").IsNil())
                result = parent.Call(parent.Globals["onEventStarted"], objects.ToArray());
            else
                return;

        }

        public void DoEventUpdate(Player player, EventUpdatePacket eventUpdate)
        {
            Script parent = null, child = null;

            if (File.Exists("./scripts/base/" + classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + classPath + ".lua");
            if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier)))
                child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier));

            if (parent == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", GetName()));
                return;
            }

            //Have to do this to combine LuaParams
            List<Object> objects = new List<Object>();
            objects.Add(player);
            objects.Add(this);
            objects.Add(eventUpdate.val2);
            objects.AddRange(LuaUtils.CreateLuaParamObjectList(eventUpdate.luaParams));

            //Run Script
            DynValue result;

            if (child != null && !child.Globals.Get("onEventUpdate").IsNil())
                result = child.Call(child.Globals["onEventUpdate"], objects.ToArray());
            else if (!parent.Globals.Get("onEventUpdate").IsNil())
                result = parent.Call(parent.Globals["onEventUpdate"], objects.ToArray());
            else
                return;

        }

        internal void DoOnActorSpawn(Player player)
        {
            Script parent = null, child = null;

            if (File.Exists("./scripts/base/" + classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + classPath + ".lua");
            if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier)))
                child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", zone.zoneName, className, uniqueIdentifier));

            if (parent == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", GetName()));
                return;
            }
               
            //Run Script
            if (child != null && !child.Globals.Get("onSpawn").IsNil())
                child.Call(child.Globals["onSpawn"], player, this);
            else if (!parent.Globals.Get("onSpawn").IsNil())
                parent.Call(parent.Globals["onSpawn"], player, this);
            else
                return;                
        }
    }
}

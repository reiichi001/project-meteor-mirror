using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
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

        public Npc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot, ushort actorState, uint animationId, string customDisplayName)
            : base((4 << 28 | spawnedArea.actorId << 19 | (uint)actorNumber))  
        {
            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;
            this.rotation = rot;
            this.animationId = animationId;

            this.displayNameId = actorClass.displayNameId;
            this.customDisplayName = customDisplayName;

            this.uniqueIdentifier = uniqueId;

            this.zoneId = spawnedArea.actorId;
            this.zone = spawnedArea;

            this.actorClassId = actorClass.actorClassId;

            LoadNpcAppearance(actorClass.actorClassId);

            this.classPath = actorClass.classPath;
            className = classPath.Substring(classPath.LastIndexOf("/")+1);

            charaWork.battleSave.potencial = 1.0f;

            charaWork.parameterSave.state_mainSkill[0] = 3;
            charaWork.parameterSave.state_mainSkill[2] = 3;
            charaWork.parameterSave.state_mainSkillLevel = 2;

            charaWork.parameterSave.hp[0] = 500;
            charaWork.parameterSave.hpMax[0] = 500;

            for (int i = 0; i < 32; i++ )            
                charaWork.property[i] = (byte)(((int)actorClass.propertyFlags >> i) & 1);            

            npcWork.pushCommand = actorClass.pushCommand;
            npcWork.pushCommandSub = actorClass.pushCommandSub;
            npcWork.pushCommandPriority = actorClass.pushCommandPriority;

            GenerateActorName((int)actorNumber);            
        }

        public SubPacket CreateAddActorPacket(uint playerActorId)
        {
            return AddActorPacket.BuildPacket(actorId, playerActorId, 8);
        }

        int val = 0x0b00;
        // actorClassId, [], [], numBattleCommon, [battleCommon], numEventCommon, [eventCommon], args for either initForBattle/initForEvent
        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            Player player = Server.GetWorldManager().GetPCInWorld(playerActorId);
            lParams = LuaEngine.GetInstance().CallLuaFunctionForReturn(player, this, "init");

            if (uniqueIdentifier.Equals("1"))
            {
                lParams[5].value = val;
                val++;
                player.SendMessage(0x20, "", String.Format("ID is now: 0x{0:X}", val));                
            }

            if (lParams != null && lParams.Count >= 3 && lParams[2].typeID == 0 && (int)lParams[2].value == 0)
                isStatic = true;
            else
            {
                charaWork.property[2] = 1;
                npcWork.hateType = 1;
            }

            if (lParams == null)
            {
                string classPathFake = "/Chara/Npc/Populace/PopulaceStandard";
                string classNameFake = "PopulaceStandard";
                lParams = LuaUtils.CreateLuaParamList(classPathFake, false, false, false, false, false, 0xF47F6, false, false, 0, 0);
                isStatic = true;
                //ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, classNameFake, lParams).DebugPrintSubPacket();
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

            //ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);
        }
        
        public override BasePacket GetSpawnPackets(uint playerActorId, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(playerActorId));
            subpackets.AddRange(GetEventConditionPackets(playerActorId));
            subpackets.Add(CreateSpeedPacket(playerActorId));            
            subpackets.Add(CreateSpawnPositonPacket(playerActorId, 0x0));

            if (uniqueIdentifier.Equals("door1"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, 0xB0D, 0x1af));
            }
            else if (uniqueIdentifier.Equals("door2"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, 0xB09, 0x1af));
            }
            else if (uniqueIdentifier.Equals("closed_gridania_gate"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, 0xB79, 0x141));
            }
            else if (uniqueIdentifier.Equals("uldah_mapshipport_1"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId,  0xdc5, 0x1af));
                subpackets[subpackets.Count - 1].DebugPrintSubPacket();
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId,  "end0"));
                subpackets[subpackets.Count - 1].DebugPrintSubPacket();
            }
            else if (uniqueIdentifier.Equals("uldah_mapshipport_2"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId,  0x2, 0x1eb));
                subpackets[subpackets.Count - 1].DebugPrintSubPacket();
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId,  "end0"));
                subpackets[subpackets.Count - 1].DebugPrintSubPacket();
            }
            else if (uniqueIdentifier.Equals("gridania_shipport"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId,playerActorId,  0xcde, 0x141));
                subpackets.Add(_0xD9Packet.BuildPacket(actorId,playerActorId,  "end0"));
            }
            else if (uniqueIdentifier.Equals("gridania_shipport2"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId,  0x02, 0x187));
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId,  "end0"));
            }
            else if (uniqueIdentifier.Equals("limsa_shipport"))
            {
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, 0x1c8, 0xc4));
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId, "spin"));
            }
            else if (actorClassId == 5900013)
            {
                uint id = 201;
                uint id2 = 0x1415;
                string val = "fdin";
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, id, id2));
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId, val));
            }
            else if (actorClassId == 5900014)
            {
                uint id = 201;
                uint id2 = 0x1415;
                string val = "fdot";
                subpackets.Add(_0xD8Packet.BuildPacket(actorId, playerActorId, id, id2));
                subpackets.Add(_0xD9Packet.BuildPacket(actorId, playerActorId, val));
            }
            else
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

            if (npcWork.pushCommand != 0)
            {
                propPacketUtil.AddProperty("npcWork.pushCommand");
                if (npcWork.pushCommandSub != 0)
                    propPacketUtil.AddProperty("npcWork.pushCommandSub");
                propPacketUtil.AddProperty("npcWork.pushCommandPriority");
            }

            return BasePacket.CreatePacket(propPacketUtil.Done(), true, false);
        }

        public string GetUniqueId()
        {
            return uniqueIdentifier;
        }

        public uint GetActorClassId()
        {
            return actorClassId;
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

        public void LoadEventConditions(string eventConditions)
        {
            EventList conditions = JsonConvert.DeserializeObject<EventList>(eventConditions);
            this.eventConditions = conditions;
        }

        public void DoOnActorSpawn(Player player)
        {
            LuaEngine.GetInstance().CallLuaFunction(player, this, "onSpawn");           
        }

        public void Update(double deltaTime)
        {
            LuaEngine.GetInstance().CallLuaFunction(null, this, "onUpdate", deltaTime);         
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

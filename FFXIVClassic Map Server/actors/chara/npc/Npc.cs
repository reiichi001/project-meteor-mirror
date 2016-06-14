using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class Npc : Character
    {
        private uint actorClassId;

        public NpcWork npcWork = new NpcWork();

        public Npc(uint id, string actorName, uint zoneId, float posX, float posY, float posZ, float rot, ushort actorState, uint animationId, uint displayNameId, string customDisplayName, string className)
            : base(id)
        {
            this.actorName = actorName;
            this.actorClassId = id;
            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;
            this.rotation = rot;
            this.animationId = animationId;
            this.className = className;

            this.displayNameId = displayNameId;
            this.customDisplayName = customDisplayName;

            this.zoneId = zoneId;

            LoadNpcTemplate(id);

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
        }

        public SubPacket CreateAddActorPacket(uint playerActorId)
        {
            return AddActorPacket.BuildPacket(actorId, playerActorId, 8);
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            Player player = Server.GetWorldManager().GetPCInWorld(playerActorId);
            lParams = LuaEngine.DoActorInstantiate(player, this);

            if (lParams == null)
            {
                className = "PopulaceStandard";
                lParams = LuaUtils.CreateLuaParamList("/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, 0xF47F6, false, false, 0, 1, "TEST");
            }

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

            return BasePacket.CreatePacket(propPacketUtil.Done(), true, false);
        }

        public uint GetActorClassId()
        {
            return actorClassId;
        }

        public void LoadNpcTemplate(uint id)
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
                            //appearanceIds[Character.WEAPON2] = reader.GetUInt32(22);
                            appearanceIds[Character.HEADGEAR] = reader.GetUInt32(26);
                            appearanceIds[Character.BODYGEAR] = reader.GetUInt32(27);
                            appearanceIds[Character.LEGSGEAR] = reader.GetUInt32(28);
                            appearanceIds[Character.HANDSGEAR] = reader.GetUInt32(29);
                            appearanceIds[Character.FEETGEAR] = reader.GetUInt32(30);
                            appearanceIds[Character.WAISTGEAR] = reader.GetUInt32(31);
                            appearanceIds[Character.R_EAR] = reader.GetUInt32(32);
                            appearanceIds[Character.L_EAR] = reader.GetUInt32(33);
                            appearanceIds[Character.R_RINGFINGER] = reader.GetUInt32(36);
                            appearanceIds[Character.L_RINGFINGER] = reader.GetUInt32(37);

                        }
                    }

                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                }
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
    }
}

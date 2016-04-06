using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Npc : Character
    {
        private uint actorClassId;

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

            loadNpcTemplate(id);
        }

        public SubPacket createAddActorPacket(uint playerActorId)
        {
            return AddActorPacket.buildPacket(actorId, playerActorId, 8);
        }

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            Player player = Server.getWorldManager().GetPCInWorld(playerActorId);
            lParams = LuaEngine.doActorOnInstantiate(player, this);

            if (lParams == null)
            {
                className = "PopulaceStandard";
                lParams = LuaUtils.createLuaParamList("/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, 0xF47F6, false, false, 0, 1, "TEST");
            }

            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }

        public override BasePacket getSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId));
            subpackets.AddRange(getEventConditionPackets(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));            
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0x0));            
            subpackets.Add(createAppearancePacket(playerActorId));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIdleAnimationPacket(playerActorId));
            subpackets.Add(createInitStatusPacket(playerActorId));
            subpackets.Add(createSetActorIconPacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));           
            subpackets.Add(createScriptBindPacket(playerActorId));            

            return BasePacket.createPacket(subpackets, true, false);
        }

        public uint getActorClassId()
        {
            return actorClassId;
        }

        public void loadNpcTemplate(uint id)
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
                            appearanceIds[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.getFaceInfo(reader.GetByte(6), reader.GetByte(7), reader.GetByte(5), reader.GetByte(14), reader.GetByte(13), reader.GetByte(12), reader.GetByte(11), reader.GetByte(10), reader.GetByte(9), reader.GetByte(8)));
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
    }
}

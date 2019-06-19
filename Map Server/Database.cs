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

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Meteor.Common;
using FFXIVClassic_Map_Server.utils;

using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.packets.receive.supportdesk;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.packets.send.actor.battle;

namespace FFXIVClassic_Map_Server
{

    class Database
    {

        public static uint GetUserIdFromSession(String sessionId)
        {
            uint id = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM sessions WHERE id = @sessionId AND expiration > NOW()", conn);
                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            id = Reader.GetUInt32("userId");
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
            return id;
        }

        public static Dictionary<uint, ItemData> GetItemGamedata()
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                Dictionary<uint, ItemData> gamedataItems = new Dictionary<uint, ItemData>();

                try
                {
                    conn.Open();

                    string query = @"
                                SELECT
                                *                                
                                FROM gamedata_items
                                LEFT JOIN gamedata_items_equipment        ON gamedata_items.catalogID = gamedata_items_equipment.catalogID
                                LEFT JOIN gamedata_items_accessory        ON gamedata_items.catalogID = gamedata_items_accessory.catalogID
                                LEFT JOIN gamedata_items_armor            ON gamedata_items.catalogID = gamedata_items_armor.catalogID
                                LEFT JOIN gamedata_items_weapon           ON gamedata_items.catalogID = gamedata_items_weapon.catalogID
                                LEFT JOIN gamedata_items_graphics         ON gamedata_items.catalogID = gamedata_items_graphics.catalogID                                
                                LEFT JOIN gamedata_items_graphics_extra   ON gamedata_items.catalogID = gamedata_items_graphics_extra.catalogID
                                ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("catalogID");
                            ItemData item = null;

                            if (ItemData.IsWeapon(id))
                                item = new WeaponItem(reader);
                            else if (ItemData.IsArmor(id))
                                item = new ArmorItem(reader);
                            else if (ItemData.IsAccessory(id))
                                item = new AccessoryItem(reader);
                            else
                                item = new ItemData(reader);

                            gamedataItems.Add(item.catalogID, item);
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

                return gamedataItems;
            }
        }

        public static Dictionary<uint, GuildleveData> GetGuildleveGamedata()
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                Dictionary<uint, GuildleveData> gamedataGuildleves = new Dictionary<uint, GuildleveData>();

                try
                {
                    conn.Open();

                    string query = @"
                                SELECT
                                *                                
                                FROM gamedata_guildleves
                                ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            GuildleveData guildleve = new GuildleveData(reader);
                            gamedataGuildleves.Add(guildleve.id, guildleve);
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

                return gamedataGuildleves;
            }
        }

        public static void SavePlayerAppearance(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters_appearance SET 
                    mainHand = @mainHand,
                    offHand = @offHand,
                    head = @head,
                    body = @body,
                    legs = @legs,
                    hands = @hands,
                    feet = @feet,
                    waist = @waist,
                    leftFinger = @leftFinger,
                    rightFinger = @rightFinger,
                    leftEar = @leftEar,
                    rightEar = @rightEar
                    WHERE characterId = @charaId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@mainHand", player.appearanceIds[Character.MAINHAND]);
                    cmd.Parameters.AddWithValue("@offHand", player.appearanceIds[Character.OFFHAND]);
                    cmd.Parameters.AddWithValue("@head", player.appearanceIds[Character.HEADGEAR]);
                    cmd.Parameters.AddWithValue("@body", player.appearanceIds[Character.BODYGEAR]);
                    cmd.Parameters.AddWithValue("@legs", player.appearanceIds[Character.LEGSGEAR]);
                    cmd.Parameters.AddWithValue("@hands", player.appearanceIds[Character.HANDSGEAR]);
                    cmd.Parameters.AddWithValue("@feet", player.appearanceIds[Character.FEETGEAR]);
                    cmd.Parameters.AddWithValue("@waist", player.appearanceIds[Character.WAISTGEAR]);
                    cmd.Parameters.AddWithValue("@leftFinger", player.appearanceIds[Character.L_RINGFINGER]);
                    cmd.Parameters.AddWithValue("@rightFinger", player.appearanceIds[Character.R_RINGFINGER]);
                    cmd.Parameters.AddWithValue("@leftEar", player.appearanceIds[Character.L_EAR]);
                    cmd.Parameters.AddWithValue("@rightEar", player.appearanceIds[Character.R_EAR]);

                    cmd.ExecuteNonQuery();
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

        public static void SavePlayerCurrentClass(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters_parametersave SET 
                    mainSkill = @classId,
                    mainSkillLevel = @classLevel
                    WHERE characterId = @charaId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", player.charaWork.parameterSave.state_mainSkill[0]);
                    cmd.Parameters.AddWithValue("@classLevel", player.charaWork.parameterSave.state_mainSkillLevel);

                    cmd.ExecuteNonQuery();
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

        public static void SavePlayerPosition(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters SET 
                    positionX = @x,
                    positionY = @y,
                    positionZ = @z,
                    rotation = @rot,
                    destinationZoneId = @destZone,
                    destinationSpawnType = @destSpawn,
                    currentZoneId = @zoneId,
                    currentPrivateArea = @privateArea,
                    currentPrivateAreaType = @privateAreaType
                    WHERE id = @charaId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@x", player.positionX);
                    cmd.Parameters.AddWithValue("@y", player.positionY);
                    cmd.Parameters.AddWithValue("@z", player.positionZ);
                    cmd.Parameters.AddWithValue("@rot", player.rotation);
                    cmd.Parameters.AddWithValue("@zoneId", player.zoneId);
                    cmd.Parameters.AddWithValue("@privateArea", player.privateArea);
                    cmd.Parameters.AddWithValue("@privateAreaType", player.privateAreaType);
                    cmd.Parameters.AddWithValue("@destZone", player.destinationZone);
                    cmd.Parameters.AddWithValue("@destSpawn", player.destinationSpawnType);

                    cmd.ExecuteNonQuery();
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

        public static void SavePlayerPlayTime(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters SET 
                    playTime = @playtime
                    WHERE id = @charaId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@playtime", player.GetPlayTime(true));

                    cmd.ExecuteNonQuery();
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

        public static void SavePlayerHomePoints(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters SET 
                    homepoint = @homepoint,
                    homepointInn = @homepointInn
                    WHERE id = @charaId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@homepoint", player.homepoint);
                    cmd.Parameters.AddWithValue("@homepointInn", player.homepointInn);

                    cmd.ExecuteNonQuery();
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

        public static void SaveQuest(Player player, Quest quest)
        {
            int slot = player.GetQuestSlot(quest.actorId);
            if (slot == -1)
            {
                Program.Log.Error("Tried saving quest player didn't have: Player: {0:x}, QuestId: {0:x}", player.actorId, quest.actorId);
                return;
            }
            else
                SaveQuest(player, quest, slot);
        }

        public static void SaveQuest(Player player, Quest quest, int slot)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO characters_quest_scenario 
                    (characterId, slot, questId, currentPhase, questData, questFlags)
                    VALUES
                    (@charaId, @slot, @questId, @phase, @questData, @questFlags)
                    ON DUPLICATE KEY UPDATE
                    questId = @questId, currentPhase = @phase, questData = @questData, questFlags = @questFlags
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@questId", 0xFFFFF & quest.actorId);
                    cmd.Parameters.AddWithValue("@phase", quest.GetPhase());
                    cmd.Parameters.AddWithValue("@questData", quest.GetSerializedQuestData());
                    cmd.Parameters.AddWithValue("@questFlags", quest.GetQuestFlags());

                    cmd.ExecuteNonQuery();
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

        public static void MarkGuildleve(Player player, uint glId, bool isAbandoned, bool isCompleted)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters_quest_guildleve_regional
                    SET abandoned = @abandoned, completed = @completed
                    WHERE characterId = @charaId and guildleveId = @guildleveId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@guildleveId", glId);
                    cmd.Parameters.AddWithValue("@abandoned", isAbandoned);
                    cmd.Parameters.AddWithValue("@completed", isCompleted);

                    cmd.ExecuteNonQuery();
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

        public static void SaveGuildleve(Player player, uint glId, int slot)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO characters_quest_guildleve_regional 
                    (characterId, slot, guildleveId, abandoned, completed)
                    VALUES
                    (@charaId, @slot, @guildleveId, @abandoned, @completed)
                    ON DUPLICATE KEY UPDATE
                    guildleveId = @guildleveId, abandoned = @abandoned, completed = @completed
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@guildleveId", glId);
                    cmd.Parameters.AddWithValue("@abandoned", 0);
                    cmd.Parameters.AddWithValue("@completed", 0);

                    cmd.ExecuteNonQuery();
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

        public static void RemoveGuildleve(Player player, uint glId)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    DELETE FROM characters_quest_guildleve_regional 
                    WHERE characterId = @charaId and guildleveId = @guildleveId                 
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@guildleveId", glId);

                    cmd.ExecuteNonQuery();
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

        public static void RemoveQuest(Player player, uint questId)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    DELETE FROM characters_quest_scenario 
                    WHERE characterId = @charaId and questId = @questId                 
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@questId", 0xFFFFF & questId);

                    cmd.ExecuteNonQuery();
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

        public static void CompleteQuest(Player player, uint questId)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO characters_quest_completed 
                    (characterId, questId)
                    VALUES
                    (@charaId, @questId)
                    ON DUPLICATE KEY UPDATE characterId=characterId
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@questId", 0xFFFFF & questId);

                    cmd.ExecuteNonQuery();
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

        public static bool IsQuestCompleted(Player player, uint questId)
        {
            bool isCompleted = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM characters_quest_completed WHERE characterId = @charaId and questId = @questId", conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@questId", questId);
                    isCompleted = cmd.ExecuteScalar() != null;
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
            return isCompleted;
        }

        public static void LoadPlayerCharacter(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load basic info                  
                    query = @"
                    SELECT 
                    name,            
                    positionX,
                    positionY,
                    positionZ,
                    rotation,
                    actorState,
                    currentZoneId,             
                    gcCurrent,
                    gcLimsaRank,
                    gcGridaniaRank,
                    gcUldahRank,
                    currentTitle,
                    guardian,
                    birthDay,
                    birthMonth,
                    initialTown,
                    tribe,
                    restBonus,
                    achievementPoints,
                    playTime,
                    destinationZoneId,
                    destinationSpawnType,
                    currentPrivateArea,
                    currentPrivateAreaType,
                    homepoint,
                    homepointInn
                    FROM characters WHERE id = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.displayNameId = 0xFFFFFFFF;
                            player.customDisplayName = reader.GetString(0);
                            player.oldPositionX = player.positionX = reader.GetFloat(1);
                            player.oldPositionY = player.positionY = reader.GetFloat(2);
                            player.oldPositionZ = player.positionZ = reader.GetFloat(3);
                            player.oldRotation = player.rotation = reader.GetFloat(4);
                            player.currentMainState = reader.GetUInt16(5);
                            player.zoneId = reader.GetUInt32(6);
                            player.isZoning = true;
                            player.gcCurrent = reader.GetByte(7);
                            player.gcRankLimsa = reader.GetByte(8);
                            player.gcRankGridania = reader.GetByte(9);
                            player.gcRankUldah = reader.GetByte(10);
                            player.currentTitle = reader.GetUInt32(11);
                            player.playerWork.guardian = reader.GetByte(12);
                            player.playerWork.birthdayDay = reader.GetByte(13);
                            player.playerWork.birthdayMonth = reader.GetByte(14);
                            player.playerWork.initialTown = reader.GetByte(15);
                            player.playerWork.tribe = reader.GetByte(16);
                            player.playerWork.restBonusExpRate = reader.GetInt32(17);
                            player.achievementPoints = reader.GetUInt32(18);
                            player.playTime = reader.GetUInt32(19);
                            player.homepoint = reader.GetUInt32("homepoint");
                            player.homepointInn = reader.GetByte("homepointInn");
                            player.destinationZone = reader.GetUInt32("destinationZoneId");
                            player.destinationSpawnType = reader.GetByte("destinationSpawnType");

                            if (!reader.IsDBNull(reader.GetOrdinal("currentPrivateArea")))
                                player.privateArea = reader.GetString("currentPrivateArea");
                            player.privateAreaType = reader.GetUInt32("currentPrivateAreaType");

                            if (player.destinationZone != 0)
                                player.zoneId = player.destinationZone;

                            if (player.privateArea != null && !player.privateArea.Equals(""))
                                player.zone = Server.GetWorldManager().GetPrivateArea(player.zoneId, player.privateArea, player.privateAreaType);
                            else
                                player.zone = Server.GetWorldManager().GetZone(player.zoneId);
                        }
                    }

                    //Get class levels
                    query = @"
                        SELECT 
                        pug,
                        gla,
                        mrd,
                        arc,
                        lnc,

                        thm,
                        cnj,

                        crp,
                        bsm,
                        arm,
                        gsm,
                        ltw,
                        wvr,
                        alc,
                        cul,

                        min,
                        btn,
                        fsh
                        FROM characters_class_levels WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_PUG - 1] = reader.GetInt16("pug");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_GLA - 1] = reader.GetInt16("gla");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_MRD - 1] = reader.GetInt16("mrd");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_ARC - 1] = reader.GetInt16("arc");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_LNC - 1] = reader.GetInt16("lnc");

                            player.charaWork.battleSave.skillLevel[Player.CLASSID_THM - 1] = reader.GetInt16("thm");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_CNJ - 1] = reader.GetInt16("cnj");

                            player.charaWork.battleSave.skillLevel[Player.CLASSID_CRP - 1] = reader.GetInt16("crp");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_BSM - 1] = reader.GetInt16("bsm");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_ARM - 1] = reader.GetInt16("arm");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_GSM - 1] = reader.GetInt16("gsm");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_LTW - 1] = reader.GetInt16("ltw");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_WVR - 1] = reader.GetInt16("wvr");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_ALC - 1] = reader.GetInt16("alc");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_CUL - 1] = reader.GetInt16("cul");

                            player.charaWork.battleSave.skillLevel[Player.CLASSID_MIN - 1] = reader.GetInt16("min");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_BTN - 1] = reader.GetInt16("btn");
                            player.charaWork.battleSave.skillLevel[Player.CLASSID_FSH - 1] = reader.GetInt16("fsh");
                        }
                    }

                    //Get class experience
                    query = @"
                        SELECT 
                        pug,
                        gla,
                        mrd,
                        arc,
                        lnc,

                        thm,
                        cnj,

                        crp,
                        bsm,
                        arm,
                        gsm,
                        ltw,
                        wvr,
                        alc,
                        cul,

                        min,
                        btn,
                        fsh
                        FROM characters_class_exp WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_PUG - 1] = reader.GetInt32("pug");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_GLA - 1] = reader.GetInt32("gla");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_MRD - 1] = reader.GetInt32("mrd");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_ARC - 1] = reader.GetInt32("arc");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_LNC - 1] = reader.GetInt32("lnc");
                            
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_THM - 1] = reader.GetInt32("thm");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_CNJ - 1] = reader.GetInt32("cnj");
                            
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_CRP - 1] = reader.GetInt32("crp");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_BSM - 1] = reader.GetInt32("bsm");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_ARM - 1] = reader.GetInt32("arm");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_GSM - 1] = reader.GetInt32("gsm");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_LTW - 1] = reader.GetInt32("ltw");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_WVR - 1] = reader.GetInt32("wvr");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_ALC - 1] = reader.GetInt32("alc");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_CUL - 1] = reader.GetInt32("cul");
                            
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_MIN - 1] = reader.GetInt32("min");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_BTN - 1] = reader.GetInt32("btn");
                            player.charaWork.battleSave.skillPoint[Player.CLASSID_FSH - 1] = reader.GetInt32("fsh");
                        }
                    }

                    //Load Saved Parameters
                    query = @"
                        SELECT 
                        hp,
                        hpMax,
                        mp,
                        mpMax,
                        mainSkill
                        FROM characters_parametersave WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.charaWork.parameterSave.hp[0] = reader.GetInt16(0);
                            player.charaWork.parameterSave.hpMax[0] = reader.GetInt16(1);
                            player.charaWork.parameterSave.mp = reader.GetInt16(2);
                            player.charaWork.parameterSave.mpMax = reader.GetInt16(3);

                            player.charaWork.parameterSave.state_mainSkill[0] = reader.GetByte(4);
                            player.charaWork.parameterSave.state_mainSkillLevel = player.charaWork.battleSave.skillLevel[reader.GetByte(4) - 1];
                        }
                    }

                    //Load appearance
                    query = @"
                        SELECT 
                        baseId,                       
                        size,
                        voice,
                        skinColor,
                        hairStyle,
                        hairColor,
                        hairHighlightColor,
                        eyeColor,
                        characteristics,
                        characteristicsColor,
                        faceType,
                        ears,
                        faceMouth,
                        faceFeatures,
                        faceNose,
                        faceEyeShape,
                        faceIrisSize,
                        faceEyebrows,
                        mainHand,
                        offHand,
                        head,
                        body,
                        legs,
                        hands,
                        feet,
                        waist,
                        leftFinger,
                        rightFinger,
                        leftEar,
                        rightEar
                        FROM characters_appearance WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetUInt32(0) == 0xFFFFFFFF)
                                player.modelId = CharacterUtils.GetTribeModel(player.playerWork.tribe);
                            else
                                player.modelId = reader.GetUInt32(0);
                            player.appearanceIds[Character.SIZE] = reader.GetByte(1);
                            player.appearanceIds[Character.COLORINFO] = (uint)(reader.GetUInt16(3) | (reader.GetUInt16(5) << 10) | (reader.GetUInt16(7) << 20));
                            player.appearanceIds[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.GetFaceInfo(reader.GetByte(8), reader.GetByte(9), reader.GetByte(10), reader.GetByte(11), reader.GetByte(12), reader.GetByte(13), reader.GetByte(14), reader.GetByte(15), reader.GetByte(16), reader.GetByte(17)));
                            player.appearanceIds[Character.HIGHLIGHT_HAIR] = (uint)(reader.GetUInt16(6) | reader.GetUInt16(4) << 10);
                            player.appearanceIds[Character.VOICE] = reader.GetByte(2);
                            player.appearanceIds[Character.MAINHAND] = reader.GetUInt32(18);
                            player.appearanceIds[Character.OFFHAND] = reader.GetUInt32(19);
                            player.appearanceIds[Character.HEADGEAR] = reader.GetUInt32(20);
                            player.appearanceIds[Character.BODYGEAR] = reader.GetUInt32(21);
                            player.appearanceIds[Character.LEGSGEAR] = reader.GetUInt32(22);
                            player.appearanceIds[Character.HANDSGEAR] = reader.GetUInt32(23);
                            player.appearanceIds[Character.FEETGEAR] = reader.GetUInt32(24);
                            player.appearanceIds[Character.WAISTGEAR] = reader.GetUInt32(25);
                            player.appearanceIds[Character.R_EAR] = reader.GetUInt32(26);
                            player.appearanceIds[Character.L_EAR] = reader.GetUInt32(27);
                            player.appearanceIds[Character.R_RINGFINGER] = reader.GetUInt32(28);
                            player.appearanceIds[Character.L_RINGFINGER] = reader.GetUInt32(29);
                        }

                    }

                    //Load Status Effects
                    query = @"
                        SELECT 
                        statusId,
                        duration,
                        magnitude,
                        tick,
                        tier,
                        extra
                        FROM characters_statuseffect WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetUInt32("statusId");
                            var duration = reader.GetUInt32("duration");
                            var magnitude = reader.GetUInt64("magnitude");
                            var tick = reader.GetUInt32("tick");
                            var tier = reader.GetByte("tier");
                            var extra = reader.GetUInt64("extra");

                            var effect = Server.GetWorldManager().GetStatusEffect(id);
                            if (effect != null)
                            {
                                effect.SetDuration(duration);
                                effect.SetMagnitude(magnitude);
                                effect.SetTickMs(tick);
                                effect.SetTier(tier);
                                effect.SetExtra(extra);

                                // dont wanna send ton of messages on login (i assume retail doesnt)
                                player.statusEffects.AddStatusEffect(effect, null);
                            }
                        }
                    }

                    //Load Chocobo
                    query = @"
                        SELECT 
                        hasChocobo,
                        hasGoobbue,
                        chocoboAppearance,
                        chocoboName                             
                        FROM characters_chocobo WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.hasChocobo = reader.GetBoolean(0);
                            player.hasGoobbue = reader.GetBoolean(1);
                            player.chocoboAppearance = reader.GetByte(2);
                            player.chocoboName = reader.GetString(3);
                        }
                    }

                    //Load Timers
                    query = @"
                        SELECT 
                        thousandmaws,
                        dzemaeldarkhold,
                        bowlofembers_hard,                
                        bowlofembers,
                        thornmarch,
                        aurumvale,
                        cutterscry,
                        battle_aleport,
                        battle_hyrstmill,
                        battle_goldenbazaar,
                        howlingeye_hard,
                        howlingeye,
                        castrumnovum,
                        bowlofembers_extreme,
                        rivenroad,
                        rivenroad_hard,
                        behests,
                        companybehests,
                        returntimer,
                        skirmish
                        FROM characters_timers WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < player.timers.Length; i++)
                                player.timers[i] = reader.GetUInt32(i);
                        }
                    }

                    //Load Hotbar
                    LoadHotbar(player);

                    //Load Scenario Quests
                    query = @"
                        SELECT 
                        slot,
                        questId,
                        questData,
                        questFlags,
                        currentPhase
                        FROM characters_quest_scenario WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questScenario[index] = 0xA0F00000 | reader.GetUInt32(1);
                            string questData = null;
                            uint questFlags = 0;
                            uint currentPhase = 0;

                            if (!reader.IsDBNull(2))
                                questData = reader.GetString(2);
                            else
                                questData = "{}";

                            if (!reader.IsDBNull(3))
                                questFlags = reader.GetUInt32(3);
                            else
                                questFlags = 0;

                            if (!reader.IsDBNull(4))
                                currentPhase = reader.GetUInt32(4);

                            string questName = Server.GetStaticActors(player.playerWork.questScenario[index]).actorName;
                            player.questScenario[index] = new Quest(player, player.playerWork.questScenario[index], questName, questData, questFlags, currentPhase);
                        }
                    }

                    //Load Local Guildleves
                    query = @"
                        SELECT 
                        slot,
                        questId,
                        abandoned,
                        completed  
                        FROM characters_quest_guildleve_local WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questGuildleve[index] = 0xA0F00000 | reader.GetUInt32(1);
                        }
                    }

                    //Load Regional Guildleve Quests
                    query = @"
                        SELECT 
                        slot,
                        guildleveId,
                        abandoned,
                        completed  
                        FROM characters_quest_guildleve_regional WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.work.guildleveId[index] = reader.GetUInt16(1);
                            player.work.guildleveDone[index] = reader.GetBoolean(2);
                            player.work.guildleveChecked[index] = reader.GetBoolean(3);
                        }
                    }

                    //Load NPC Linkshell
                    query = @"
                        SELECT 
                        npcLinkshellId,
                        isCalling,
                        isExtra  
                        FROM characters_npclinkshell WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int npcLSId = reader.GetUInt16(0);
                            player.playerWork.npcLinkshellChatCalling[npcLSId] = reader.GetBoolean(1);
                            player.playerWork.npcLinkshellChatExtra[npcLSId] = reader.GetBoolean(2);
                        }
                    }

                    player.GetItemPackage(ItemPackage.NORMAL).InitList(GetItemPackage(player, 0, ItemPackage.NORMAL));
                    player.GetItemPackage(ItemPackage.KEYITEMS).InitList(GetItemPackage(player, 0, ItemPackage.KEYITEMS));
                    player.GetItemPackage(ItemPackage.CURRENCY_CRYSTALS).InitList(GetItemPackage(player, 0, ItemPackage.CURRENCY_CRYSTALS));
                    player.GetItemPackage(ItemPackage.BAZAAR).InitList(GetItemPackage(player, 0, ItemPackage.BAZAAR));
                    player.GetItemPackage(ItemPackage.MELDREQUEST).InitList(GetItemPackage(player, 0, ItemPackage.MELDREQUEST));
                    player.GetItemPackage(ItemPackage.LOOT).InitList(GetItemPackage(player, 0, ItemPackage.LOOT));

                    player.GetEquipment().SetList(GetEquipment(player, player.charaWork.parameterSave.state_mainSkill[0]));
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

        public static InventoryItem[] GetEquipment(Player player, ushort classId)
        {
            InventoryItem[] equipment = new InventoryItem[player.GetEquipment().GetCapacity()];
            
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    equipSlot,
                                    itemId
                                    FROM characters_inventory_equipment                                    
                                    WHERE characterId = @charId AND (classId = @classId OR classId = 0) ORDER BY equipSlot";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", classId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ushort equipSlot = reader.GetUInt16(0);
                            ulong uniqueItemId = reader.GetUInt16(1);
                            InventoryItem item = player.GetItemPackage(ItemPackage.NORMAL).GetItemByUniqueId(uniqueItemId);
                            equipment[equipSlot] = item;
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

            return equipment;
        }

        public static void EquipItem(Player player, ushort equipSlot, ulong uniqueItemId)
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    INSERT INTO characters_inventory_equipment                                    
                                    (characterId, classId, equipSlot, itemId)
                                    VALUES
                                    (@characterId, @classId, @equipSlot, @uniqueItemId)
                                    ON DUPLICATE KEY UPDATE itemId=@uniqueItemId;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", (equipSlot == Player.SLOT_UNDERSHIRT || equipSlot == Player.SLOT_UNDERGARMENT) ? 0 : player.charaWork.parameterSave.state_mainSkill[0]);
                    cmd.Parameters.AddWithValue("@equipSlot", equipSlot);
                    cmd.Parameters.AddWithValue("@uniqueItemId", uniqueItemId);

                    cmd.ExecuteNonQuery();
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

        public static void UnequipItem(Player player, ushort equipSlot)
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    DELETE FROM characters_inventory_equipment                                    
                                    WHERE characterId = @characterId AND classId = @classId AND equipSlot = @equipSlot;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", player.charaWork.parameterSave.state_mainSkill[0]);
                    cmd.Parameters.AddWithValue("@equipSlot", equipSlot);

                    cmd.ExecuteNonQuery();
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
        public static void EquipAbility(Player player, byte classId, ushort hotbarSlot, uint commandId, uint recastTime)
        {
            commandId &= 0xFFFF;
            if (commandId > 0)
            {
                using (MySqlConnection conn = new MySqlConnection(
                    String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}",
                    ConfigConstants.DATABASE_HOST,
                    ConfigConstants.DATABASE_PORT,
                    ConfigConstants.DATABASE_NAME,
                    ConfigConstants.DATABASE_USERNAME,
                    ConfigConstants.DATABASE_PASSWORD)))
                {
                    try
                    {
                        conn.Open();
                        MySqlCommand cmd;
                        string query = @"
                                    INSERT INTO characters_hotbar                                    
                                    (characterId, classId, hotbarSlot, commandId, recastTime)
                                    VALUES
                                    (@charId, @classId, @hotbarSlot, @commandId, @recastTime)
                                    ON DUPLICATE KEY UPDATE commandId=@commandId, recastTime=@recastTime;
                        ";

                        cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@charId", player.actorId);
                        cmd.Parameters.AddWithValue("@classId", classId);
                        cmd.Parameters.AddWithValue("@commandId", commandId);
                        cmd.Parameters.AddWithValue("@hotbarSlot", hotbarSlot);
                        cmd.Parameters.AddWithValue("@recastTime", recastTime);
                        cmd.ExecuteNonQuery();
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
            else
                UnequipAbility(player, hotbarSlot);
        }

        //Unequipping is done by sending an equip packet with 0xA0F00000 as the ability and the hotbar slot of the action being unequipped
        public static void UnequipAbility(Player player, ushort hotbarSlot)
        {
            using (MySqlConnection conn = new MySqlConnection(
                    String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}",
                    ConfigConstants.DATABASE_HOST,
                    ConfigConstants.DATABASE_PORT,
                    ConfigConstants.DATABASE_NAME,
                    ConfigConstants.DATABASE_USERNAME,
                    ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd;
                    string query = "";
                    
                    query = @"
                                DELETE FROM characters_hotbar
                                WHERE characterId = @charId AND classId = @classId AND hotbarSlot = @hotbarSlot
                        ";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", player.charaWork.parameterSave.state_mainSkill[0]);
                    cmd.Parameters.AddWithValue("@hotbarSlot", hotbarSlot);
                    cmd.ExecuteNonQuery();
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

        public static void LoadHotbar(Player player)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    //Load Hotbar
                    query = @"
                        SELECT 
                        hotbarSlot,
                        commandId,
                        recastTime
                        FROM characters_hotbar WHERE characterId = @charId AND classId = @classId
                        ORDER BY hotbarSlot";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", player.GetCurrentClassOrJob());

                    player.charaWork.commandBorder = 32;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int hotbarSlot = reader.GetUInt16("hotbarSlot");
                            uint commandId = reader.GetUInt32("commandId");
                            player.charaWork.command[hotbarSlot + player.charaWork.commandBorder] = 0xA0F00000 | commandId;
                            player.charaWork.commandCategory[hotbarSlot + player.charaWork.commandBorder] = 1;
                            player.charaWork.parameterSave.commandSlot_recastTime[hotbarSlot] = reader.GetUInt32("recastTime");

                            //Recast timer
                            BattleCommand ability = Server.GetWorldManager().GetBattleCommand((ushort)(commandId));
                            player.charaWork.parameterTemp.maxCommandRecastTime[hotbarSlot] = (ushort) (ability != null ? ability.maxRecastTimeSeconds : 1);
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

        public static ushort FindFirstCommandSlot(Player player, byte classId)
        {
            ushort slot = 0;
            using (MySqlConnection conn = new MySqlConnection(
                String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}",
                ConfigConstants.DATABASE_HOST,
                ConfigConstants.DATABASE_PORT,
                ConfigConstants.DATABASE_NAME,
                ConfigConstants.DATABASE_USERNAME,
                ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd;
                    string query = "";

                    //Drop
                    List<Tuple<ushort, uint>> hotbarList = new List<Tuple<ushort, uint>>();
                    query = @"
                        SELECT hotbarSlot
                        FROM characters_hotbar
                        WHERE characterId = @charId AND classId = @classId
                        ORDER BY hotbarSlot
                        ";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", classId);
                   
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (slot != reader.GetUInt16("hotbarSlot"))
                                break;

                            slot++;
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
            return slot;
        }

        public static List<InventoryItem> GetItemPackage(Character owner, uint slotOffset, uint type)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    serverItemId,
                                    itemId,
                                    server_items_modifiers.id AS modifierId,
                                    quantity,
                                    quality,

                                    dealingValue,
                                    dealingMode,
                                    dealingAttached1,
                                    dealingAttached2,
                                    dealingAttached3,
                                    dealingTag,
                                    bazaarMode,

                                    durability,
                                    mainQuality,
                                    subQuality1,
                                    subQuality2,
                                    subQuality3,
                                    param1,
                                    param2,
                                    param3,
                                    spiritbind,
                                    materia1,
                                    materia2,
                                    materia3,
                                    materia4,
                                    materia5

                                    FROM characters_inventory
                                    INNER JOIN server_items ON serverItemId = server_items.id
                                    LEFT JOIN server_items_modifiers ON server_items.id = server_items_modifiers.id
                                    LEFT JOIN server_items_dealing ON server_items.id = server_items_dealing.id
                                    WHERE characterId = @charId AND itemPackage = @type
                                    ORDER BY slot ASC";                                    

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", owner.actorId);
                    cmd.Parameters.AddWithValue("@type", type);

                    ushort slot = 0;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {                           
                            InventoryItem item = new InventoryItem(reader);
                            items.Add(item);
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

            return items;
        }       

        public static InventoryItem CreateItem(InventoryItem item, uint quantity)
        {
            return CreateItem(item.itemId, (int) quantity, item.quality, item.modifiers);
        }

        public static InventoryItem CreateItem(uint itemId, int quantity, byte quality, InventoryItem.ItemModifier modifiers = null)
        {
            InventoryItem insertedItem = null;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();



                    string query = @"
                                    INSERT INTO server_items                                    
                                    (itemId, quantity, quality)
                                    VALUES
                                    (@itemId, @quantity, @quality);                                    
                                    ";

                    string query2 = @"
                                    INSERT INTO server_items_modifiers                                    
                                    (id, durability)
                                    VALUES
                                    (@id, @durability); 
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@quality", quality);
                    cmd.ExecuteNonQuery();

                    insertedItem = new InventoryItem((uint)cmd.LastInsertedId, itemId, quantity, quality, modifiers);

                    if (modifiers != null)
                    {
                        MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                        cmd2.Parameters.AddWithValue("@id", insertedItem.uniqueId);
                        cmd2.Parameters.AddWithValue("@durability", modifiers.durability);
                        cmd2.ExecuteNonQuery();
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

            return insertedItem;
        }

        public static void AddItem(Character owner, InventoryItem addedItem, ushort itemPackage, ushort slot)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    INSERT INTO characters_inventory
                                    (characterId, itemPackage, serverItemId, slot)
                                    VALUES
                                    (@charId, @itemPackage, @serverItemId, @slot)                                    
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@serverItemId", addedItem.uniqueId);
                    cmd.Parameters.AddWithValue("@charId", owner.actorId);
                    cmd.Parameters.AddWithValue("@itemPackage", itemPackage);
                    cmd.Parameters.AddWithValue("@slot", slot);

                    cmd.ExecuteNonQuery();
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

        public static void RemoveItem(Character owner, ulong serverItemId)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}; Allow User Variables=True", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    DELETE FROM characters_inventory
                                    WHERE characterId = @charId and serverItemId = @serverItemId;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", owner.actorId);
                    cmd.Parameters.AddWithValue("@serverItemId", serverItemId);
                    cmd.ExecuteNonQuery();

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

        public static void UpdateItemPositions(List<InventoryItem> updated)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    UPDATE characters_inventory
                                    SET slot = @slot
                                    WHERE serverItemId = @serverItemId;
                                    ";

                    using (MySqlTransaction trans = conn.BeginTransaction())
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, conn, trans))
                        {
                            foreach (InventoryItem item in updated)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@serverItemId", item.uniqueId);
                                cmd.Parameters.AddWithValue("@slot", item.slot);
                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit();
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

        public static void SetQuantity(ulong serverItemId, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    UPDATE server_items
                                    SET quantity = @quantity
                                    WHERE id = @serverItemId;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@serverItemId", serverItemId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.ExecuteNonQuery();

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

        public static void SetDealingInfo(InventoryItem item)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    REPLACE INTO server_items_dealing
                                    (id, dealingValue, dealingMode, dealingAttached1, dealingAttached2, dealingAttached3, dealingTag, bazaarMode)
                                    VALUES 
                                    (@serverItemId, @dealingValue, @dealingMode, @dealingAttached1, @dealingAttached2, @dealingAttached3, @dealingTag, @bazaarMode);                                  
                                    ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    item.SaveDealingInfo(cmd);
                    cmd.ExecuteNonQuery();
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

        public static void ClearDealingInfo(InventoryItem item)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    DELETE FROM 
                                    server_items_dealing  
                                    WHERE 
                                    id = @serverItemId;
                                    ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@serverItemId", item.uniqueId);
                    cmd.ExecuteNonQuery();
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
       
        public static SubPacket GetLatestAchievements(Player player)
        {
            uint[] latestAchievements = new uint[5];
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    SELECT 
                                    characters_achievements.achievementId FROM characters_achievements 
                                    INNER JOIN gamedata_achievements ON characters_achievements.achievementId = gamedata_achievements.achievementId
                                    WHERE characterId = @charId AND rewardPoints <> 0 AND timeDone IS NOT NULL ORDER BY timeDone LIMIT 5";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);
                            latestAchievements[count++] = id;
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

            return SetLatestAchievementsPacket.BuildPacket(player.actorId, latestAchievements);
        }

        public static SubPacket GetAchievementsPacket(Player player)
        {
            SetCompletedAchievementsPacket cheevosPacket = new SetCompletedAchievementsPacket();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT packetOffsetId 
                                    FROM characters_achievements 
                                    INNER JOIN gamedata_achievements ON characters_achievements.achievementId = gamedata_achievements.achievementId
                                    WHERE characterId = @charId AND timeDone IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint offset = reader.GetUInt32(0);

                            if (offset < 0 || offset >= cheevosPacket.achievementFlags.Length)
                            {
                                Program.Log.Error("SQL Error; achievement flag offset id out of range: " + offset);
                                continue;
                            }
                            cheevosPacket.achievementFlags[offset] = true;
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

            return cheevosPacket.BuildPacket(player.actorId);
        }

        public static SubPacket GetAchievementProgress(Player player, uint achievementId)
        {
            uint progress = 0, progressFlags = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT progress, progressFlags 
                                    FROM characters_achievements 
                                    WHERE characterId = @charId AND achievementId = @achievementId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@achievementId", achievementId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            progress = reader.GetUInt32(0);
                            progressFlags = reader.GetUInt32(1);
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
            return SendAchievementRatePacket.BuildPacket(player.actorId, achievementId, progress, progressFlags);
        }

        public static bool CreateLinkshell(Player player, string lsName, ushort lsCrest)
        {
            bool success = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    INSERT INTO server_linkshells
                                    (name, master, crest)
                                    VALUES
                                    (@lsName, @master, @crest)
                                    ;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@lsName", lsName);
                    cmd.Parameters.AddWithValue("@master", player.actorId);
                    cmd.Parameters.AddWithValue("@crest", lsCrest);

                    cmd.ExecuteNonQuery();
                    success = true;
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
            return success;
        }


        public static void SaveNpcLS(Player player, uint npcLSId, bool isCalling, bool isExtra)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO characters_npclinkshell 
                    (characterId, npcLinkshellId, isCalling, isExtra)
                    VALUES
                    (@charaId, @lsId, @calling, @extra)
                    ON DUPLICATE KEY UPDATE
                    characterId = @charaId, npcLinkshellId = @lsId, isCalling = @calling, isExtra = @extra
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@lsId", npcLSId);
                    cmd.Parameters.AddWithValue("@calling", isCalling ? 1 : 0);
                    cmd.Parameters.AddWithValue("@extra", isExtra ? 1 : 0);

                    cmd.ExecuteNonQuery();
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

        public static bool SaveSupportTicket(GMSupportTicketPacket gmTicket, string playerName)
        {
            string query;
            MySqlCommand cmd;
            bool wasError = false;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO supportdesk_tickets
                    (name, title, body, langCode)
                    VALUES
                    (@name, @title, @body, @langCode)";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.Parameters.AddWithValue("@title", gmTicket.ticketTitle);
                    cmd.Parameters.AddWithValue("@body", gmTicket.ticketBody);
                    cmd.Parameters.AddWithValue("@langCode", gmTicket.langCode);

                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                    wasError = true;
                }
                finally
                {
                    conn.Dispose();
                }
            }

            return wasError;
        }

        public static bool isTicketOpen(string playerName)
        {
            bool isOpen = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    isOpen
                                    FROM supportdesk_tickets
                                    WHERE name = @name
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@name", playerName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            isOpen = reader.GetBoolean(0);
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

            return isOpen;
        }

        public static void closeTicket(string playerName)
        {
            bool isOpen = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    UPDATE
                                    supportdesk_tickets
                                    SET isOpen = 0
                                    WHERE name = @name
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", playerName);
                    cmd.ExecuteNonQuery();
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

        public static string[] getFAQNames(uint langCode = 1)
        {
            string[] faqs = null;
            List<string> raw = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    title
                                    FROM supportdesk_faqs
                                    WHERE languageCode = @langCode
                                    ORDER BY slot
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@langCode", langCode);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string label = reader.GetString(0);
                            raw.Add(label);
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
                    faqs = raw.ToArray();
                }
            }
            return faqs;
        }

        public static string getFAQBody(uint slot, uint langCode = 1)
        {
            string body = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    body
                                    FROM supportdesk_faqs
                                    WHERE slot=@slot and languageCode=@langCode";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@langCode", langCode);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            body = reader.GetString(0);
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
            return body;
        }

        public static string[] getIssues(uint lanCode = 1)
        {
            string[] issues = null;
            List<string> raw = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    title
                                    FROM supportdesk_issues
                                    ORDER BY slot";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string label = reader.GetString(0);
                            raw.Add(label);
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
                    issues = raw.ToArray();
                }
            }
            return issues;
        }

        public static void IssuePlayerChocobo(Player player, byte appearanceId, string name)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO characters_chocobo
                    (characterId, hasChocobo, chocoboAppearance, chocoboName)
                    VALUES
                    (@characterId, @hasChocobo, @chocoboAppearance, @chocoboName)
                    ON DUPLICATE KEY UPDATE
                    hasChocobo=@hasChocobo, chocoboAppearance=@chocoboAppearance, chocoboName=@chocoboName";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@hasChocobo", 1);
                    cmd.Parameters.AddWithValue("@chocoboAppearance", appearanceId);
                    cmd.Parameters.AddWithValue("@chocoboName", name);

                    cmd.ExecuteNonQuery();
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

        public static void ChangePlayerChocoboAppearance(Player player, byte appearanceId)
        {
            string query;
            MySqlCommand cmd;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    UPDATE characters_chocobo
                    SET
                    chocoboAppearance=@chocoboAppearance
                    WHERE
                    characterId = @characterId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@chocoboAppearance", appearanceId);

                    cmd.ExecuteNonQuery();
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
        
        public static Dictionary<uint, StatusEffect> LoadGlobalStatusEffectList()
        {
            var effects = new Dictionary<uint, StatusEffect>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    var query = @"SELECT id, name, flags, overwrite, tickMs, hidden, silentOnGain, silentOnLoss, statusGainTextId, statusLossTextId FROM server_statuseffects;";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetUInt32("id");
                            var name = reader.GetString("name");
                            var flags = reader.GetUInt32("flags");
                            var overwrite = reader.GetByte("overwrite");
                            var tickMs = reader.GetUInt32("tickMs");
                            var hidden = reader.GetBoolean("hidden");
                            var silentOnGain = reader.GetBoolean("silentOnGain");
                            var silentOnLoss = reader.GetBoolean("silentOnLoss");
                            var statusGainTextId = reader.GetUInt16("statusGainTextId");
                            var statusLossTextId = reader.GetUInt16("statusLossTextId");

                            var effect = new StatusEffect(id, name, flags, overwrite, tickMs, hidden, silentOnGain, silentOnLoss, statusGainTextId, statusLossTextId);

                            lua.LuaEngine.LoadStatusEffectScript(effect);
                            effects.Add(id, effect);
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
            return effects;
        }

        public static void SavePlayerStatusEffects(Player player)
        {
            string[] faqs = null;
            List<string> raw = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    using (MySqlTransaction trns = conn.BeginTransaction())
                    {
                        string query = @"
                                    REPLACE INTO characters_statuseffect
                                    (characterId, statusId, magnitude, duration, tick, tier, extra)
                                    VALUES
                                    (@actorId, @statusId, @magnitude, @duration, @tick, @tier, @extra)                                  
                                    ";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn, trns))
                        {    
                            foreach (var effect in player.statusEffects.GetStatusEffects())
                            {
                                var duration = Utils.UnixTimeStampUTC(effect.GetEndTime()) - Utils.UnixTimeStampUTC();

                                cmd.Parameters.AddWithValue("@actorId", player.actorId);
                                cmd.Parameters.AddWithValue("@statusId", effect.GetStatusEffectId());
                                cmd.Parameters.AddWithValue("@magnitude", effect.GetMagnitude());
                                cmd.Parameters.AddWithValue("@duration", duration);
                                cmd.Parameters.AddWithValue("@tick", effect.GetTickMs());
                                cmd.Parameters.AddWithValue("@tier", effect.GetTier());
                                cmd.Parameters.AddWithValue("@extra", effect.GetExtra());

                                cmd.ExecuteNonQuery();
                            }
                            trns.Commit();
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

        public static void LoadGlobalBattleCommandList(Dictionary<ushort, BattleCommand> battleCommandDict, Dictionary<Tuple<byte, short>, List<ushort>> battleCommandIdByLevel)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    int count = 0;
                    conn.Open();

                    var query = ("SELECT `id`, name, classJob, lvl, requirements, mainTarget, validTarget, aoeType, aoeRange, aoeMinRange, aoeConeAngle, aoeRotateAngle, aoeTarget, basePotency, numHits, positionBonus, procRequirement, `range`, minRange, rangeHeight, rangeWidth, statusId, statusDuration, statusChance, " +
                        "castType, castTime, recastTime, mpCost, tpCost, animationType, effectAnimation, modelAnimation, animationDuration, battleAnimation, validUser, comboId1, comboId2, comboStep, accuracyMod, worldMasterTextId, commandType, actionType, actionProperty FROM server_battle_commands;");

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetUInt16("id");
                            var name = reader.GetString("name");
                            var battleCommand = new BattleCommand(id, name);

                            battleCommand.job = reader.GetByte("classJob");
                            battleCommand.level = reader.GetByte("lvl");
                            battleCommand.requirements = (BattleCommandRequirements)reader.GetUInt16("requirements");
                            battleCommand.mainTarget = (ValidTarget)reader.GetUInt16("mainTarget");
                            battleCommand.validTarget = (ValidTarget)reader.GetUInt16("validTarget");
                            battleCommand.aoeType = (TargetFindAOEType)reader.GetByte("aoeType");
                            battleCommand.basePotency = reader.GetUInt16("basePotency");
                            battleCommand.numHits = reader.GetByte("numHits");
                            battleCommand.positionBonus = (BattleCommandPositionBonus)reader.GetByte("positionBonus");
                            battleCommand.procRequirement = (BattleCommandProcRequirement)reader.GetByte("procRequirement");
                            battleCommand.range = reader.GetFloat("range");
                            battleCommand.minRange = reader.GetFloat("minRange");
                            battleCommand.rangeHeight = reader.GetInt32("rangeHeight");
                            battleCommand.rangeWidth = reader.GetInt32("rangeWidth");
                            battleCommand.statusId = reader.GetUInt32("statusId");
                            battleCommand.statusDuration = reader.GetUInt32("statusDuration");
                            battleCommand.statusChance = reader.GetFloat("statusChance");
                            battleCommand.castType = reader.GetByte("castType");
                            battleCommand.castTimeMs = reader.GetUInt32("castTime");
                            battleCommand.maxRecastTimeSeconds = reader.GetUInt32("recastTime");
                            battleCommand.recastTimeMs = battleCommand.maxRecastTimeSeconds * 1000;
                            battleCommand.mpCost = reader.GetInt16("mpCost");
                            battleCommand.tpCost = reader.GetInt16("tpCost");
                            battleCommand.animationType = reader.GetByte("animationType");
                            battleCommand.effectAnimation = reader.GetUInt16("effectAnimation");
                            battleCommand.modelAnimation = reader.GetUInt16("modelAnimation");
                            battleCommand.animationDurationSeconds = reader.GetUInt16("animationDuration");
                            battleCommand.aoeRange = reader.GetFloat("aoeRange");
                            battleCommand.aoeMinRange = reader.GetFloat("aoeMinRange");
                            battleCommand.aoeConeAngle = reader.GetFloat("aoeConeAngle");
                            battleCommand.aoeRotateAngle = reader.GetFloat("aoeRotateAngle");
                            battleCommand.aoeTarget = (TargetFindAOETarget)reader.GetByte("aoeTarget");

                            battleCommand.battleAnimation = reader.GetUInt32("battleAnimation");
                            battleCommand.validUser = (BattleCommandValidUser)reader.GetByte("validUser");

                            battleCommand.comboNextCommandId[0] = reader.GetInt32("comboId1");
                            battleCommand.comboNextCommandId[1] = reader.GetInt32("comboId2");
                            battleCommand.comboStep = reader.GetInt16("comboStep");
                            battleCommand.commandType = (CommandType) reader.GetInt16("commandType");
                            battleCommand.actionProperty = (ActionProperty)reader.GetInt16("actionProperty");
                            battleCommand.actionType = (ActionType)reader.GetInt16("actionType");
                            battleCommand.accuracyModifier = reader.GetFloat("accuracyMod");
                            battleCommand.worldMasterTextId = reader.GetUInt16("worldMasterTextId");

                            string folderName = "";

                            switch (battleCommand.commandType)
                            {
                                case CommandType.AutoAttack:
                                    folderName = "autoattack";
                                    break;
                                case CommandType.WeaponSkill:
                                    folderName = "weaponskill";
                                    break;
                                case CommandType.Ability:
                                    folderName = "ability";
                                    break;
                                case CommandType.Spell:
                                    folderName = "magic";
                                    break;
                            }

                            lua.LuaEngine.LoadBattleCommandScript(battleCommand, folderName);
                            battleCommandDict.Add(id, battleCommand);

                            Tuple<byte, short> tuple = Tuple.Create<byte, short>(battleCommand.job, battleCommand.level);
                            if (battleCommandIdByLevel.ContainsKey(tuple))
                            {
                                battleCommandIdByLevel[tuple].Add(id);
                            }
                            else
                            {
                                List<ushort> list = new List<ushort>() { id };
                                battleCommandIdByLevel.Add(tuple, list);
                            }
                            count++;
                        }
                    }

                    Program.Log.Info(String.Format("Loaded {0} battle commands.", count));
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

        public static void LoadGlobalBattleTraitList(Dictionary<ushort, BattleTrait> battleTraitDict, Dictionary<byte, List<ushort>> battleTraitJobDict)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    int count = 0;
                    conn.Open();

                    var query = ("SELECT `id`, name, classJob, lvl, modifier, bonus FROM server_battle_traits;");

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetUInt16("id");
                            var name = reader.GetString("name");
                            var job = reader.GetByte("classJob");
                            var level = reader.GetByte("lvl");
                            uint modifier = reader.GetUInt32("modifier");
                            var bonus = reader.GetInt32("bonus");

                            var trait = new BattleTrait(id, name, job, level, modifier, bonus);

                            battleTraitDict.Add(id, trait);

                            if(battleTraitJobDict.ContainsKey(job))
                            {
                                battleTraitJobDict[job].Add(id);
                            }
                            else
                            {
                                battleTraitJobDict[job] = new List<ushort>();
                                battleTraitJobDict[job].Add(id);
                            }

                            count++;
                        }
                    }
                    Program.Log.Info(String.Format("Loaded {0} battle traits.", count));
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

        public static void SetExp(Player player, byte classId, int exp)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    var query = String.Format(@"
                    UPDATE characters_class_exp
                    SET
                    {0} = @exp
                    WHERE
                    characterId = @characterId", CharacterUtils.GetClassNameForId(classId));
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Prepare();
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@exp", exp);
                    cmd.ExecuteNonQuery();
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

        public static void SetLevel(Player player, byte classId, short level)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    var query = String.Format(@"
                    UPDATE characters_class_levels
                    SET
                    {0} = @lvl
                    WHERE
                    characterId = @characterId", CharacterUtils.GetClassNameForId(classId));
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Prepare();
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@lvl", level);
                    cmd.ExecuteNonQuery();
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

        public static Retainer LoadRetainer(Player player, int retainerIndex)
        {
            Retainer retainer = null;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT server_retainers.id as retainerId, server_retainers.name as name, actorClassId FROM characters_retainers                                    
                                    INNER JOIN server_retainers ON characters_retainers.retainerId = server_retainers.id
                                    WHERE characterId = @charaId
                                    ORDER BY id
                                    LIMIT 1 OFFSET @retainerIndex
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", player.actorId);
                    cmd.Parameters.AddWithValue("@retainerIndex", retainerIndex - 1);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint retainerId = reader.GetUInt32("retainerId");
                            string name = reader.GetString("name");
                            uint actorClassId = reader.GetUInt32("actorClassId");

                            ActorClass actorClass = Server.GetWorldManager().GetActorClass(actorClassId);

                            retainer = new Retainer(retainerId, actorClass, player, 0, 0, 0, 0);
                            retainer.customDisplayName = name;
                            retainer.LoadEventConditions(actorClass.eventConditions);
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

                return retainer;
            }
        }

        public static void PlayerCharacterUpdateClassLevel(Player player, byte classId, short level)
        {
            string query;
            MySqlCommand cmd;

            string[] classNames = {
                "",
                "",
                "pug",
                "gla",
                "mrd",
                "",
                "",
                "arc",
                "lnc",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "thm",
                "cnj",
                "",
                "",
                "",
                "",
                "",
                "crp",
                "bsm",
                "arm",
                "gsm",
                "ltw",
                "wvr",
                "alc",
                "cul",
                "",
                "",
                "min",
                "btn",
                "fsh"
            };
            
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = String.Format(@"
                    UPDATE characters_class_levels
                    SET
                    {0}=@level
                    WHERE
                    characterId = @characterId", classNames[classId]);

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@level", level);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);

                    cmd.ExecuteNonQuery();
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

    }
}

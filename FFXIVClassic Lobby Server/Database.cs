using FFXIVClassic_Lobby_Server.dataobjects;
using MySql.Data.MySqlClient;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.utils;

namespace FFXIVClassic_Lobby_Server
{
    //charState: 0 - Reserved, 1 - Inactive, 2 - Active

    class Database
    {
        public static uint getUserIdFromSession(String sessionId)
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
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }                
            }
            return id;
        }

        public static bool reserveCharacter(uint userId, uint slot, uint serverId, String name, out uint pid, out uint cid)
        {
            bool alreadyExists = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {                    
                    conn.Open();

                    //Check if exists                    
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM characters WHERE name=@name AND serverId=@serverId AND state != 2 AND state != 1", conn);
                    cmd.Parameters.AddWithValue("@serverId", serverId);
                    cmd.Parameters.AddWithValue("@name", name);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            alreadyExists = true;                            
                        }
                    }

                    //Reserve
                    if (!alreadyExists)
                    {
                        MySqlCommand cmd2 = new MySqlCommand();
                        cmd2.Connection = conn;
                        cmd2.CommandText = "INSERT INTO characters(userId, slot, serverId, name, state) VALUES(@userId, @slot, @serverId, @name, 0)";
                        cmd2.Prepare();
                        cmd2.Parameters.AddWithValue("@userId", userId);
                        cmd2.Parameters.AddWithValue("@slot", slot);
                        cmd2.Parameters.AddWithValue("@serverId", serverId);
                        cmd2.Parameters.AddWithValue("@name", name);
                        cmd2.ExecuteNonQuery();
                        cid = (ushort)cmd2.LastInsertedId;
                        pid = 0xBABE;
                    }
                    else
                    {
                        pid = 0;
                        cid = 0;
                    }
                }
                catch (MySqlException e)
                {
                    pid = 0;
                    cid = 0;
                }
                finally
                {
                    conn.Dispose();
                }

                Log.database(String.Format("CID={0} created on 'characters' table.", cid));
            }

            return alreadyExists;
        }        

        public static void makeCharacter(uint accountId, uint cid, CharaInfo charaInfo)
        {
            //Update character entry
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = @"
                                        UPDATE characters SET 
                                        state=2,
                                        currentZoneId=@zoneId,
                                        positionX=@x,
                                        positionY=@y,
                                        positionZ=@z,
                                        rotation=@r,
                                        guardian=@guardian,
                                        birthDay=@birthDay,
                                        birthMonth=@birthMonth,
                                        initialTown=@initialTown,
                                        tribe=@tribe
                                        WHERE userId=@userId AND id=@cid;
            
                                        INSERT INTO characters_appearance
                                        (characterId, baseId, size, voice, skinColor, hairStyle, hairColor, hairHighlightColor, hairVariation, eyeColor, faceType, faceEyebrows, faceEyeShape, faceIrisSize, faceNose, faceMouth, faceFeatures, ears, characteristics, characteristicsColor, mainhand, offhand, head, body, hands, legs, feet, waist)
                                        VALUES
                                        (@cid, 4294967295, @size, @voice, @skinColor, @hairStyle, @hairColor, @hairHighlightColor, @hairVariation, @eyeColor, @faceType, @faceEyebrows, @faceEyeShape, @faceIrisSize, @faceNose, @faceMouth, @faceFeatures, @ears, @characteristics, @characteristicsColor, @mainhand, @offhand, @head, @body, @hands, @legs, @feet, @waist)
                                        ";
                    cmd.Parameters.AddWithValue("@userId", accountId);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.Parameters.AddWithValue("@guardian", charaInfo.guardian);
                    cmd.Parameters.AddWithValue("@birthDay", charaInfo.birthDay);
                    cmd.Parameters.AddWithValue("@birthMonth", charaInfo.birthMonth);
                    cmd.Parameters.AddWithValue("@initialTown", charaInfo.initialTown);
                    cmd.Parameters.AddWithValue("@tribe", charaInfo.tribe);

                    cmd.Parameters.AddWithValue("@zoneId", charaInfo.zoneId);
                    cmd.Parameters.AddWithValue("@x", charaInfo.x);
                    cmd.Parameters.AddWithValue("@y", charaInfo.y);
                    cmd.Parameters.AddWithValue("@z", charaInfo.z);
                    cmd.Parameters.AddWithValue("@r", charaInfo.rot);

                    cmd.Parameters.AddWithValue("@size", charaInfo.appearance.size);
                    cmd.Parameters.AddWithValue("@voice", charaInfo.appearance.voice);
                    cmd.Parameters.AddWithValue("@skinColor", charaInfo.appearance.skinColor);
                    cmd.Parameters.AddWithValue("@hairStyle", charaInfo.appearance.hairStyle);
                    cmd.Parameters.AddWithValue("@hairColor", charaInfo.appearance.hairColor);
                    cmd.Parameters.AddWithValue("@hairHighlightColor", charaInfo.appearance.hairHighlightColor);
                    cmd.Parameters.AddWithValue("@hairVariation", charaInfo.appearance.hairVariation);
                    cmd.Parameters.AddWithValue("@eyeColor", charaInfo.appearance.eyeColor);
                    cmd.Parameters.AddWithValue("@faceType", charaInfo.appearance.faceType);
                    cmd.Parameters.AddWithValue("@faceEyebrows", charaInfo.appearance.faceEyebrows);
                    cmd.Parameters.AddWithValue("@faceEyeShape", charaInfo.appearance.faceEyeShape);
                    cmd.Parameters.AddWithValue("@faceIrisSize", charaInfo.appearance.faceIrisSize);
                    cmd.Parameters.AddWithValue("@faceNose", charaInfo.appearance.faceNose);
                    cmd.Parameters.AddWithValue("@faceMouth", charaInfo.appearance.faceMouth);
                    cmd.Parameters.AddWithValue("@faceFeatures", charaInfo.appearance.faceFeatures);
                    cmd.Parameters.AddWithValue("@ears", charaInfo.appearance.ears);
                    cmd.Parameters.AddWithValue("@characteristics", charaInfo.appearance.characteristics);
                    cmd.Parameters.AddWithValue("@characteristicsColor", charaInfo.appearance.characteristicsColor);

                    cmd.Parameters.AddWithValue("@mainhand", charaInfo.weapon1);
                    cmd.Parameters.AddWithValue("@offhand", charaInfo.weapon2);
                    cmd.Parameters.AddWithValue("@head", charaInfo.head);
                    cmd.Parameters.AddWithValue("@body", charaInfo.body);
                    cmd.Parameters.AddWithValue("@legs", charaInfo.legs);
                    cmd.Parameters.AddWithValue("@hands", charaInfo.hands);
                    cmd.Parameters.AddWithValue("@feet", charaInfo.feet);
                    cmd.Parameters.AddWithValue("@waist", charaInfo.belt);

                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {
                    conn.Dispose();
                    return;
                }
                finally
                {
                }


                //Create Level and EXP entries
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = String.Format("INSERT INTO characters_class_levels(characterId, {0}) VALUES(@characterId, 1); INSERT INTO characters_class_exp(characterId) VALUES(@characterId)", CharacterCreatorUtils.GetClassNameForId((short)charaInfo.currentClass));
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@characterId", cid);

                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {
                    conn.Dispose();
                    return;
                }

                //Create Parameter Save
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = String.Format("INSERT INTO characters_parametersave(characterId, hp, hpMax, mp, mpMax, mainSkill, mainSkillLevel) VALUES(@characterId, 1, 1, 1, 1, @mainSkill, 1);", CharacterCreatorUtils.GetClassNameForId((short)charaInfo.currentClass));
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@characterId", cid);
                    cmd.Parameters.AddWithValue("@mainSkill", charaInfo.currentClass);

                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Dispose();
                }

                
            }

            Log.database(String.Format("CID={0} state updated to active(2).", cid));
        }

        public static bool renameCharacter(uint userId, uint characterId, uint serverId, String newName)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Check if exists                    
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM characters WHERE name=@name AND serverId=@serverId", conn);
                    cmd.Parameters.AddWithValue("@serverId", serverId);
                    cmd.Parameters.AddWithValue("@name", newName);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            return true;
                        }
                    }

                    cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE characters SET name=@name, doRename=0 WHERE id=@cid AND userId=@uid";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@cid", characterId);
                    cmd.Parameters.AddWithValue("@name", newName);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Dispose();
                }

                Log.database(String.Format("CID={0} name updated to \"{1}\".", characterId, newName));

                return false;
            }
        }

        public static void deleteCharacter(uint characterId, String name)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE characters SET state=1 WHERE id=@cid AND name=@name";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@cid", characterId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();                    

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Dispose();
                }
            }

            Log.database(String.Format("CID={0} deleted.", characterId));
        }

        public static List<World> getServers()
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<World> worldList = null;
                try
                {
                    conn.Open();
                    worldList = conn.Query<World>("SELECT * FROM servers WHERE isActive=true").ToList();                                       
                }
                catch (MySqlException e)
                { worldList = new List<World>(); }
                finally
                {                    
                    conn.Dispose();
                }
                return worldList;
            }
        }

        public static World getServer(uint serverId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                World world = null;
                try
                {
                    conn.Open();
                    world = conn.Query<World>("SELECT * FROM servers WHERE id=@ServerId", new {ServerId = serverId}).SingleOrDefault();                  
                }
                catch (MySqlException e)
                {                    
                }
                finally
                {
                    conn.Dispose();
                }

                return world;
            }
        }

        public static List<Character> getCharacters(uint userId)
        {
            List<Character> characters = new List<Character>();
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                conn.Open();

                //Load basic info                  
                string query = @"
                    SELECT 
                    id,            
                    slot,
                    serverId,
                    name,
                    isLegacy,
                    doRename,
                    currentZoneId,             
                    guardian,
                    birthMonth,
                    birthDay,
                    initialTown,
                    tribe,
                    mainSkill,
                    mainSkillLevel
                    FROM characters
                    INNER JOIN characters_parametersave ON id = characters_parametersave.characterId
                    WHERE userId = @userId AND state = 2
                    ORDER BY slot";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Character chara = new Character();
                        chara.id = reader.GetUInt32("id");
                        chara.slot = reader.GetUInt16("slot");
                        chara.serverId = reader.GetUInt16("serverId");
                        chara.name = reader.GetString("name");
                        chara.isLegacy = reader.GetBoolean("isLegacy");
                        chara.doRename = reader.GetBoolean("doRename");
                        chara.currentZoneId = reader.GetUInt32("currentZoneId");
                        chara.guardian = reader.GetByte("guardian");
                        chara.birthMonth = reader.GetByte("birthMonth");
                        chara.birthDay = reader.GetByte("birthDay");
                        chara.initialTown = reader.GetByte("initialTown");
                        chara.tribe = reader.GetByte("tribe");
                        chara.currentClass = reader.GetByte("mainSkill");
                        //chara.currentJob = ???
                        chara.currentLevel = reader.GetInt16("mainSkillLevel");
                        characters.Add(chara);
                    }
                }

            }
            return characters;
        }

        public static Character getCharacter(uint userId, uint charId)
        {
            Character chara = null;
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                conn.Open();

                string query = @"
                    SELECT 
                    id,            
                    slot,
                    serverId,
                    name,
                    isLegacy,
                    doRename,
                    currentZoneId,             
                    guardian,
                    birthMonth,
                    birthDay,
                    initialTown,
                    tribe,
                    mainSkill,
                    mainSkillLevel
                    FROM characters
                    INNER JOIN characters_parametersave ON id = characters_parametersave.characterId
                    WHERE id = @charId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@charId", charId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        chara = new Character();
                        chara.id = reader.GetUInt32("id");
                        chara.slot = reader.GetUInt16("slot");
                        chara.serverId = reader.GetUInt16("serverId");
                        chara.name = reader.GetString("name");
                        chara.isLegacy = reader.GetBoolean("isLegacy");
                        chara.doRename = reader.GetBoolean("doRename");
                        chara.currentZoneId = reader.GetUInt32("currentZoneId");
                        chara.guardian = reader.GetByte("guardian");
                        chara.birthMonth = reader.GetByte("birthMonth");
                        chara.birthDay = reader.GetByte("birthDay");
                        chara.initialTown = reader.GetByte("initialTown");
                        chara.tribe = reader.GetByte("tribe");
                        chara.currentClass = reader.GetByte("mainSkill");
                        //chara.currentJob = ???
                        chara.currentLevel = reader.GetInt16("mainSkillLevel");
                    }
                }
            }
            return chara;
        }

        public static Appearance getAppearance(uint charaId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                Appearance appearance = null;
                try
                {
                    conn.Open();
                    appearance = conn.Query<Appearance>("SELECT * FROM characters_appearance WHERE characterId=@CharaId", new { CharaId = charaId }).SingleOrDefault();
                }
                catch (MySqlException e)
                {
                }
                finally
                {
                    conn.Dispose();
                }

                return appearance;
            }
        }

        public static List<String> getReservedNames(uint userId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<String> nameList = null;
                try
                {
                    conn.Open();
                    nameList = conn.Query<String>("SELECT name FROM reserved_names WHERE userId=@UserId", new { UserId = userId }).ToList();
                }
                catch (MySqlException e)
                { nameList = new List<String>(); }
                finally
                {
                    conn.Dispose();
                }
                return nameList;
            }
        }

        public static List<Retainer> getRetainers(uint userId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<Retainer> retainerList = null;
                try
                {
                    conn.Open();
                    retainerList = conn.Query<Retainer>("SELECT * FROM retainers WHERE id=@UserId ORDER BY characterId, slot", new { UserId = userId }).ToList();
                }
                catch (MySqlException e)
                { retainerList = new List<Retainer>(); }
                finally
                {
                    conn.Dispose();
                }
                return retainerList;
            }
        }

    }
}

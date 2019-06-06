using FFXIVClassic_Lobby_Server.dataobjects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using FFXIVClassic_Lobby_Server.utils;

namespace FFXIVClassic_Lobby_Server
{
    //charState: 0 - Reserved, 1 - Inactive, 2 - Active

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

        public static bool ReserveCharacter(uint userId, uint slot, uint serverId, String name, out uint pid, out uint cid)
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
                    Program.Log.Error(e.ToString());
                   
                    Program.Log.Error(e.ToString());
                   
                    pid = 0;
                    cid = 0;
                }
                finally
                {
                    conn.Dispose();
                }

                Program.Log.Debug("[SQL] CID={0} Created on 'characters' table.", cid);
            }

            return alreadyExists;
        }        

        public static void MakeCharacter(uint accountId, uint cid, CharaInfo charaInfo)
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
                    Program.Log.Error(e.ToString());
                   
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
                    Program.Log.Error(e.ToString());
                   
                    conn.Dispose();
                    return;
                }

                //Create Parameter Save
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = String.Format("INSERT INTO characters_parametersave(characterId, hp, hpMax, mp, mpMax, mainSkill, mainSkillLevel) VALUES(@characterId, 1900, 1000, 115, 115, @mainSkill, 1);", CharacterCreatorUtils.GetClassNameForId((short)charaInfo.currentClass));
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@characterId", cid);
                    cmd.Parameters.AddWithValue("@mainSkill", charaInfo.currentClass);

                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                    conn.Dispose();
                    return;
                }

                //Create Hotbar
                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT id FROM server_battle_commands WHERE classJob = @classjob AND lvl = 1 ORDER BY id DESC";
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@classJob", charaInfo.currentClass);
                    List<uint> defaultActions = new List<uint>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            defaultActions.Add(reader.GetUInt32("id"));
                        }
                    }
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = conn;
                    cmd2.CommandText = "INSERT INTO characters_hotbar (characterId, classId, hotbarSlot, commandId, recastTime) VALUES (@characterId, @classId, @hotbarSlot, @commandId, 0)";
                    cmd2.Prepare();
                    cmd2.Parameters.AddWithValue("@characterId", cid);
                    cmd2.Parameters.AddWithValue("@classId", charaInfo.currentClass);
                    cmd2.Parameters.Add("@hotbarSlot", MySqlDbType.Int16);
                    cmd2.Parameters.Add("@commandId", MySqlDbType.Int16);

                    for(int i = 0; i < defaultActions.Count; i++)
                    {
                        cmd2.Parameters["@hotbarSlot"].Value = i;
                        cmd2.Parameters["@commandId"].Value = defaultActions[i];
                        cmd2.ExecuteNonQuery();
                    }
                }
                catch(MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                }
                finally
                {
                    conn.Dispose();
                }
            }

            Program.Log.Debug("[SQL] CID={0} state updated to active(2).", cid);
        }

        public static bool RenameCharacter(uint userId, uint characterId, uint serverId, String newName)
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
                    cmd.CommandText = "UPDATE characters SET name=@name, DoRename=0 WHERE id=@cid AND userId=@uid";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@cid", characterId);
                    cmd.Parameters.AddWithValue("@name", newName);
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

                Program.Log.Debug("[SQL] CID={0} name updated to \"{1}\".", characterId, newName);

                return false;
            }
        }

        public static void DeleteCharacter(uint characterId, String name)
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
                    Program.Log.Error(e.ToString());
                   

                }
                finally
                {
                    conn.Dispose();
                }
            }

            Program.Log.Debug("[SQL] CID={0} deleted.", characterId);
        }

        public static List<World> GetServers()
        {
            string query;
            MySqlCommand cmd;
            List<World> worldList = new List<World>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    query = "SELECT * FROM servers WHERE isActive=true";
                    cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ushort id;
                            string address;
                            ushort port;
                            ushort listPosition;
                            ushort population;
                            string name;
                            bool isActive;

                            id = reader.GetUInt16("id");
                            address = reader.GetString("address");
                            port = reader.GetUInt16("port");
                            listPosition = reader.GetUInt16("listPosition");
                            population = 2;
                            name = reader.GetString("name");
                            isActive = reader.GetBoolean("isActive");

                            worldList.Add(new World(id, address, port, listPosition, population, name, isActive));
                        }
                    }
                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                    worldList = new List<World>();
                }
                finally
                {
                    conn.Dispose();
                }
            }
            return worldList;
        }

        public static World GetServer(uint serverId)
        {
            string query;
            MySqlCommand cmd;
            World world = null;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    query = "SELECT * FROM servers WHERE id=@ServerId";
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ServerId", serverId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ushort id;
                            string address;
                            ushort port;
                            ushort listPosition;
                            ushort population;
                            string name;
                            bool isActive;

                            id = reader.GetUInt16("id");
                            address = reader.GetString("address");
                            port = reader.GetUInt16("port");
                            listPosition = reader.GetUInt16("listPosition");
                            population = 2; //TODO
                            name = reader.GetString("name");
                            isActive = reader.GetBoolean("isActive");

                            world = new World(id, address, port, listPosition, population, name, isActive);
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

            return world;           
        }

        public static List<Character> GetCharacters(uint userId)
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

        public static Character GetCharacter(uint userId, uint charId)
        {
            Character chara = null;
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
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
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());

                }
                finally
                {
                    conn.Dispose();
                }
            }
            return chara;
        }

        public static Appearance GetAppearance(uint charaId)
        {
            Appearance appearance = new Appearance();
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    //Load appearance
                    string query = @"
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
                            FROM characters_appearance WHERE characterId = @charaId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charaId", charaId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            appearance.size = reader.GetByte("size");
                            appearance.voice = reader.GetByte("voice");
                            appearance.skinColor = reader.GetUInt16("skinColor");
                            appearance.hairStyle = reader.GetUInt16("hairStyle");
                            appearance.hairColor = reader.GetUInt16("hairColor");
                            appearance.hairHighlightColor = reader.GetUInt16("hairHighlightColor");
                            appearance.eyeColor = reader.GetUInt16("eyeColor");
                            appearance.characteristics = reader.GetByte("characteristics");
                            appearance.characteristicsColor = reader.GetByte("characteristicsColor");
                            appearance.faceType = reader.GetByte("faceType");
                            appearance.ears = reader.GetByte("ears");
                            appearance.faceMouth = reader.GetByte("faceMouth");
                            appearance.faceFeatures = reader.GetByte("faceFeatures");
                            appearance.faceNose = reader.GetByte("faceNose");
                            appearance.faceEyeShape = reader.GetByte("faceEyeShape");
                            appearance.faceIrisSize = reader.GetByte("faceIrisSize");
                            appearance.faceEyebrows = reader.GetByte("faceEyebrows");

                            appearance.mainHand = reader.GetUInt32("mainHand");
                            appearance.offHand = reader.GetUInt32("offHand");
                            appearance.head = reader.GetUInt32("head");
                            appearance.body = reader.GetUInt32("body");
                            appearance.mainHand = reader.GetUInt32("mainHand");
                            appearance.legs = reader.GetUInt32("legs");
                            appearance.hands = reader.GetUInt32("hands");
                            appearance.feet = reader.GetUInt32("feet");
                            appearance.waist = reader.GetUInt32("waist");
                            appearance.leftFinger = reader.GetUInt32("leftFinger");
                            appearance.rightFinger = reader.GetUInt32("rightFinger");
                            appearance.leftEar = reader.GetUInt32("leftEar");
                            appearance.rightEar = reader.GetUInt32("rightEar");
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

            return appearance;
        }

        public static List<String> GetReservedNames(uint userId)
        {
            List<String> reservedNames = new List<String>();
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT name FROM reserved_names WHERE userId=@UserId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservedNames.Add(reader.GetString("name"));
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
            return reservedNames;
        }

        public static List<Retainer> GetRetainers(uint userId)
        {
            return new List<Retainer>();
        }

    }
}

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
using FFXIVClassic_Map_Server.dataobjects.database;
using FFXIVClassic_Map_Server.dataobjects.chara.npc;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Lobby_Server
{

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
     
        public static DBWorld getServer(uint serverId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBWorld world = null;
                try
                {
                    conn.Open();
                    world = conn.Query<DBWorld>("SELECT * FROM servers WHERE id=@ServerId", new {ServerId = serverId}).SingleOrDefault();                  
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

        public static DBCharacter getCharacter(uint charId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBCharacter chara = null;
                try
                {
                    conn.Open();
                    chara = conn.Query<DBCharacter>("SELECT * FROM characters WHERE id=@CharaId", new { CharaId = charId }).SingleOrDefault();
                }
                catch (MySqlException e)
                {
                }
                finally
                {
                    conn.Dispose();
                }

                return chara;
            }
        }

        public static DBAppearance getAppearance(bool loadFromPlayerTable, uint charaId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBAppearance appearance = null;
                try
                {
                    conn.Open();

                    if (loadFromPlayerTable)
                        appearance = conn.Query<DBAppearance>("SELECT * FROM characters_appearance WHERE characterId=@CharaId", new { CharaId = charaId }).SingleOrDefault();
                    else
                        appearance = conn.Query<DBAppearance>("SELECT * FROM npc_appearance WHERE npcId=@CharaId", new { CharaId = charaId }).SingleOrDefault();
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

        public static DBStats getCharacterStats(uint charaId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBStats stats = null;
                try
                {
                    conn.Open();
                    stats = conn.Query<DBStats>("SELECT * FROM characters_stats WHERE characterId=@CharaId", new { CharaId = charaId }).SingleOrDefault();
                }
                catch (MySqlException e)
                {
                }
                finally
                {
                    conn.Dispose();
                }

                return stats;
            }
        }

        public static List<Npc> getNpcList()
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<Npc> npcList = null;
                try
                {
                    conn.Open();
                    npcList = conn.Query<Npc>("SELECT * FROM npc_list").ToList();
                }
                catch (MySqlException e)
                {
                }
                finally
                {
                    conn.Dispose();
                }

                return npcList;
            }
        }

        public static void loadPlayerCharacter(Player player, bool isMe)
        {
            //Load basic info
            string query;
            MySqlCommand cmd;            

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    if (isMe)
                    {
                        query = @"
                        SELECT 
                        name,
                        positionX,
                        positionY,
                        positionZ,
                        rotation,
                        actorState,
                        currentZoneId,
                        currentClassJob,                
                        gcCurrent,
                        gcLimsaRank,
                        gcGridaniaRank,
                        gcUldahRank,
                        currentTitle,
                        guardian,
                        birthDay,
                        birthMonth,
                        initialNation,
                        currentParty,
                        restBonus,
                        achievementPoints
                        FROM characters WHERE id = @charId";
                    }
                    else
                    {
                        query = @"
                        SELECT 
                        name,
                        positionX,
                        positionY,
                        positionZ,
                        rotation,
                        actorState,
                        currentZoneId,
                        currentClassJob,
                        gcCurrent,          
                        gcLimsaRank,
                        gcGridaniaRank,
                        gcUldahRank,
                        currentTitle
                        FROM characters WHERE id = @charId";
                    }

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        player.displayNameId = 0xFFFFFFFF;
                        player.customDisplayName = reader.GetString(0);
                        player.oldPositionX = player.positionX = reader.GetFloat(1);
                        player.oldPositionY = player.positionY = reader.GetFloat(2);
                        player.oldPositionZ = player.positionZ = reader.GetFloat(3);
                        player.oldRotation = player.rotation = reader.GetFloat(4);
                        player.currentMainState = reader.GetUInt16(5);
                        player.currentZoneId = reader.GetUInt32(6);
                        player.charaWork.parameterSave.state_mainSkill[0] = reader.GetByte(7);
                        player.gcCurrent = reader.GetByte(8);
                        player.gcRankLimsa = reader.GetByte(9);
                        player.gcRankGridania = reader.GetByte(10);
                        player.gcRankUldah = reader.GetByte(11);
                        player.currentTitle = reader.GetUInt32(12);

                        if (isMe)
                        {
                            player.playerWork.guardian = reader.GetByte(13);
                            player.playerWork.birthdayDay = reader.GetByte(14);
                            player.playerWork.birthdayMonth = reader.GetByte(15);
                            player.playerWork.initialTown = reader.GetByte(16);
                            player.playerWork.restBonusExpRate = reader.GetInt32(17);
                            player.achievementPoints = reader.GetUInt32(18);
                        }
                    }

                    //Load appearance
                    query = @"
                        SELECT 
                        baseId,
                        tribe,
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
                        hands,
                        legs,
                        feet,
                        waist,
                        leftFinger,
                        rightFinger,
                        leftEars,
                        rightEars
                        FROM characters_appearance WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        player.modelID = DBAppearance.getTribeModel(reader.GetByte(1));
                        player.appearanceIDs[Character.SIZE] = reader.GetByte(2);
                        player.appearanceIDs[Character.COLORINFO] = (uint)(reader.GetUInt16(4) | (reader.GetUInt16(6) << 10) | (reader.GetUInt16(8) << 20));
                        player.appearanceIDs[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.getFaceInfo(reader.GetByte(9), reader.GetByte(10), reader.GetByte(11), reader.GetByte(12), reader.GetByte(13), reader.GetByte(14), reader.GetByte(15), reader.GetByte(16), reader.GetByte(17), reader.GetByte(18)));
                        player.appearanceIDs[Character.HIGHLIGHT_HAIR] = (uint)(reader.GetUInt16(7) | reader.GetUInt16(5) << 10);
                        player.appearanceIDs[Character.VOICE] = reader.GetByte(3);
                        player.appearanceIDs[Character.WEAPON1] = reader.GetUInt32(19);
                        player.appearanceIDs[Character.WEAPON2] = reader.GetUInt32(20);
                        player.appearanceIDs[Character.HEADGEAR] = reader.GetUInt32(21);
                        player.appearanceIDs[Character.BODYGEAR] = reader.GetUInt32(22);
                        player.appearanceIDs[Character.LEGSGEAR] = reader.GetUInt32(23);
                        player.appearanceIDs[Character.HANDSGEAR] = reader.GetUInt32(24);
                        player.appearanceIDs[Character.FEETGEAR] = reader.GetUInt32(25);
                        player.appearanceIDs[Character.WAISTGEAR] = reader.GetUInt32(26);
                        player.appearanceIDs[Character.R_EAR] = reader.GetUInt32(27);
                        player.appearanceIDs[Character.L_EAR] = reader.GetUInt32(28);
                        player.appearanceIDs[Character.R_FINGER] = reader.GetUInt32(29);
                        player.appearanceIDs[Character.L_FINGER] = reader.GetUInt32(30);
                    }

                    //Load Status Effects
                    query = @"
                        SELECT 
                        statusId,
                        expireTime                     
                        FROM characters_statuseffect WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            player.charaWork.status[count] = reader.GetUInt16(0);
                            player.charaWork.statusShownTime[count] = reader.GetUInt32(1);
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
                        player.hasChocobo = reader.GetBoolean(0);
                        player.hasGoobbue = reader.GetBoolean(1);
                        player.chocoboAppearance = reader.GetByte(2);
                        player.chocoboName = reader.GetString(3);
                    }

                    //Load Achievements
                    query = @"
                        SELECT 
                        achievementId                                          
                        FROM characters_achievements WHERE characterId = @charId AND timeDone NOT NULL";

                    //Load Last 5 Completed
                    query = @"
                        SELECT 
                        achievementId                                     
                        FROM characters_achievements WHERE characterId = @charId ORDER BY timeDone DESC LIMIT 5";

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
                        skirmish,
                        FROM characters_timers WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        for (int i = 0; i < player.timers.Length; i++)
                            player.timers[i] = reader.GetUInt32(i);
                    }
                   
                    //Load Hotbar
                    query = @"
                        SELECT 
                        hotbarIndex,
                        commandId,
                        recastTime                
                        FROM characters_hotbar WHERE characterId = @charId AND classId = @classId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    //cmd.Parameters.AddWithValue("@classId", player.currentClassId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {                        
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.charaWork.command[index+32] = reader.GetUInt32(1);
                            player.charaWork.parameterSave.commandSlot_recastTime[index] = reader.GetUInt32(2);
                        }
                    }

                    //Load Scenario Quests
                    query = @"
                        SELECT 
                        index,
                        questId                
                        FROM characters_quest_scenario WHERE characterId = @charId";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questScenario[index] = reader.GetUInt32(1);
                        }
                    }

                    //Load Guildleve Quests
                    query = @"
                        SELECT 
                        index,
                        questId,
                        abandoned,
                        completed  
                        FROM characters_quest_scenario WHERE characterId = @charId";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questGuildLeve[index] = reader.GetUInt32(1);
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
                        FROM characters_quest_scenario WHERE characterId = @charId";

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int npcLSId = reader.GetUInt16(0);
                            player.playerWork.npcLinkshellChatCalling[npcLSId] = reader.GetBoolean(1);
                            player.playerWork.npcLinkshellChatExtra[npcLSId] = reader.GetBoolean(2);                            
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

    }
}

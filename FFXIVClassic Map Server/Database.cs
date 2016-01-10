using MySql.Data.MySqlClient;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.dataobjects.chara.npc;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Lobby_Server.dataobjects;

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

        public static void loadPlayerCharacter(Player player)
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
                    currentClassJob,                
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
                    currentParty,
                    restBonus,
                    achievementPoints
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
                            player.currentZoneId = reader.GetUInt32(6);
                            player.charaWork.parameterSave.state_mainSkill[0] = reader.GetByte(7);
                            player.gcCurrent = reader.GetByte(8);
                            player.gcRankLimsa = reader.GetByte(9);
                            player.gcRankGridania = reader.GetByte(10);
                            player.gcRankUldah = reader.GetByte(11);
                            player.currentTitle = reader.GetUInt32(12);
                            player.playerWork.guardian = reader.GetByte(13);
                            player.playerWork.birthdayDay = reader.GetByte(14);
                            player.playerWork.birthdayMonth = reader.GetByte(15);
                            player.playerWork.initialTown = reader.GetByte(16);
                            player.playerWork.tribe = reader.GetByte(17);
                            player.playerWork.restBonusExpRate = reader.GetInt32(19);
                            player.achievementPoints = reader.GetUInt32(20);
                        }
                    }

                    player.charaWork.parameterSave.state_mainSkillLevel = 1;

                    /*
                    //Get level of our classjob
                    //Load appearance
                    query = @"
                        SELECT 
                        baseId
                        FROM characters_appearance WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                        }

                    }
                    */
                   
                    //Get level of our classjob
                    //Load appearance
                    query = @"
                        SELECT 
                        hp,
                        hpMax,
                        mp,
                        mpMax                        
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
                        if (reader.Read())
                        {
                            if (reader.GetUInt32(0) == 0xFFFFFFFF)
                                player.modelID = CharacterUtils.getTribeModel(player.playerWork.tribe);
                            else
                                player.modelID = reader.GetUInt32(0);
                            player.appearanceIDs[Character.SIZE] = reader.GetByte(1);
                            player.appearanceIDs[Character.COLORINFO] = (uint)(reader.GetUInt16(3) | (reader.GetUInt16(5) << 10) | (reader.GetUInt16(7) << 20));
                            player.appearanceIDs[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.getFaceInfo(reader.GetByte(8), reader.GetByte(9), reader.GetByte(10), reader.GetByte(11), reader.GetByte(12), reader.GetByte(13), reader.GetByte(14), reader.GetByte(15), reader.GetByte(16), reader.GetByte(17)));
                            player.appearanceIDs[Character.HIGHLIGHT_HAIR] = (uint)(reader.GetUInt16(6) | reader.GetUInt16(4) << 10);
                            player.appearanceIDs[Character.VOICE] = reader.GetByte(2);
                            player.appearanceIDs[Character.WEAPON1] = reader.GetUInt32(18);
                            player.appearanceIDs[Character.WEAPON2] = reader.GetUInt32(19);
                            player.appearanceIDs[Character.HEADGEAR] = reader.GetUInt32(20);
                            player.appearanceIDs[Character.BODYGEAR] = reader.GetUInt32(21);
                            player.appearanceIDs[Character.LEGSGEAR] = reader.GetUInt32(22);
                            player.appearanceIDs[Character.HANDSGEAR] = reader.GetUInt32(23);
                            player.appearanceIDs[Character.FEETGEAR] = reader.GetUInt32(24);
                            player.appearanceIDs[Character.WAISTGEAR] = reader.GetUInt32(25);
                            player.appearanceIDs[Character.R_EAR] = reader.GetUInt32(26);
                            player.appearanceIDs[Character.L_EAR] = reader.GetUInt32(27);
                            player.appearanceIDs[Character.R_FINGER] = reader.GetUInt32(28);
                            player.appearanceIDs[Character.L_FINGER] = reader.GetUInt32(29);
                        }

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
                    query = @"
                        SELECT 
                        hotbarSlot,
                        commandId,
                        recastTime                
                        FROM characters_hotbar WHERE characterId = @charId AND classId = @classId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player);
                    cmd.Parameters.AddWithValue("@classId", player.charaWork.parameterSave.state_mainSkill[0]);
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
                        slot,
                        questId                       
                        FROM characters_quest_scenario WHERE characterId = @charId";
                   
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player);
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
                        slot,
                        questId,
                        abandoned,
                        completed  
                        FROM characters_quest_scenario WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player);
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
                        FROM characters_npclinkshell WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player);
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

        public static SubPacket getLatestAchievements(Player player)
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
                                    achievementId                                     
                                    FROM characters_achievements WHERE characterId = @charId ORDER BY timeDone DESC LIMIT 5";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                            latestAchievements[count] = reader.GetUInt32(0);
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return SetLatestAchievementsPacket.buildPacket(player.actorId, latestAchievements);
        }

        public static SubPacket getAchievements(Player player)
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
                                achievementId                                          
                                FROM characters_achievements WHERE characterId = @charId AND timeDone NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                            latestAchievements[count] = reader.GetUInt32(0);
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }


            return SetLatestAchievementsPacket.buildPacket(player.actorId, latestAchievements);  
        }

    }
}

using FFXIVClassic_Lobby_Server.dataobjects;
using MySql.Data.MySqlClient;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server
{
    //charState: 0 - Reserved, 1 - Deleted, 2 - Inactive, 3 - Active

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
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM characters WHERE name=@name AND serverId=@serverId", conn);
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
            }

            return alreadyExists;
        }        

        public static void makeCharacter(uint accountId, uint cid, CharaInfo charaInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE characters SET state=3, charaInfo=@encodedInfo WHERE userId=@userId AND id=@cid";
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@userId", accountId);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    string json = JsonConvert.SerializeObject(charaInfo);
                    cmd.Parameters.AddWithValue("@encodedInfo", json);
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
        }

        public static void renameCharacter(uint characterId, String newName)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE characters SET name=@name WHERE id=@cid";
                    cmd.Prepare();
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
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<Character> charaList = null;
                try
                {
                    conn.Open();
                    charaList = conn.Query<Character>("SELECT * FROM characters WHERE userId=@UserId AND state in (2,3) ORDER BY slot", new { UserId = userId }).ToList();
                }
                catch (MySqlException e)
                { charaList = new List<Character>(); }
                finally
                {
                    conn.Dispose();
                }
                return charaList;
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

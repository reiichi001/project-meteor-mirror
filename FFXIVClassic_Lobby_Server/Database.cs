using FFXIVClassic_Lobby_Server.dataobjects;
using MySql.Data.MySqlClient;
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
        public static void reserveCharacter(int accountId, int slot, int serverId, String name)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO ffxiv_characters2(accountId, slot, serverId, name, charState) VALUES(@accountId, @slot, @serverId, @name, 0)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@serverId", serverId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static void makeCharacter(int accountId, String name, Character charaInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE ffxiv_characters2 SET data=@encodedInfo WHERE accountId=@accountId AND name=@name";
                    cmd.Prepare();

                    cmd.Parameters.AddWithValue("@accountId", accountId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@encodedInfo", JsonConvert.SerializeObject(charaInfo));
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Close();
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
                    cmd.CommandText = "UPDATE ffxiv_characters2 SET name=@name WHERE id=@cid";
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
                    conn.Close();
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
                    cmd.CommandText = "UPDATE ffxiv_characters2 SET state=1 WHERE id=@cid AND name=@name";
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
                    conn.Close();
                }
            }
        }

        public static List<World> getServers()
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<World> worldList = new List<World>();
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM ffxiv_servers WHERE isActive=true";
                    cmd.Prepare();
                    MySqlDataReader Reader = cmd.ExecuteReader();

                    
                    if (!Reader.HasRows) return worldList;
                    while (Reader.Read())
                    {
                        var id = Reader.GetUInt16("id");
                        var name = Reader.GetString("name");
                        var address = Reader.GetString("address");
                        var port = Reader.GetUInt16("port");
                        var unknown = Reader.GetUInt16("unknown");
                        var numChars = Reader.GetUInt32("numChars");
                        var maxChars = Reader.GetUInt32("maxChars");
                        var isActive = Reader.GetBoolean("isActive");

                        if (isActive)
                        {
                            World world = new World();
                            world.id = id; 
                            world.name = name;
                            world.address = address;
                            world.port = port;
                            world.unknown = unknown;
                            uint result = ((numChars / maxChars) *0xFF) & 0xFF;
                            world.population = (ushort)result;
                            world.isActive = isActive;
                            worldList.Add(world);
                        }
                    }

                    
                }
                catch (MySqlException e)
                { }
                finally
                {
                    conn.Close();
                }
                return worldList;
            }
        }

        public static World getServer(uint serverId)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                World world = null;
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM ffxiv_servers WHERE id=%serverId";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@serverId", serverId);

                    MySqlDataReader Reader = cmd.ExecuteReader();

                    if (!Reader.HasRows) return world;
                    while (Reader.Read())
                    {
                        var id = Reader.GetUInt16("id");
                        var name = Reader.GetString("name");
                        var address = Reader.GetString("address");
                        var port = Reader.GetUInt16("port");
                        var unknown = Reader.GetUInt16("unknown");
                        var numChars = Reader.GetUInt32("numChars");
                        var maxChars = Reader.GetUInt32("maxChars");
                        var isActive = Reader.GetBoolean("isActive");

                        if (isActive)
                        {
                            world = new World();
                            world.id = id;
                            world.name = name;
                            world.address = address;
                            world.port = port;
                            world.unknown = unknown;
                            uint result = ((numChars / maxChars) * 0xFF) & 0xFF;
                            world.population = (ushort)result;
                            world.isActive = isActive;                           
                        }
                    }

                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    conn.Close();
                }

                return world;
            }
        }
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using FFXIVClassic_World_Server.DataObjects;
using FFXIVClassic_World_Server.DataObjects.Group;

namespace FFXIVClassic_World_Server
{
    class Database
    {
        public static DBWorld GetServer(uint serverId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBWorld world = null;
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT name, address, port FROM servers WHERE id = @serverId", conn);
                    cmd.Parameters.AddWithValue("@serverId", serverId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            world = new DBWorld();
                            world.id = serverId;
                            world.name = Reader.GetString("name");
                            world.address = Reader.GetString("address");
                            world.port = Reader.GetUInt16("port");                           
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

                return world;
            }
        }   

        public static bool LoadZoneSessionInfo(Session session)
        {
            string characterName;
            uint currentZone = 0;
            uint destinationZone = 0;
            bool readIn = false;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT name, currentZoneId, destinationZoneId FROM characters WHERE id = @charaId", conn);
                    cmd.Parameters.AddWithValue("@charaId", session.sessionId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            characterName = Reader.GetString("name");
                            currentZone = Reader.GetUInt32("currentZoneId");
                            destinationZone = Reader.GetUInt32("destinationZoneId");

                            session.characterName = characterName;
                            session.currentZoneId = currentZone;

                            readIn = true;
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

            return readIn;
        }

        public static void GetAllCharaNames(Dictionary<uint, string> mIdToNameMap)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM characters", conn);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint id = Reader.GetUInt32("id");
                            string name = Reader.GetString("name");
                            mIdToNameMap.Add(id, name);
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

        public static uint GetCurrentZoneForSession(uint charId)
        {
            uint currentZone = 0;
            uint destinationZone = 0;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT currentZoneId, destinationZoneId FROM characters WHERE id = @charaId", conn);
                    cmd.Parameters.AddWithValue("@charaId", charId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            currentZone = Reader.GetUInt32("currentZoneId");
                            destinationZone = Reader.GetUInt32("destinationZoneId");
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

            if (currentZone == 0 && destinationZone != 0)
                return destinationZone;
            if (currentZone != 0 && destinationZone == 0)
                return currentZone;
            else
            {
                return 0;
            }
        }

        public static List<RetainerGroupMember> GetRetainers(uint charaId)
        {
            List<RetainerGroupMember> members = new List<RetainerGroupMember>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT id, name, classActorId, cdIDOffset, placeName, conditions, level FROM server_retainers INNER JOIN characters_retainers ON retainerId = server_retainers.id WHERE characterId = @charaId", conn);
                    cmd.Parameters.AddWithValue("@charaId", charaId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint id = Reader.GetUInt32("id") | 0xE0000000;
                            string name = Reader.GetString("name");
                            uint classActorId = Reader.GetUInt32("classActorId");
                            byte cdIDOffset = Reader.GetByte("cdIDOffset");
                            ushort placeName = Reader.GetUInt16("placeName");
                            byte conditions = Reader.GetByte("conditions");
                            byte level = Reader.GetByte("level");

                            members.Add(new RetainerGroupMember(id, name, classActorId, cdIDOffset, placeName, conditions, level));
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
            return members;
        }

        public static Linkshell GetLinkshell(ulong groupIndex, string lsName)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT id, name, crestIcon, master FROM server_linkshells WHERE name = @lsName", conn);
                    cmd.Parameters.AddWithValue("@lsName", lsName);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            ulong lsId = Reader.GetUInt64("id");
                            string name = Reader.GetString("name");
                            ushort crest = Reader.GetUInt16("crestIcon");
                            uint master = Reader.GetUInt32("master");

                            Linkshell linkshell = new Linkshell(lsId, groupIndex, name, crest, master, 0xa);
                            return linkshell;
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
            return null;
        }

        public static Linkshell GetLinkshell(ulong groupIndex, ulong lsId)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT name, crestIcon, master FROM server_linkshells WHERE id = @lsId", conn);
                    cmd.Parameters.AddWithValue("@lsId", lsId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            string name = Reader.GetString("name");
                            ushort crest = Reader.GetUInt16("crestIcon");
                            uint master = Reader.GetUInt32("master");

                            Linkshell linkshell = new Linkshell(lsId, groupIndex, name, crest, master, 0xa);
                            return linkshell;
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
            return null;
        }

        public static List<LinkshellMember> GetLSMembers(Linkshell ls)
        {
            List<LinkshellMember> memberList = new List<LinkshellMember>();            
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT characterId, linkshellId, rank FROM characters_linkshells WHERE linkshellId = @lsId", conn);
                    cmd.Parameters.AddWithValue("@lsId", ls.dbId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint characterId = Reader.GetUInt32("characterId");
                            ulong linkshellId = Reader.GetUInt64("linkshellId");
                            byte rank = Reader.GetByte("rank");

                            LinkshellMember member = new LinkshellMember(characterId, linkshellId, rank);
                            memberList.Add(member);
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
            memberList.Sort();
            return memberList;
        }

        public static List<LinkshellMember> GetPlayerLSMembership(uint charaId)
        {
            List<LinkshellMember> memberList = new List<LinkshellMember>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT characterId, linkshellId, rank FROM characters_linkshells WHERE characterId = @charaId", conn);
                    cmd.Parameters.AddWithValue("@charaId", charaId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint characterId = Reader.GetUInt32("characterId");
                            ulong linkshellId = Reader.GetUInt64("linkshellId");
                            byte rank = Reader.GetByte("rank");

                            LinkshellMember member = new LinkshellMember(characterId, linkshellId, rank);
                            memberList.Add(member);
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
            return memberList;
        }

        public static ulong CreateLinkshell(string name, ushort crest, uint master)
        {
            string query;
            MySqlCommand cmd;
            ulong lastId = 0;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    query = @"
                    INSERT INTO server_linkshells 
                    (name, crestIcon, master, rank)
                    VALUES
                    (@name, @crestIcon, @master, @rank)             
                    ";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@crestIcon", crest);
                    cmd.Parameters.AddWithValue("@master", master);
                    cmd.Parameters.AddWithValue("@rank", 0xa);

                    if (cmd.ExecuteNonQuery() == 1)
                        lastId = (ulong)cmd.LastInsertedId;
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

            return lastId;
        }

        public static bool DeleteLinkshell(ulong lsId)
        {
            throw new NotImplementedException();
        }

        public static bool LinkshellAddPlayer(ulong lsId, uint charaId)
        {
            throw new NotImplementedException();
        }

        public static bool LinkshellRemovePlayer(ulong lsId, uint charaId)
        {
            throw new NotImplementedException();
        }

        public static bool ChangeLinkshellCrest(ulong lsId, ushort newCrestId)
        {
            bool success = false;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE server_linkshells SET crestIcon = @crestIcon WHERE id = @lsId", conn);
                    cmd.Parameters.AddWithValue("@lsId", lsId);
                    cmd.Parameters.AddWithValue("@crestIcon", newCrestId);
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
    }
}

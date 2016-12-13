using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_World_Server.DataObjects;
using FFXIVClassic_World_Server.DataObjects.Group;

namespace FFXIVClassic_World_Server
{
    class Database
    {
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

        public static Dictionary<uint, RetainerGroupMember> GetRetainers(uint charaId)
        {
            throw new NotImplementedException();
        }

        public static Linkshell GetLinkshell(ulong groupIndex, ulong id)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT name, crestIcon, master FROM server_linkshells WHERE id = @lsId", conn);
                    cmd.Parameters.AddWithValue("@lsId", id);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            string name = Reader.GetString("name");
                            ushort crest = Reader.GetUInt16("crestIcon");
                            uint master = Reader.GetUInt32("master");

                            Linkshell linkshell = new Linkshell(id, groupIndex, name, crest, master, 0xa);
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

        public static Dictionary<ulong, LinkshellMember> GetLSMembers(ulong lsId)
        {
            Dictionary<ulong, LinkshellMember> memberList = new Dictionary<ulong, LinkshellMember>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT characterId, linkshellId, slot, rank FROM characters_linkshells WHERE linkshellId = @lsId", conn);
                    cmd.Parameters.AddWithValue("@lsId", lsId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint characterId = Reader.GetUInt32("characterId");
                            ulong linkshellId = Reader.GetUInt64("linkshellId");
                            ushort slot = Reader.GetUInt16("slot");
                            ushort rank = Reader.GetUInt16("rank");

                            LinkshellMember member = new LinkshellMember(characterId, linkshellId, slot, rank);
                            memberList.Add(characterId, member);
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

        public static List<LinkshellMember> GetPlayerLSMembership(uint charaId)
        {
            List<LinkshellMember> memberList = new List<LinkshellMember>();
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT characterId, linkshellId, slot, rank FROM characters_linkshells WHERE characterid = @charaId", conn);
                    cmd.Parameters.AddWithValue("@lsId", charaId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            uint characterId = Reader.GetUInt32("characterId");
                            ulong linkshellId = Reader.GetUInt64("linkshellId");
                            ushort slot = Reader.GetUInt16("slot");
                            ushort rank = Reader.GetUInt16("rank");

                            LinkshellMember member = new LinkshellMember(characterId, linkshellId, slot, rank);
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
            throw new NotImplementedException();
        }

        public static bool DeleteLinkshell(ulong lsId)
        {
            throw new NotImplementedException();
        }

        public static bool LinkshellAddPlayer(ulong dbId, uint charaId)
        {
            throw new NotImplementedException();
        }

        public static bool LinkshellRemovePlayer(ulong lsId, uint charaId)
        {
            throw new NotImplementedException();
        }
        
    }
}

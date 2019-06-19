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

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.packets.send.player;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FFXIVClassic_Map_Server.utils
{
    class SQLGeneration
    {
        
        public static void GenerateZones()
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    INSERT INTO server_zones VALUES (@id, NULL, @placename, false, false, false, false)";


                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", 100);
                    cmd.Parameters.AddWithValue("@placename", "");

                    cmd.Prepare();

                    Dictionary<uint, string> placenames = new Dictionary<uint, string>();

                    string line2;
                    Regex csvSplit2 = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
                    System.IO.StreamReader file2 = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\xtx_placeName.csv");
                    while ((line2 = file2.ReadLine()) != null)
                    {
                        MatchCollection matches = csvSplit2.Matches(line2);

                        uint id;
                        string name;

                        try
                        {
                            id = UInt32.Parse(matches[0].Value.Trim(','));
                            name = matches[2].Value.Trim(',');

                        }
                        catch (FormatException)
                        { continue; }

                        placenames.Add(id, name);

                    }

                    string line;
                    Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
                    System.IO.StreamReader file = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\_zoneParam.csv");
                    while ((line = file.ReadLine()) != null)
                    {
                        MatchCollection matches = csvSplit.Matches(line);

                        uint id;
                        uint pId;

                        try
                        {
                            id = UInt32.Parse(matches[0].Value.Trim(','));
                            pId = UInt32.Parse(matches[1].Value.Trim(','));
                        }
                        catch (FormatException)
                        { continue; }

                        cmd.Parameters["@id"].Value = id;

                        cmd.Parameters["@placename"].Value = placenames[pId];

                        Program.Log.Debug("Wrote: {0}", id);
                        cmd.ExecuteNonQuery();

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

        public static void GenerateActors()
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    INSERT INTO gamedata_actor_templates VALUES (@id, @displayNameId, NULL)";


                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", 100);
                    cmd.Parameters.AddWithValue("@displayNameId", 100);
                   
                    cmd.Prepare();

                    string line, line2;
                    Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
                    System.IO.StreamReader file = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\actorclass.csv");
                    while ((line = file.ReadLine()) != null)
                    {
                        MatchCollection matches = csvSplit.Matches(line);

                        uint id;
                        uint nameId;

                        try
                        {
                            id = UInt32.Parse(matches[0].Value.Trim(','));
                            nameId = UInt32.Parse(matches[6].Value.Trim(','));

                        }
                        catch (FormatException)
                        { continue; }

                        cmd.Parameters["@id"].Value = id;
                        cmd.Parameters["@displayNameId"].Value = nameId;

                        Program.Log.Debug("Wrote: {0} : {1}", id, nameId);
                        cmd.ExecuteNonQuery();

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

        public static void GenerateActorAppearance()
        {
            uint NUMFIELDS = 39;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    INSERT INTO gamedata_actor_appearance VALUES (@id, ";

                    for (int i = 0; i < NUMFIELDS-1; i++)
                        query += "@v"+i+", ";

                    query += "@v" + (NUMFIELDS-1) + ")";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", 0);

                    for (int i = 0; i < NUMFIELDS; i++)
                        cmd.Parameters.AddWithValue("@v" + i, 100);

                    cmd.Prepare();

                    string line;
                    Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
                    //System.IO.StreamReader file = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\actorclass.csv");
                    System.IO.StreamReader file = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\actorclass_graphic.csv");
                    while ((line = file.ReadLine()) != null)
                    {
                        MatchCollection matches = csvSplit.Matches(line);
                        
                        uint id;
                       
                        try
                        {
                            id = UInt32.Parse(matches[0].Value.Trim(','));

                            for (int i = 0; i < NUMFIELDS; i++)                                                           
                                cmd.Parameters["@v" + i].Value = matches[i + 7].Value.Trim(',');
                            

                        }
                        catch (FormatException)
                        { continue; }

                        cmd.Parameters["@id"].Value = id;

                        Program.Log.Debug("Wrote: {0}", id);                        
                        cmd.ExecuteNonQuery();

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

        public static void GenerateAchievementIds()
        {
            
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    INSERT INTO gamedata_achievements VALUES (@id, @name, @otherId, @rewardPoints)";
                        
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", 100);
                    cmd.Parameters.AddWithValue("@name", "Battle");
                    cmd.Parameters.AddWithValue("@otherId", 0);
                    cmd.Parameters.AddWithValue("@rewardPoints", 0);
                    cmd.Prepare();

                    int otherId = 1;
                    string line, line2;
                    Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
                    System.IO.StreamReader file = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\achievement.csv");
                    System.IO.StreamReader file2 = new System.IO.StreamReader("D:\\Coding\\FFXIV Related\\FFXIV Tool\\2012.09.19.0001.decode.csv\\xtx_achievement.csv");
                    while ((line = file.ReadLine()) != null)
                    {
                        line2 = file2.ReadLine();
                        MatchCollection matches = csvSplit.Matches(line);
                        MatchCollection matches2 = csvSplit.Matches(line2);
                        uint id;
                        string name;
                        uint points;
                        try
                        {
                            id = UInt32.Parse(matches2[0].Value.Trim(','));
                            name = matches2[9].Value.Trim(',');
                            points = UInt32.Parse(matches[3].Value.Trim(','));
                        }
                        catch (FormatException)
                        { continue;  }

                        if (id == 100)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_BATTLE;
                        else if (id == 200)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_CHARACTER;
                        else if (id == 400)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_CURRENCY;
                        else if (id == 500)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_ITEMS;
                        else if (id == 600)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_SYNTHESIS;
                        else if (id == 700)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_GATHERING;
                        else if (id == 900)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_MATERIA;
                        else if (id == 1000)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_QUESTS;
                        else if (id == 1200)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_SEASONAL_EVENTS;
                        else if (id == 1300)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_DUNGEONS;
                        else if (id == 1400)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_EXPLORATION;
                        else if (id == 1500)
                            otherId = SetCompletedAchievementsPacket.CATEGORY_GRAND_COMPANY;

                        Program.Log.Debug("Wrote: {0} : {1} : {2} : {3}", id, name, otherId, points);
                        cmd.Parameters["@id"].Value = id;
                        cmd.Parameters["@name"].Value = name;
                        cmd.Parameters["@otherId"].Value = otherId;                        
                        cmd.Parameters["@rewardPoints"].Value = points;
                        cmd.ExecuteNonQuery();

                        otherId++;
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

        public static void GetStaticActors()
        {
            using (MemoryStream s = new MemoryStream(File.ReadAllBytes("D:\\luadec\\script\\staticactorr9w.luab")))
            {
                using (BinaryReader binReader = new BinaryReader(s))
                {

                    using (StreamWriter w = File.AppendText("D:\\myfile.txt"))
                    {


                        while (binReader.BaseStream.Position != binReader.BaseStream.Length)
                        {
                            uint id = Utils.SwapEndian(binReader.ReadUInt32()) | 0xA0F00000;

                            List<byte> list = new List<byte>();
                            byte readByte;

                            while ((readByte = binReader.ReadByte()) != 0)
                            { //or whatever your condition is
                                list.Add(readByte);
                            }

                            string output = Encoding.UTF8.GetString(list.ToArray());

                            string output2 = String.Format("mStaticActors.Add(0x{0:x}, new {2}(0x{0:x}, \"{1}\"));", id, output.Substring(1 + output.LastIndexOf("/")), output.Split('/')[1]);

                            Program.Log.Debug(output2);
                            w.WriteLine(output2);

                        }

                    }
                }
            }

            return;
        }

        public static void GenerateScriptsForNPCs()
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load Last 5 Completed
                    string query = @"
                                    SELECT uniqueId FROM server_spawn_locations WHERE zoneId = 206";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString("uniqueId");

                            if (name == null || name.Equals(""))
                                continue;

                            string nameCapital = name.Substring(0, 1).ToUpper() + name.Substring(1);

                            string template = File.ReadAllText("D:\\Coding\\FFXIV Related\\ffxiv-classic-map-server\\FFXIVClassic Map Server\\bin\\Debug\\scripts\\unique\\wil0Town01\\PopulaceStandard\\bertram.lua");

                            template = template.Replace("defaultWil", "defaultFst");
                            template = template.Replace("DftWil", "DftFst");
                            template = template.Replace("Bertram", nameCapital);

                            File.WriteAllText(String.Format("D:\\Coding\\FFXIV Related\\ffxiv-classic-map-server\\FFXIVClassic Map Server\\bin\\Debug\\scripts\\unique\\fst0Town01a\\PopulaceStandard\\{0}.lua", name), template);
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
    }
}

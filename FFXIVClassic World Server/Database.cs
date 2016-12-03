using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

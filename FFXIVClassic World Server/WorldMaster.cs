using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server
{
    class WorldManager
    {
        private Server mServer;
        public Dictionary<string, ZoneServer> mZoneServerList;

        public WorldManager(Server server)
        {
            mServer = server;
        }

        public void LoadZoneServerList()
        {
            mZoneServerList = new Dictionary<string, ZoneServer>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    serverIp,
                                    serverPort
                                    FROM server_zones 
                                    WHERE serverIp IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ip = reader.GetString(0);
                            int port = reader.GetInt32(1);
                            string address = ip + ":" + port;

                            if (!mZoneServerList.ContainsKey(address))
                            {
                                ZoneServer zone = new ZoneServer(ip, port);
                                mZoneServerList.Add(address, zone);
                            }
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
        
        public void ConnectToZoneServers()
        {
            Program.Log.Info("--------------------------");
            Program.Log.Info("Connecting to zone servers");
            Program.Log.Info("--------------------------");

            foreach (ZoneServer zs in mZoneServerList.Values)
            {
                zs.Connect();
            }
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed.
        public void DoSeamlessZoneServerChange(Session session, uint destinationZoneId)
        {
            
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneServerChange(Session session, uint zoneEntrance)
        {
            /*
            ->Tell old server to save session info and remove session. Start zone packets.
            ->Update the position to zoneEntrance
            ->Update routing
            ->Tell new server to load session info and add session. Send end zone packets.
            */
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneServerChange(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            /*
           ->Tell old server to save session info and remove
           ->Update the position to params
           ->Update routing
           ->Tell new server to load session info and add           
           */
        }

        //Login Zone In
        public void DoLogin(Session session)
        {
            /*
           ->Update routing
           ->Tell new server to load session info and add           
           */

            
        }

        public class ZoneEntrance
        {
            public uint zoneId;
            public string privateAreaName;
            public byte spawnType;
            public float spawnX;
            public float spawnY;
            public float spawnZ;
            public float spawnRotation;

            public ZoneEntrance(uint zoneId, string privateAreaName, byte spawnType, float x, float y, float z, float rot)
            {
                this.zoneId = zoneId;
                this.privateAreaName = privateAreaName;
                this.spawnType = spawnType;
                this.spawnX = x;
                this.spawnY = y;
                this.spawnZ  = z;
                this.spawnRotation = rot;
            }
        }       

    }

}

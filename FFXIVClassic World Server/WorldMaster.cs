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
        private Dictionary<uint, ZoneEntrance> zoneEntranceList;

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
                                    id,
                                    serverIp,
                                    serverPort
                                    FROM server_zones 
                                    WHERE serverIp IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0); 
                            string ip = reader.GetString(1);
                            int port = reader.GetInt32(2);
                            string address = ip + ":" + port;

                            if (!mZoneServerList.ContainsKey(address))
                            {
                                ZoneServer zone = new ZoneServer(ip, port, id);
                                mZoneServerList.Add(address, zone);
                            }
                            else
                                mZoneServerList[address].AddLoadedZone(id);
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

        public void LoadZoneEntranceList()
        {
            zoneEntranceList = new Dictionary<uint, ZoneEntrance>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    zoneId,
                                    spawnType,
                                    spawnX,
                                    spawnY,
                                    spawnZ,
                                    spawnRotation,
                                    privateAreaName
                                    FROM server_zones_spawnlocations";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);
                            string privArea = null;

                            if (!reader.IsDBNull(7))
                                privArea = reader.GetString(7);

                            ZoneEntrance entance = new ZoneEntrance(reader.GetUInt32(1), privArea, reader.GetByte(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6));
                            zoneEntranceList[id] = entance;
                            count++;
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

            Program.Log.Info(String.Format("Loaded {0} zone spawn locations.", count));
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

        public ZoneServer GetZoneServer(uint zoneId)
        {
            foreach (ZoneServer zs in mZoneServerList.Values)
            {
                if (zs.ownedZoneIds.Contains(zoneId))
                    return zs;
            }

            return null;
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed.
        public void DoSeamlessZoneServerChange(Session session, uint destinationZoneId)
        {
            
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneServerChange(Session session, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneServerChange(session, ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneServerChange(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            session.routing1.SendSessionEnd(session, destinationZoneId, destinationPrivateArea, spawnType, spawnX, spawnY, spawnZ, spawnRotation);
        }

        //Login Zone In
        public void DoLogin(Session session)
        {            
            session.routing1 = GetZoneServer(Database.GetCurrentZoneForSession(session.sessionId));
            session.routing1.SendSessionStart(session);            
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

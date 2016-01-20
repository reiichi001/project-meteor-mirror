using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.debug;
using FFXIVClassic_Map_Server.actors.world;
using FFXIVClassic_Map_Server.common.EfficientHashTables;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.packets.send;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server
{
    class WorldManager
    {
        private DebugProg debug = new DebugProg();
        private WorldMaster worldMaster = new WorldMaster();
        private Dictionary<uint, Zone> zoneList;
        private Dictionary<uint, ZoneEntrance> zoneEntranceList;

        private Server mServer;

        public WorldManager(Server server)
        {
            mServer = server;
        }

        public void LoadZoneList()
        {
            zoneList = new Dictionary<uint, Zone>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    regionId,
                                    zoneName,
                                    dayMusic,
                                    nightMusic,
                                    battleMusic,
                                    isInn,
                                    canRideChocobo,
                                    canStealth,
                                    isInstanceRaid
                                    FROM server_zones 
                                    WHERE zoneName IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Zone zone = new Zone(reader.GetUInt32(0), reader.GetString(2), reader.GetUInt16(1), reader.GetUInt16(3), reader.GetUInt16(4), reader.GetUInt16(5), reader.GetBoolean(6), reader.GetBoolean(7), reader.GetBoolean(8), reader.GetBoolean(9));
                            zoneList[zone.actorId] = zone;
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

            Log.info(String.Format("Loaded {0} zones.", count));
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
                                    spawnRotation
                                    FROM server_zones_spawnlocations";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);
                            ZoneEntrance entance = new ZoneEntrance(reader.GetUInt32(1), reader.GetByte(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6));
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

            Log.info(String.Format("Loaded {0} zone spawn locations.", count));
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed.
        public void DoSeamlessZoneChange(Player player, uint destinationZoneId)
        {
            Zone oldZone;

            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.removeActorToZone(player);
            }

            //Add player to new zone and update
            Zone newZone = GetZone(destinationZoneId);

            //This server does not contain that zoneId
            if (newZone == null)
                return;

            newZone.addActorToZone(player);
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneChange(Player player, uint destinationZoneId, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Log.error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneChange(player, destinationZoneId, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneChange(Player player, uint destinationZoneId, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            Zone oldZone;

            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.removeActorToZone(player);
            }

            //Add player to new zone and update
            Zone newZone = GetZone(destinationZoneId);

            //This server does not contain that zoneId
            if (newZone == null)
                return;

            newZone.addActorToZone(player);
            
            //Update player actor's properties
            player.zoneId = newZone.actorId;
            player.zone = newZone;
            player.positionX = spawnX;
            player.positionY = spawnY;
            player.positionZ = spawnZ;
            player.rotation = spawnRotation;

            //Send packets
            player.sendZoneInPackets(this);               
        }

        //Login Zone In
        public void DoLogin(Player player)
        {
            //Add player to new zone and update
            Zone zone = GetZone(player.zoneId);

            //This server does not contain that zoneId
            if (zone == null)
                return;

            //Set the current zone and add player
            player.zone = zone;
            zone.addActorToZone(player);

            //Send packets
            player.sendZoneInPackets(this);  
        }

        public Player GetPCInWorld(string name)
        {            
            foreach (Zone zone in zoneList.Values)
            { 
                Player p = zone.FindPCInZone(name);
                if (p != null)
                    return p;
            }
            return null;
        }

        public Player GetPCInWorld(uint charId)
        {
            foreach (Zone zone in zoneList.Values)
            {
                Player p = zone.FindPCInZone(charId);
                if (p != null)
                    return p;
            }
            return null;
        }

        public Zone GetZone(uint zoneId)
        {
            return zoneList[zoneId];
        }

        public WorldMaster GetActor()
        {
            return worldMaster;
        }

        public DebugProg GetDebugActor()
        {
            return debug;
        }

        public class ZoneEntrance
        {
            public uint zoneId;
            public byte spawnType;
            public float spawnX;
            public float spawnY;
            public float spawnZ;
            public float spawnRotation;

            public ZoneEntrance(uint zoneId, byte spawnType, float x, float y, float z, float rot)
            {
                this.zoneId = zoneId;
                this.spawnType = spawnType;
                this.spawnX = x;
                this.spawnY = y;
                this.spawnZ  = z;
                this.spawnRotation = rot;
            }
        }

    }

}

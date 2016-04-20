using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.common.EfficientHashTables;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.login;
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
            int count1 = 0;
            int count2 = 0;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    zoneName,
                                    regionId,
                                    className,
                                    dayMusic,
                                    nightMusic,
                                    battleMusic,
                                    isIsolated,
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
                            Zone zone = new Zone(reader.GetUInt32(0), reader.GetString(1), reader.GetUInt16(2), reader.GetString(3), reader.GetUInt16(4), reader.GetUInt16(5), reader.GetUInt16(6), reader.GetBoolean(7), reader.GetBoolean(8), reader.GetBoolean(9), reader.GetBoolean(10), reader.GetBoolean(11));
                            zoneList[zone.actorId] = zone;
                            count1++;
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

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    parentZoneId,
                                    privateAreaName,
                                    className,
                                    dayMusic,
                                    nightMusic,
                                    battleMusic
                                    FROM server_zones_privateareas
                                    WHERE privateAreaName IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint parentZoneId = reader.GetUInt32("parentZoneId");

                            if (zoneList.ContainsKey(parentZoneId))
                            {
                                Zone parent = zoneList[parentZoneId];
                                PrivateArea privArea = new PrivateArea(parent, reader.GetUInt32("id"), reader.GetString("className"), reader.GetString("privateAreaName"), reader.GetUInt16("dayMusic"), reader.GetUInt16("nightMusic"), reader.GetUInt16("battleMusic"));
                                parent.addPrivateArea(privArea);
                            }
                            else
                                continue;
      
                            count2++;
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

            Log.info(String.Format("Loaded {0} zones and {1} private areas.", count1, count2));
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

            Log.info(String.Format("Loaded {0} zone spawn locations.", count));
        }

        public void LoadNPCs()
        {            
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    name,
                                    zoneId,                                    
                                    positionX,
                                    positionY,
                                    positionZ,
                                    rotation,
                                    actorState,
                                    animationId,
                                    displayNameId,
                                    customDisplayName,
                                    actorClassName,
                                    eventConditions
                                    FROM gamedata_actor_class
                                    WHERE name is not NULL AND zoneId > 0
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string customName = null;
                            if (!reader.IsDBNull(10))
                                customName = reader.GetString(10);

                            Npc npc = new Npc(reader.GetUInt32(0), reader.GetString(1), reader.GetUInt32(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6), reader.GetUInt16(7), reader.GetUInt32(8), reader.GetUInt32(9), customName, reader.GetString(11));

                            if (!reader.IsDBNull(12))
                            {
                                string eventConditions = reader.GetString(12);
                                npc.loadEventConditions(eventConditions);
                            }

                            if (!zoneList.ContainsKey(npc.zoneId))
                                continue;
                            Zone zone = zoneList[npc.zoneId];
                            if (zone == null)
                                continue;
                            npc.zone = zone;
                            zone.addActorToZone(npc);
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

            Log.info(String.Format("Loaded {0} npc(s).", count));
        }

        public void LoadNPCs(uint zoneId)
        {
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    name,
                                    zoneId,                                    
                                    positionX,
                                    positionY,
                                    positionZ,
                                    rotation,
                                    actorState,
                                    animationId,
                                    displayNameId,
                                    customDisplayName,
                                    actorClassName,
                                    eventConditions
                                    FROM gamedata_actor_class
                                    WHERE name is not NULL AND zoneId = @zoneId
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@zoneId", zoneId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string customName = null;
                            if (!reader.IsDBNull(10))
                                customName = reader.GetString(10);

                            Npc npc = new Npc(reader.GetUInt32(0), reader.GetString(1), reader.GetUInt32(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6), reader.GetUInt16(7), reader.GetUInt32(8), reader.GetUInt32(9), customName, reader.GetString(11));

                            if (!reader.IsDBNull(12))
                            {
                                string eventConditions = reader.GetString(12);
                                npc.loadEventConditions(eventConditions);
                            }

                            if (!zoneList.ContainsKey(npc.zoneId))
                                continue;
                            Zone zone = zoneList[npc.zoneId];
                            if (zone == null)
                                continue;
                            npc.zone = zone;
                            zone.addActorToZone(npc);
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

            Log.info(String.Format("Loaded {0} npc(s).", count));
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed.
        public void DoSeamlessZoneChange(Player player, uint destinationZoneId)
        {
            Area oldZone;

            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.removeActorFromZone(player);
            }

            //Add player to new zone and update
            Zone newZone = GetZone(destinationZoneId);

            //This server does not contain that zoneId
            if (newZone == null)
                return;

            newZone.addActorToZone(player);

            LuaEngine.onZoneIn(player);
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneChange(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Log.error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneChange(player, ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneChange(Player player, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            Area oldZone;

            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.removeActorFromZone(player);
            }

            //Add player to new zone and update
            Area newArea;

            if (destinationPrivateArea == null)
                newArea = GetZone(destinationZoneId);
            else
                newArea = GetZone(destinationZoneId).getPrivateArea(destinationPrivateArea, 0);
            //This server does not contain that zoneId
            if (newArea == null)
                return;

            newArea.addActorToZone(player);

            //Update player actor's properties
            player.zoneId = newArea.actorId;
            player.zone = newArea;
            player.positionX = spawnX;
            player.positionY = spawnY;
            player.positionZ = spawnZ;
            player.rotation = spawnRotation;

            //Send packets
            player.playerSession.queuePacket(DeleteAllActorsPacket.buildPacket(player.actorId), true, false);
            player.playerSession.queuePacket(_0xE2Packet.buildPacket(player.actorId, 0x0), true, false);
            player.sendZoneInPackets(this, spawnType);
            player.playerSession.clearInstance();
            player.sendInstanceUpdate();

            LuaEngine.onZoneIn(player);
        }

        //Moves actor within zone to spawn position
        public void DoPlayerMoveInZone(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Log.error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];

            if (ze.zoneId != player.zoneId)
                return;

            DoPlayerMoveInZone(player, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation, ze.spawnType);
        }

        //Moves actor within the zone
        public void DoPlayerMoveInZone(Player player, float spawnX, float spawnY, float spawnZ, float spawnRotation, byte spawnType = 0xF)
        {            
            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                player.zone.removeActorFromZone(player);
                player.zone.addActorToZone(player);

                //Update player actor's properties;
                player.positionX = spawnX;
                player.positionY = spawnY;
                player.positionZ = spawnZ;
                player.rotation = spawnRotation;

                //Send packets
                player.playerSession.queuePacket(_0xE2Packet.buildPacket(player.actorId, 0x0), true, false);
                player.playerSession.queuePacket(player.createSpawnTeleportPacket(player.actorId, spawnType), true, false);
                player.sendInstanceUpdate();

            }            
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

            LuaEngine.onBeginLogin(player);
            
            zone.addActorToZone(player);
            
            //Send packets            
            player.sendZoneInPackets(this, 0x1);

            LuaEngine.onLogin(player);
            LuaEngine.onZoneIn(player);
        }

        public void reloadZone(uint zoneId)
        {
            if (!zoneList.ContainsKey(zoneId))
                return;

            Zone zone = zoneList[zoneId];
            //zone.clear();
            LoadNPCs(zone.actorId);

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

        public Actor GetActorInWorld(uint charId)
        {
            foreach (Zone zone in zoneList.Values)
            {
                Actor a = zone.FindActorInZone(charId);
                if (a != null)
                    return a;
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
            if (!zoneList.ContainsKey(zoneId))
                return null;
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

        public ZoneEntrance getZoneEntrance(uint entranceId)
        {
            if (zoneEntranceList.ContainsKey(entranceId))
                return zoneEntranceList[entranceId];
            else
                return null;
        }
    }

}

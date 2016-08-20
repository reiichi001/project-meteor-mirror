using FFXIVClassic_Map_Server;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;
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
        private Dictionary<uint, List<SeamlessBoundry>> seamlessBoundryList;
        private Dictionary<uint, ZoneEntrance> zoneEntranceList;
        private Dictionary<uint, ActorClass> actorClasses = new Dictionary<uint,ActorClass>();

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
                                PrivateArea privArea = new PrivateArea(parent, reader.GetUInt32("id"), reader.GetString("className"), reader.GetString("privateAreaName"), 1, reader.GetUInt16("dayMusic"), reader.GetUInt16("nightMusic"), reader.GetUInt16("battleMusic"));
                                parent.AddPrivateArea(privArea);
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

            Program.Log.Info(String.Format("Loaded {0} zones and {1} private areas.", count1, count2));
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

        public void LoadSeamlessBoundryList()
        {
            seamlessBoundryList = new Dictionary<uint, List<SeamlessBoundry>>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    *
                                    FROM server_seamless_zonechange_bounds";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            uint regionId = reader.GetUInt32("regionId");
                            uint zoneId1 = reader.GetUInt32("zoneId1");
                            uint zoneId2 = reader.GetUInt32("zoneId2");

                            float z1_x1 = reader.GetFloat("zone1_boundingbox_x1");
                            float z1_y1 = reader.GetFloat("zone1_boundingbox_y1");
                            float z1_x2 = reader.GetFloat("zone1_boundingbox_x2");
                            float z1_y2 = reader.GetFloat("zone1_boundingbox_y2");

                            float z2_x1 = reader.GetFloat("zone2_boundingbox_x1");
                            float z2_y1 = reader.GetFloat("zone2_boundingbox_y1");
                            float z2_x2 = reader.GetFloat("zone2_boundingbox_x2");
                            float z2_y2 = reader.GetFloat("zone2_boundingbox_y2");

                            float m_x1 = reader.GetFloat("merge_boundingbox_x1");
                            float m_y1 = reader.GetFloat("merge_boundingbox_y1");
                            float m_x2 = reader.GetFloat("merge_boundingbox_x2");
                            float m_y2 = reader.GetFloat("merge_boundingbox_y2");
                            
                            if (!seamlessBoundryList.ContainsKey(regionId))
                                seamlessBoundryList.Add(regionId, new List<SeamlessBoundry>());

                            seamlessBoundryList[regionId].Add(new SeamlessBoundry(regionId, zoneId1, zoneId2, z1_x1, z1_y1, z1_x2, z1_y2, z2_x1, z2_y1, z2_x2, z2_y2, m_x1, m_y1, m_x2, m_y2));

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

            Program.Log.Info(String.Format("Loaded {0} region seamless boundries.", count));
        }

        public void LoadActorClasses()
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
                                    classPath,                                    
                                    displayNameId,
                                    propertyFlags,
                                    eventConditions
                                    FROM gamedata_actor_class
                                    WHERE classPath <> ''
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            string classPath = reader.GetString("classPath");
                            uint nameId = reader.GetUInt32("displayNameId");
                            string eventConditions = null;

                            uint propertyFlags = reader.GetUInt32("propertyFlags");

                            if (!reader.IsDBNull(4))
                                eventConditions = reader.GetString("eventConditions");
                            else
                                eventConditions = "{}";

                            ActorClass actorClass = new ActorClass(id, classPath, nameId, propertyFlags, eventConditions);
                            actorClasses.Add(id, actorClass);
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

            Program.Log.Info(String.Format("Loaded {0} actor classes.", count));
        }

        public void LoadSpawnLocations()
        {
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    actorClassId,  
                                    uniqueId,                                  
                                    zoneId,      
                                    privateAreaName,                              
                                    privateAreaLevel,
                                    positionX,
                                    positionY,
                                    positionZ,
                                    rotation,
                                    actorState,
                                    animationId,
                                    customDisplayName
                                    FROM server_spawn_locations                                    
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string customName = null;
                            if (!reader.IsDBNull(11))
                                customName = reader.GetString("customDisplayName");

                            uint classId = reader.GetUInt32("actorClassId");
                            string uniqueId = reader.GetString("uniqueId");
                            uint zoneId = reader.GetUInt32("zoneId");
                            string privAreaName = reader.GetString("privateAreaName");
                            uint privAreaLevel = reader.GetUInt32("privateAreaLevel");
                            float x = reader.GetFloat("positionX");
                            float y = reader.GetFloat("positionY");
                            float z = reader.GetFloat("positionZ");
                            float rot = reader.GetFloat("rotation");
                            ushort state = reader.GetUInt16("actorState");
                            uint animId = reader.GetUInt32("animationId");

                            if (!actorClasses.ContainsKey(classId))                                
                                continue;
                            if (!zoneList.ContainsKey(zoneId))
                                continue;

                            Zone zone = zoneList[zoneId];
                            if (zone == null)
                                continue;

                            SpawnLocation spawn = new SpawnLocation(classId, uniqueId, zoneId, privAreaName, privAreaLevel, x, y, z, rot, state, animId);

                            zone.AddSpawnLocation(spawn);

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

            Program.Log.Info(String.Format("Loaded {0} spawn(s).", count));
        }

        public void SpawnAllActors()
        {
            Program.Log.Info("Spawning actors...");
            foreach (Zone z in zoneList.Values)
                z.SpawnAllActors(true);
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed. Merged zone is removed.
        public void DoSeamlessZoneChange(Player player, uint destinationZoneId)
        {
            Area oldZone;

            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.RemoveActorFromZone(player);
            }

            //Add player to new zone and update
            Zone newZone = GetZone(destinationZoneId);

            //This server does not contain that zoneId
            if (newZone == null)
                return;

            newZone.AddActorToZone(player);

            player.zone = newZone;
            player.zoneId = destinationZoneId;

            player.zone2 = null;
            player.zoneId2 = 0;

            player.SendSeamlessZoneInPackets();

            player.SendMessage(0x20, "", "Doing Seamless Zone Change");

            LuaEngine.OnZoneIn(player);
        }

        //Adds a second zone to pull actors from. Used for an improved seamless zone change.
        public void MergeZones(Player player, uint mergedZoneId)
        {           
            //Add player to new zone and update
            Zone mergedZone = GetZone(mergedZoneId);

            //This server does not contain that zoneId
            if (mergedZone == null)
                return;

            mergedZone.AddActorToZone(player);

            player.zone2 = mergedZone;
            player.zoneId2 = mergedZone.actorId;

            player.SendMessage(0x20, "", "Merging Zones");

            LuaEngine.OnZoneIn(player);
        }

        //Checks all seamless bounding boxes in region to see if player needs to merge or zonechange
        public void SeamlessCheck(Player player)
        {
            //Check if you are in a seamless bounding box
            //WorldMaster.DoSeamlessCheck(this) -- Return 

            /*
             * Find what bounding box in region I am in
             * ->If none, ignore
             * ->If zone box && is my zone, ignore
             * ->If zone box && is not my zone, DoSeamlessZoneChange
             * ->If merge box, MergeZones
             */

            if (player.zone == null)
                return;

            uint regionId = player.zone.regionId;

            if (!seamlessBoundryList.ContainsKey(regionId))
                return;

            foreach (SeamlessBoundry bounds in seamlessBoundryList[regionId])
            {
                if (CheckPosInBounds(player.positionX, player.positionZ, bounds.zone1_x1, bounds.zone1_y1, bounds.zone1_x2, bounds.zone1_y2))
                {
                    if (player.zoneId == bounds.zoneId1 && player.zoneId2 == 0)
                        return;

                    DoSeamlessZoneChange(player, bounds.zoneId1);
                }
                else if (CheckPosInBounds(player.positionX, player.positionZ, bounds.zone2_x1, bounds.zone2_y1, bounds.zone2_x2, bounds.zone2_y2))
                {
                    if (player.zoneId == bounds.zoneId2 && player.zoneId2 == 0)
                        return;

                    DoSeamlessZoneChange(player, bounds.zoneId2);
                }
                else if (CheckPosInBounds(player.positionX, player.positionZ, bounds.merge_x1, bounds.merge_y1, bounds.merge_x2, bounds.merge_y2))
                {
                    uint merged;
                    if (player.zoneId == bounds.zoneId1)
                        merged = bounds.zoneId2;
                    else
                        merged = bounds.zoneId1;

                    //Already merged
                    if (player.zoneId2 == merged)
                        return;

                    MergeZones(player, merged);
                }
            }
        }

        public bool CheckPosInBounds(float x, float y, float x1, float y1, float x2, float y2)
        {
            bool xIsGood = false;
            bool yIsGood = false;

            if ((x1 < x && x < x2) || (x1 > x && x > x2))
                xIsGood = true;

            if ((y1 < y && y < y2) || (y1 > y && y > y2))
                yIsGood = true;

            return xIsGood && yIsGood;
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneChange(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneChange(player, ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneChange(Player player, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            Area oldZone = player.zone;
            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                oldZone.RemoveActorFromZone(player);
            }

            //Add player to new zone and update
            Area newArea;

            if (destinationPrivateArea == null)
                newArea = GetZone(destinationZoneId);
            else
                newArea = GetZone(destinationZoneId).GetPrivateArea(destinationPrivateArea, 0);

            //This server does not contain that zoneId
            if (newArea == null)
            {
                if (oldZone != null)
                {
                    oldZone.AddActorToZone(player);
                }

                var message = "WorldManager.DoZoneChange: unable to change areas, new area is not valid.";
                player.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM, "[Debug]", message);
                Program.Log.Debug(message);
                return;
            }

            newArea.AddActorToZone(player);

            //Update player actor's properties
            player.zoneId = newArea.actorId;
            player.zone = newArea;
            player.positionX = spawnX;
            player.positionY = spawnY;
            player.positionZ = spawnZ;
            player.rotation = spawnRotation;

            //Send packets
            player.playerSession.QueuePacket(DeleteAllActorsPacket.BuildPacket(player.actorId), true, false);
            player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x0), true, false);
            player.SendZoneInPackets(this, spawnType);
            player.playerSession.ClearInstance();
            player.SendInstanceUpdate();

            LuaEngine.OnZoneIn(player);
        }

        //Moves actor within zone to spawn position
        public void DoPlayerMoveInZone(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
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
                player.zone.RemoveActorFromZone(player);
                player.zone.AddActorToZone(player);

                //Update player actor's properties;
                player.positionX = spawnX;
                player.positionY = spawnY;
                player.positionZ = spawnZ;
                player.rotation = spawnRotation;

                //Send packets
                player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x0), true, false);
                player.playerSession.QueuePacket(player.CreateSpawnTeleportPacket(player.actorId, spawnType), true, false);
                player.SendInstanceUpdate();

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

            LuaEngine.OnBeginLogin(player);
            
            zone.AddActorToZone(player);
            
            //Send packets            
            player.SendZoneInPackets(this, 0x1);

            LuaEngine.OnLogin(player);
            LuaEngine.OnZoneIn(player);
        }

        public void ReloadZone(uint zoneId)
        {
            if (!zoneList.ContainsKey(zoneId))
                return;

            Zone zone = zoneList[zoneId];
            //zone.clear();
            //LoadNPCs(zone.actorId);

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

        public ZoneEntrance GetZoneEntrance(uint entranceId)
        {
            if (zoneEntranceList.ContainsKey(entranceId))
                return zoneEntranceList[entranceId];
            else
                return null;
        }

        public ActorClass GetActorClass(uint id)
        {
            if (actorClasses.ContainsKey(id))
                return actorClasses[id];
            else
                return null;
        }
    }
}

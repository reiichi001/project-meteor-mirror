using FFXIVClassic_Map_Server;
using FFXIVClassic.Common;

using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using FFXIVClassic_Map_Server.actors.director;

namespace FFXIVClassic_Map_Server.actors.area
{
    class Zone : Area
    {        
        Dictionary<string, Dictionary<uint, PrivateArea>> privateAreas = new Dictionary<string, Dictionary<uint, PrivateArea>>();
        Dictionary<string, List<PrivateAreaContent>> contentAreas = new Dictionary<string, List<PrivateAreaContent>>();
        Object contentAreasLock = new Object();

        public SharpNav.TiledNavMesh tiledNavMesh;
        public SharpNav.NavMeshQuery navMeshQuery;

        public Int64 pathCalls;
        public Int64 pathCallTime;

        public Zone(uint id, string zoneName, ushort regionId, string classPath, ushort bgmDay, ushort bgmNight, ushort bgmBattle, bool isIsolated, bool isInn, bool canRideChocobo, bool canStealth, bool isInstanceRaid, bool loadNavMesh = false)
            : base(id, zoneName, regionId, classPath, bgmDay, bgmNight, bgmBattle, isIsolated, isInn, canRideChocobo, canStealth, isInstanceRaid)
        {
            if (loadNavMesh)
            {
                try
                {
                    tiledNavMesh = utils.NavmeshUtils.LoadNavmesh(tiledNavMesh, zoneName + ".snb");
                    navMeshQuery = new SharpNav.NavMeshQuery(tiledNavMesh, 100);

                    if (tiledNavMesh != null && tiledNavMesh.Tiles[0].PolyCount > 0)
                        Program.Log.Info($"Loaded navmesh for {zoneName}");
                }
                catch (Exception e)
                {
                    Program.Log.Error(e.Message);
                }
            }
        }

        public void AddPrivateArea(PrivateArea pa)
        {
            if (privateAreas.ContainsKey(pa.GetPrivateAreaName()))
                privateAreas[pa.GetPrivateAreaName()][pa.GetPrivateAreaType()] = pa;
            else
            {
                privateAreas[pa.GetPrivateAreaName()] = new Dictionary<uint, PrivateArea>();
                privateAreas[pa.GetPrivateAreaName()][pa.GetPrivateAreaType()] = pa;
            }
        }

        public PrivateArea GetPrivateArea(string type, uint number)
        {
            if (privateAreas.ContainsKey(type))
            {
                Dictionary<uint, PrivateArea> instances = privateAreas[type];
                if (instances.ContainsKey(number))
                    return instances[number];
                else
                    return null;
            }
            else
                return null;
        }

        public override SubPacket CreateScriptBindPacket()
        {
            bool isEntranceDesion = false;

            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList(classPath, false, true, zoneName, "", -1, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, true, isInstanceRaid, isEntranceDesion);
            return ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams);        
        }

        public void AddSpawnLocation(SpawnLocation spawn)
        {
            //Is it in a private area?
            if (!spawn.privAreaName.Equals(""))
            {
                if (privateAreas.ContainsKey(spawn.privAreaName))
                {
                    Dictionary<uint, PrivateArea> levels = privateAreas[spawn.privAreaName];
                    if (levels.ContainsKey(spawn.privAreaLevel))
                        levels[spawn.privAreaLevel].AddSpawnLocation(spawn);
                    else
                        Program.Log.Error("Tried to add a spawn location to non-existing private area level \"{0}\" in area {1} in zone {2}", spawn.privAreaName, spawn.privAreaLevel, zoneName);
                }
                else
                    Program.Log.Error("Tried to add a spawn location to non-existing private area \"{0}\" in zone {1}", spawn.privAreaName, zoneName);
            }
            else            
                mSpawnLocations.Add(spawn);            
        }

        public void SpawnAllActors(bool doPrivAreas)
        {
            foreach (SpawnLocation spawn in mSpawnLocations)            
                SpawnActor(spawn);

            if (doPrivAreas)
            {
                foreach (Dictionary<uint, PrivateArea> areas in privateAreas.Values)
                {
                    foreach (PrivateArea pa in areas.Values)
                        pa.SpawnAllActors();
                }
            }
        }

        public Actor FindActorInZone(uint id)
        {
            if (!mActorList.ContainsKey(id))
            {
                foreach(Dictionary<uint, PrivateArea> paList in privateAreas.Values)
                {
                    foreach(PrivateArea pa in paList.Values)
                    {
                        Actor actor = pa.FindActorInArea(id);
                        if (actor != null)
                            return actor;
                    }
                }
                return null;
            }
            else
                return mActorList[id];
        }

        public PrivateAreaContent CreateContentArea(Player starterPlayer, string areaClassPath, string contentScript, string areaName, string directorName, params object[] args)
        {
            lock (contentAreasLock)
            {
                Director director = CreateDirector(directorName, true, args);

                if (director == null)
                    return null;

                if (!contentAreas.ContainsKey(areaName))
                    contentAreas.Add(areaName, new List<PrivateAreaContent>());
                PrivateAreaContent contentArea = new PrivateAreaContent(this, classPath, areaName, 1, director, starterPlayer);                
                contentAreas[areaName].Add(contentArea);
                return contentArea;
            }
        }

        public void DeleteContentArea(PrivateAreaContent area)
        {
            if (contentAreas.ContainsKey(area.GetPrivateAreaName()))
            {
                contentAreas[area.GetPrivateAreaName()].Remove(area);
            }
        }

        public override void Update(DateTime tick)
        {
            // todo: again, this is retarded but debug stuff
            base.Update(tick);

            var diffTime = tick - lastUpdate;
            // arbitrary cap
            if (diffTime.TotalMilliseconds >= 33)
            {
            }
            
            if (diffTime.TotalSeconds >= 10)
            {
                if (this.pathCalls > 0)
                {
                    Program.Log.Error("Number of pathfinding calls {0} average time {1}", pathCalls, pathCallTime / pathCalls);
                }
                // todo: this is stupid debug stuff that needs to fuck off
                lastUpdate = tick;
            }
        }

    }
}

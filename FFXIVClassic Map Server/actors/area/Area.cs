using FFXIVClassic_Map_Server;
using FFXIVClassic.Common;

using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.packets.send;

namespace FFXIVClassic_Map_Server.Actors
{
    class Area : Actor
    {
        public string zoneName;        
        public ushort regionId;
        public bool isIsolated, canStealth, isInn, canRideChocobo, isInstanceRaid;
        public ushort weatherNormal, weatherCommon, weatherRare;
        public ushort bgmDay, bgmNight, bgmBattle;

        protected string classPath;

        public int boundingGridSize = 50;
        public int minX = -1000, minY = -1000, maxX = 1000, maxY = 1000;
        protected int numXBlocks, numYBlocks;
        protected int halfWidth, halfHeight;

        protected List<SpawnLocation> mSpawnLocations = new List<SpawnLocation>();
        protected Dictionary<uint, Actor> mActorList = new Dictionary<uint, Actor>();
        protected List<Actor>[,] mActorBlock;

        LuaScript areaScript;

        public Area(uint id, string zoneName, ushort regionId, string className, ushort bgmDay, ushort bgmNight, ushort bgmBattle, bool isIsolated, bool isInn, bool canRideChocobo, bool canStealth, bool isInstanceRaid)
            : base(id)
        {

            this.zoneName = zoneName;
            this.regionId = regionId;
            this.canStealth = canStealth;
            this.isIsolated = isIsolated;
            this.isInn = isInn;
            this.canRideChocobo = canRideChocobo;
            this.isInstanceRaid = isInstanceRaid;

            this.bgmDay = bgmDay;
            this.bgmNight = bgmNight;
            this.bgmBattle = bgmBattle;

            this.displayNameId = 0;
            this.customDisplayName = "_areaMaster";
            this.actorName = String.Format("_areaMaster@{0:X5}",id<<8);

            this.className = className;

            numXBlocks = (maxX - minX) / boundingGridSize;
            numYBlocks = (maxY - minY) / boundingGridSize;
            mActorBlock = new List<Actor>[numXBlocks, numYBlocks];
            halfWidth = numXBlocks / 2;
            halfHeight = numYBlocks / 2;

            for (int y = 0; y < numYBlocks; y++)
            {
                for (int x = 0; x < numXBlocks; x++ )
                {
                    mActorBlock[x, y] = new List<Actor>();
                }
            }
            
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList(classPath, false, true, zoneName, "/Area/Zone/ZoneDefault", -1, (byte)1, true, false, false, false, false, false, false, false);
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, "ZoneDefault", lParams);
        }

        public override BasePacket GetSpawnPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(playerActorId, 0));            
            subpackets.Add(CreateSpeedPacket(playerActorId));
            subpackets.Add(CreateSpawnPositonPacket(playerActorId, 0x1));
            subpackets.Add(CreateNamePacket(playerActorId));
            subpackets.Add(CreateStatePacket(playerActorId));
            subpackets.Add(CreateIsZoneingPacket(playerActorId));
            subpackets.Add(CreateScriptBindPacket(playerActorId));
            return BasePacket.CreatePacket(subpackets, true, false);
        }

        #region Actor Management

        public void AddActorToZone(Actor actor)
        {
            if (!mActorList.ContainsKey(actor.actorId))
                mActorList.Add(actor.actorId, actor);

            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionZ / boundingGridSize;

            gridX += halfWidth;
            gridY += halfHeight;

            //Boundries
            if (gridX < 0)
                gridX = 0;
            if (gridX >= numXBlocks)
                gridX = numXBlocks - 1;
            if (gridY < 0)
                gridY = 0;
            if (gridY >= numYBlocks)
                gridY = numYBlocks - 1;

            lock (mActorBlock)
                mActorBlock[gridX, gridY].Add(actor);
        }

        public void RemoveActorFromZone(Actor actor)
        {
            mActorList.Remove(actor.actorId);

            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionZ / boundingGridSize;

            gridX += halfWidth;
            gridY += halfHeight;

            //Boundries
            if (gridX < 0)
                gridX = 0;
            if (gridX >= numXBlocks)
                gridX = numXBlocks - 1;
            if (gridY < 0)
                gridY = 0;
            if (gridY >= numYBlocks)
                gridY = numYBlocks - 1;

            lock (mActorBlock)
                mActorBlock[gridX, gridY].Remove(actor);
        }

        public void UpdateActorPosition(Actor actor)
        {
            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionZ / boundingGridSize;

            gridX += halfWidth;
            gridY += halfHeight;

            //Boundries
            if (gridX < 0)
                gridX = 0;
            if (gridX >= numXBlocks)
                gridX = numXBlocks - 1;
            if (gridY < 0)
                gridY = 0;
            if (gridY >= numYBlocks)
                gridY = numYBlocks - 1;

            int gridOldX = (int)actor.oldPositionX / boundingGridSize;
            int gridOldY = (int)actor.oldPositionZ / boundingGridSize;

            gridOldX += halfWidth;
            gridOldY += halfHeight;

            //Boundries
            if (gridOldX < 0)
                gridOldX = 0;
            if (gridOldX >= numXBlocks)
                gridOldX = numXBlocks - 1;
            if (gridOldY < 0)
                gridOldY = 0;
            if (gridOldY >= numYBlocks)
                gridOldY = numYBlocks - 1;

            //Still in same block
            if (gridX == gridOldX && gridY == gridOldY)
                return;

            lock (mActorBlock)
            {
                mActorBlock[gridOldX, gridOldY].Remove(actor);
                mActorBlock[gridX, gridY].Add(actor);
            }
        }

        public List<Actor> GetActorsAroundPoint(float x, float y, int checkDistance)
        {
            checkDistance /= boundingGridSize;

            int gridX = (int)x/boundingGridSize;
            int gridY = (int)y/boundingGridSize;

            gridX += halfWidth;
            gridY += halfHeight;

            //Boundries
            if (gridX < 0)
                gridX = 0;
            if (gridX >= numXBlocks)
                gridX = numXBlocks - 1;
            if (gridY < 0)
                gridY = 0;
            if (gridY >= numYBlocks)
                gridY = numYBlocks - 1;

            List<Actor> result = new List<Actor>();

            for (int gx = gridX - checkDistance; gx <= gridX + checkDistance; gx++)
            {
                for (int gy = gridY - checkDistance; gy <= gridY + checkDistance; gy++)
                {
                    result.AddRange(mActorBlock[gx, gy]);
                }
            }

            //Remove players if isolation zone
            if (isIsolated)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i] is Player)
                        result.RemoveAt(i);
                }
            }

            return result;
        }

        public List<Actor> GetActorsAroundActor(Actor actor, int checkDistance)
        {
            checkDistance /= boundingGridSize;

            int gridX = (int)actor.positionX / boundingGridSize;
            int gridY = (int)actor.positionZ / boundingGridSize;

            gridX += halfWidth;
            gridY += halfHeight;

            //Boundries
            if (gridX < 0)
                gridX = 0;
            if (gridX >= numXBlocks)
                gridX = numXBlocks - 1;
            if (gridY < 0)
                gridY = 0;
            if (gridY >= numYBlocks)
                gridY = numYBlocks - 1;

            List<Actor> result = new List<Actor>();

            for (int gy = ((gridY - checkDistance) < 0 ? 0 : (gridY - checkDistance)); gy <= ((gridY + checkDistance) >= numYBlocks ? numYBlocks - 1 : (gridY + checkDistance)); gy++)
            {
                for (int gx = ((gridX - checkDistance) < 0 ? 0 : (gridX - checkDistance)); gx <= ((gridX + checkDistance) >= numXBlocks ? numXBlocks - 1 : (gridX + checkDistance)); gx++)
                {
                    result.AddRange(mActorBlock[gx, gy]);
                }
            }

            //Remove players if isolation zone
            if (isIsolated)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i] is Player)
                        result.RemoveAt(i);
                }
            }

            return result;
        }

        #endregion

        public Actor FindActorInZone(uint id)
        {
            if (!mActorList.ContainsKey(id))
                return null;
            return mActorList[id];
        }

        public Player FindPCInZone(string name)
        {
            foreach (Actor a in mActorList.Values)
            {
                if (a is Player)
                {
                    if (((Player)a).customDisplayName.ToLower().Equals(name.ToLower()))
                        return (Player)a;
                }
            }
            return null;
        }

        public Player FindPCInZone(uint id)
        {
            if (!mActorList.ContainsKey(id))
                return null;
            return (Player)mActorList[id];
        }

        public void Clear()
        {
            //Clear All
            mActorList.Clear();
            for (int y = 0; y < numYBlocks; y++)
            {
                for (int x = 0; x < numXBlocks; x++)
                {
                    mActorBlock[x, y].Clear();
                }
            }
        }

        public void BroadcastPacketAroundActor(Actor actor, SubPacket packet)
        {
            if (isIsolated)
                return;

            List<Actor> aroundActor = GetActorsAroundActor(actor, 50);
            foreach (Actor a in aroundActor)
            {                
                if (a is Player)
                {
                    if (isIsolated && packet.header.sourceId != a.actorId)
                        continue;

                    SubPacket clonedPacket = new SubPacket(packet, actor.actorId);
                    Player p = (Player)a;                        
                    p.QueuePacket(clonedPacket);
                }
            }            
        }

        public void SpawnActor(SpawnLocation location)
        {
            ActorClass actorClass = Server.GetWorldManager().GetActorClass(location.classId);
            
            if (actorClass == null)
                return;

            Npc npc = new Npc(mActorList.Count + 1, actorClass, location.uniqueId, actorId, location.x, location.y, location.z, location.rot, location.state, location.animId, null);
            npc.LoadEventConditions(actorClass.eventConditions);            

            AddActorToZone(npc);                          
        }

        public void ChangeWeather(ushort weather, ushort transitionTime, Player player, bool zoneWide = false)
        {
            weatherNormal = weather;

            if (player != null && !zoneWide)
            {
                player.QueuePacket(BasePacket.CreatePacket(SetWeatherPacket.BuildPacket(player.actorId, weather, transitionTime), true, false));
            }
            if (zoneWide)
            {
                foreach (var actor in mActorList)
                {
                    if (actor.Value is Player)
                    {
                        player = ((Player)actor.Value);
                        player.QueuePacket(BasePacket.CreatePacket(SetWeatherPacket.BuildPacket(player.actorId, weather, transitionTime), true, false));
                    }
                }
            }
        }

        public void Update(double deltaTime)
        {
            lock (mActorList)
            {
                foreach (Actor a in mActorList.Values)
                    a.Update(deltaTime);
            }
        }
    }
}

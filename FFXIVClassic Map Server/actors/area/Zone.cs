using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Zone : Actor
    {
        public string zoneName;        
        public ushort regionId;
        public bool canStealth, isInn, canRideChocobo, isInstanceRaid;
        public ushort weatherNormal, weatherCommon, weatherRare;
        public ushort bgmDay, bgmNight, bgmBattle;
        
        public int boundingGridSize = 50;
        public int minX = -1000, minY = -1000, maxX = 1000, maxY = 1000;
        private int numXBlocks, numYBlocks;
        private int halfWidth, halfHeight;

        private Dictionary<uint, Actor> mActorList = new Dictionary<uint,Actor>();
        private List<Actor>[,] mActorBlock;

        public Zone(uint id, string zoneName, ushort regionId, ushort bgmDay, ushort bgmNight, ushort bgmBattle, bool canStealth, bool isInn, bool canRideChocobo, bool isInstanceRaid)
            : base(id)
        {

            this.zoneName = zoneName;
            this.regionId = regionId;
            this.canStealth = canStealth;
            this.isInn = isInn;
            this.canRideChocobo = canRideChocobo;
            this.isInstanceRaid = isInstanceRaid;

            this.bgmDay = bgmDay;
            this.bgmNight = bgmNight;
            this.bgmBattle = bgmBattle;

            this.displayNameId = 0;
            this.customDisplayName = "_areaMaster";
            this.actorName = String.Format("_areaMaster@{0:X5}",id<<8);

            this.className = "ZoneMasterPrvI0";

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

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.createLuaParamList("/Area/Zone/ZoneMasterPrvI0", false, true, zoneName, "", 0xFFFFFFFF, false, false, canStealth, isInn, false, false, false, false, false, false);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }

        public override BasePacket getSpawnPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId, 0));            
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0x1));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));
            return BasePacket.createPacket(subpackets, true, false);
        }

        #region Actor Management

        public void addActorToZone(Actor actor)
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

        public void removeActorToZone(Actor actor)
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

        public void updateActorPosition(Actor actor)
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

        public List<Actor> getActorsAroundPoint(float x, float y, int checkDistance)
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

            return result;
        }

        public List<Actor> getActorsAroundActor(Actor actor, int checkDistance)
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
                    if (((Player)a).customDisplayName.Equals(name))
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

        public void clear()
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

        public void broadcastPacketAroundActor(Actor actor, SubPacket packet)
        {
            if (zone != null)
            {
                List<Actor> aroundActor = getActorsAroundActor(actor, 50);
                foreach (Actor a in aroundActor)
                {
                    if (a is Player)
                    {
                        SubPacket clonedPacket = new SubPacket(packet, actor.actorId);
                        Player p = (Player)a;                        
                        p.queuePacket(clonedPacket);
                    }
                }
            }
        }

    }
}

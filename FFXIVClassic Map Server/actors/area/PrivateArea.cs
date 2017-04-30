
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.area
{
    class PrivateArea : Area    
    {
        private Zone parentZone;
        private string privateAreaName;
        private uint privateAreaType;

        public PrivateArea(Zone parent, uint id, string classPath, string privateAreaName, uint privateAreaType, ushort bgmDay, ushort bgmNight, ushort bgmBattle)
            : base(id, parent.zoneName, parent.regionId, classPath, bgmDay, bgmNight, bgmBattle, parent.isIsolated, parent.isInn, parent.canRideChocobo, parent.canStealth, true)
        {
            this.parentZone = parent;
            this.zoneName = parent.zoneName;
            this.privateAreaName = privateAreaName;
            this.privateAreaType = privateAreaType;
        }

        public string GetPrivateAreaName()
        {
            return privateAreaName;
        }

        public uint GetPrivateAreaType()
        {
            return privateAreaType;
        }

        public Zone GetParentZone()
        {
            return parentZone;
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            string path = className;

            string realClassName = className.Substring(className.LastIndexOf("/") + 1);

            lParams = LuaUtils.CreateLuaParamList(classPath, false, true, zoneName, privateAreaName, privateAreaType, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, false, false, false);
            ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, realClassName, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, realClassName, lParams);
        }


        public void AddSpawnLocation(SpawnLocation spawn)
        {
            mSpawnLocations.Add(spawn);
        }

        public void SpawnAllActors()
        {
            foreach (SpawnLocation spawn in mSpawnLocations)
                SpawnActor(spawn);
        }
    }
}


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
        private uint privateAreaLevel;

        public PrivateArea(Zone parent, uint id, string className, string privateAreaName, uint privateAreaLevel, ushort bgmDay, ushort bgmNight, ushort bgmBattle)
            : base(id, parent.zoneName, parent.regionId, className, bgmDay, bgmNight, bgmBattle, parent.isIsolated, parent.isInn, parent.canRideChocobo, parent.canStealth, true)
        {
            this.parentZone = parent;
            this.privateAreaName = privateAreaName;
            this.privateAreaLevel = privateAreaLevel;
        }

        public string GetPrivateAreaName()
        {
            return privateAreaName;
        }

        public uint GetPrivateAreaLevel()
        {
            return privateAreaLevel;
        }

        public Zone GetParentZone()
        {
            return parentZone;
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;

            string path = className;

            if (className.ToLower().Contains("content"))
                path = "Content/" + className;

            lParams = LuaUtils.CreateLuaParamList("/Area/PrivateArea/" + path, false, true, zoneName, privateAreaName, 0x9E, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, false, false, false);
            ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams).DebugPrintSubPacket();
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);
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

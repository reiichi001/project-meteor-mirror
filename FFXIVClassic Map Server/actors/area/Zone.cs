using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors.chara.npc;
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
    class Zone : Area
    {        
        Dictionary<string, Dictionary<uint, PrivateArea>> privateAreas = new Dictionary<string, Dictionary<uint, PrivateArea>>();

        public Zone(uint id, string zoneName, ushort regionId, string className, ushort bgmDay, ushort bgmNight, ushort bgmBattle, bool isIsolated, bool isInn, bool canRideChocobo, bool canStealth, bool isInstanceRaid)
            : base(id, zoneName, regionId, className, bgmDay, bgmNight, bgmBattle, isIsolated, isInn, canRideChocobo, canStealth, isInstanceRaid)
        {

        }

        public void addPrivateArea(PrivateArea pa)
        {
            if (privateAreas.ContainsKey(pa.getPrivateAreaName()))
                privateAreas[pa.getPrivateAreaName()][0] = pa;
            else
            {
                privateAreas[pa.getPrivateAreaName()] = new Dictionary<uint, PrivateArea>();
                privateAreas[pa.getPrivateAreaName()][0] = pa;
            }
        }

        public PrivateArea getPrivateArea(string type, uint number)
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

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            bool isEntranceDesion = false;

            List<LuaParam> lParams;
            lParams = LuaUtils.createLuaParamList("/Area/Zone/" + className, false, true, zoneName, "", -1, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, true, isInstanceRaid, isEntranceDesion);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);        
        }

        public void addSpawnLocation(SpawnLocation spawn)
        {
            //Is it in a private area?
            if (!spawn.privAreaName.Equals(""))
            {
                if (privateAreas.ContainsKey(spawn.privAreaName))
                {
                    Dictionary<uint, PrivateArea> levels = privateAreas[spawn.privAreaName];
                    if (levels.ContainsKey(spawn.privAreaLevel))
                        levels[spawn.privAreaLevel].addSpawnLocation(spawn);
                    else
                        Log.error(String.Format("Tried to add a spawn location to non-existing private area level \"{0}\" in area {1} in zone {2}", spawn.privAreaName, spawn.privAreaLevel, zoneName));
                }
                else
                    Log.error(String.Format("Tried to add a spawn location to non-existing private area \"{0}\" in zone {1}", spawn.privAreaName, zoneName));
            }
            else            
                mSpawnLocations.Add(spawn);            
        }

        public void spawnAllActors(bool doPrivAreas)
        {
            foreach (SpawnLocation spawn in mSpawnLocations)            
                spawnActor(spawn);

            if (doPrivAreas)
            {
                foreach (Dictionary<uint, PrivateArea> areas in privateAreas.Values)
                {
                    foreach (PrivateArea pa in areas.Values)
                        pa.spawnAllActors();
                }
            }
        }

    }
}

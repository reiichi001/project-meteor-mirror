using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.actors.area
{
    class Zone : Area
    {
        Dictionary<string, Dictionary<uint, PrivateArea>> privateAreas = new Dictionary<string, Dictionary<uint, PrivateArea>>();

        public Zone(uint id, string zoneName, ushort regionId, string className, ushort bgmDay, ushort bgmNight, ushort bgmBattle, bool isIsolated, bool isInn, bool canRideChocobo, bool canStealth, bool isInstanceRaid)
            : base(id, zoneName, regionId, className, bgmDay, bgmNight, bgmBattle, isIsolated, isInn, canRideChocobo, canStealth, isInstanceRaid)
        {

        }

        public void AddPrivateArea(PrivateArea pa)
        {
            if (privateAreas.ContainsKey(pa.GetPrivateAreaName()))
                privateAreas[pa.GetPrivateAreaName()][0] = pa;
            else
            {
                privateAreas[pa.GetPrivateAreaName()] = new Dictionary<uint, PrivateArea>();
                privateAreas[pa.GetPrivateAreaName()][0] = pa;
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

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            bool isEntranceDesion = false;

            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/Area/Zone/" + className, false, true, zoneName, "", -1, canRideChocobo ? (byte)1 : (byte)0, canStealth, isInn, false, false, false, true, isInstanceRaid, isEntranceDesion);
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);        
        }

    }
}

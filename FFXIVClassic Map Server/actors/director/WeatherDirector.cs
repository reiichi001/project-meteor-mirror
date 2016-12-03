
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class WeatherDirector : Director
    {
        private uint weatherId;

        public WeatherDirector(Player player, uint weatherId)
            : base(player, 0x5FF80003)
        {
            this.weatherId = weatherId;

            this.displayNameId = 0;

            this.customDisplayName = String.Format("weatherDire_{0}_{1}", player.zone.zoneName, "07");

            this.actorName = String.Format("weatherDire_{0}_{1}@{2:x3}{3:x2}", player.zone.zoneName, "04", player.zoneId, 0);
            
            this.className = "Debug";
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/Director/Weather/WeatherDirector", false, false, false, false, weatherId);
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);
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
    }
}

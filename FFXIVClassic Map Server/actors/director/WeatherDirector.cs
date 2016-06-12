using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.createLuaParamList("/Director/Weather/WeatherDirector", false, false, false, false, weatherId);
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
    }
}

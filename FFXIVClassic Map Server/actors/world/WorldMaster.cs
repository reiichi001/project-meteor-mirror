using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class WorldMaster : Actor
    {
        public WorldMaster() : base(0x5FF80001)
        {
            this.displayNameId = 0;
            this.customDisplayName = "worldMaster";

            this.actorName = "worldMaster";
            this.className = "WorldMaster";
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/World/WorldMaster_event", false, false, false, false, false, null);
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

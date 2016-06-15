using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class DebugProg : Actor
    {

        public DebugProg()
            : base(0x5FF80002)
        {
            this.displayNameId = 0;
            this.customDisplayName = "debug";

            this.actorName = "debug";
            this.className = "Debug";
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/System/Debug.prog", false, false, false, false, true, 0xC51F, true, true);
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

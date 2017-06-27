using FFXIVClassic.Common;
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

        public override SubPacket CreateScriptBindPacket()
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/System/Debug.prog", false, false, false, false, true, 0xC51F, true, true);
            return ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams);
        }

        public override BasePacket GetSpawnPackets()
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(0));            
            subpackets.Add(CreateSpeedPacket());
            subpackets.Add(CreateSpawnPositonPacket(0x1));
            subpackets.Add(CreateNamePacket());
            subpackets.Add(CreateStatePacket());
            subpackets.Add(CreateIsZoneingPacket());
            subpackets.Add(CreateScriptBindPacket());
            return BasePacket.CreatePacket(subpackets, true, false);
        }

    }
}

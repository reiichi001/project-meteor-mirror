using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.director
{
    class Director : Actor
    {
        Player owner;

        public Director(Player owner, uint id) : base(id)
        {
            this.owner = owner;
        }

        public virtual BasePacket getSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId, 0));
            subpackets.AddRange(getEventConditionPackets(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));
            return BasePacket.createPacket(subpackets, true, false);
        }        

        public override BasePacket getInitPackets(uint playerActorId)
        {
            SetActorPropetyPacket initProperties = new SetActorPropetyPacket("/_init");
            initProperties.addTarget();
            return BasePacket.createPacket(initProperties.buildPacket(playerActorId, actorId), true, false);
        }

        public void onTalked(Npc npc)
        {
            LuaEngine.doDirectorOnTalked(this, owner, npc);
        }

        public void onCommand(Command command)
        {
            LuaEngine.doDirectorOnCommand(this, owner, command);
        }

    }    
}

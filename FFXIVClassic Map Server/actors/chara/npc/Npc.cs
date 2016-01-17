using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara.npc
{
    class Npc : Character
    {
        public Npc(uint id, string actorName, uint displayNameId, string customDisplayName, float positionX, float positionY, float positionZ, float rotation, uint animationId, string className, byte[] initParams)
            : base(id)
        {
            this.actorName = actorName;
            this.displayNameId = displayNameId;
            this.customDisplayName = customDisplayName;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
            this.rotation = rotation;
            this.animationId = animationId;
            this.className = className;

            if (initParams.Length != 0)
                this.classParams = LuaUtils.readLuaParams(initParams);

        }

        public override BasePacket getInitPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0x1));
            subpackets.Add(createAppearancePacket(playerActorId));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(_0xFPacket.buildPacket(playerActorId, playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIdleAnimationPacket(playerActorId));
            subpackets.Add(createInitStatusPacket(playerActorId));
            subpackets.Add(createSetActorIconPacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));

            return BasePacket.createPacket(subpackets, true, false);
        }

    }
}

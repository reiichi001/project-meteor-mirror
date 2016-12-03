
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.actors.director
{
    class OpeningDirector : Director
    {
        public OpeningDirector(Player player, uint id) : base(player, id)
        {
            this.displayNameId = 0;
            this.customDisplayName = String.Format("openingDire_{0}_{1}", player.zone.zoneName, "04");

            this.actorName = String.Format("openingDire_{0}_{1}@{2:x3}{3:x2}", player.zone.zoneName, "04", player.zoneId, 0);

            this.actorName = this.actorName.Replace("Battle", "Btl");

            this.className = "OpeningDirector";

            this.eventConditions = new EventList();

            List<EventList.NoticeEventCondition> noticeEventList = new List<EventList.NoticeEventCondition>();

            noticeEventList.Add(new EventList.NoticeEventCondition("noticeEvent", 0xE, 0x0));
            noticeEventList.Add(new EventList.NoticeEventCondition("noticeRequest", 0x0, 0x1));

            this.eventConditions.noticeEventConditions = noticeEventList;
        }

        public override SubPacket CreateScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.CreateLuaParamList("/Director/OpeningDirector", false, false, false, false, 0x13881);
            return ActorInstantiatePacket.BuildPacket(actorId, playerActorId, actorName, className, lParams);
        }
    }
}

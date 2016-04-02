using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.director
{
    class OpeningDirector : Director
    {
        public OpeningDirector(uint id) : base(id)
        {
            this.displayNameId = 0;
            this.customDisplayName = "openingDire";

            this.actorName = "openingDire";
            this.className = "OpeningDirector";

            this.eventConditions = new EventList();

            List<EventList.NoticeEventCondition> noticeEventList = new List<EventList.NoticeEventCondition>();

            noticeEventList.Add(new EventList.NoticeEventCondition("noticeEvent", 0xE, 0x0));
            noticeEventList.Add(new EventList.NoticeEventCondition("noticeRequest", 0x0, 0x1));

            this.eventConditions.noticeEventConditions = noticeEventList;
        }

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.createLuaParamList("/Director/OpeningDirector", false, false, false, false, 0x13881);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }
    }
}

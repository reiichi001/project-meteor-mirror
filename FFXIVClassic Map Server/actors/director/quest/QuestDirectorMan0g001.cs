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
    class QuestDirectorMan0g001 : Director
    {
        public QuestDirectorMan0g001(Player player, uint id)
            : base(player, id)
        {
            this.displayNameId = 0;
            this.customDisplayName = "questDirect_fst0Btl03_01";

            this.actorName = "questDirect_fst0Btl03_01@0A615";
            this.className = "QuestDirectorMan0g001";

            this.eventConditions = new EventList();

            List<EventList.NoticeEventCondition> noticeEventList = new List<EventList.NoticeEventCondition>();

            noticeEventList.Add(new EventList.NoticeEventCondition("noticeEvent", 0xE, 0x0));
            noticeEventList.Add(new EventList.NoticeEventCondition("noticeRequest", 0x0, 0x1));

            this.eventConditions.noticeEventConditions = noticeEventList;
        }

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            lParams = LuaUtils.createLuaParamList("/Director/Quest/QuestDirectorMan0g001", false, false, false, false, false, 0x753A);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }
    }
}

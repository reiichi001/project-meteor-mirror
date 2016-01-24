using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors
{
    class EventList
    {
        public List<TalkEventCondition> talkEventConditions;
        public List<NoticeEventCondition> noticeEventConditions;
        public List<EmoteEventCondition> emoteEventConditions;
        public List<PushCircleEventCondition> pushWithCircleEventConditions;
        public List<PushFanEventCondition> pushWithFanEventConditions;
        public List<PushBoxEventCondition> pushWithBoxEventConditions;

        public class TalkEventCondition
        {
            public byte unknown1;
            public byte unknown2;
            public string conditionName;
        }

        public class NoticeEventCondition
        {
            public byte unknown1;
            public byte unknown2;
            public string conditionName;
        }

        public class EmoteEventCondition
        {
            public byte unknown1;
            public byte unknown2;
            public byte emoteId;
            public string conditionName;
        }

        public class PushCircleEventCondition
        {
            public byte unknown1;
            public byte unknown2;
            public byte unknown4;
            public byte emoteId;
            public float radius;
            public int unknown5;
            public float unknown6;
            public string conditionName;
        }

        public class PushFanEventCondition
        {
            byte unknown1;
            byte unknown2;
            byte emoteId;
            string conditionName;
        }

        public class PushBoxEventCondition
        {
            byte unknown1;
            byte unknown2;
            byte emoteId;
            string conditionName;
        }
    }
}

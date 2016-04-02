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
            public bool isDisabled = false;
            public string conditionName;
        }

        public class NoticeEventCondition
        {
            public byte unknown1;
            public byte unknown2;
            public string conditionName;

            public NoticeEventCondition(string name, byte unk1, byte unk2)
            {
                conditionName = name;
                unknown1 = unk1;
                unknown2 = unk2;
            }
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
            public string conditionName = "";
            public float radius = 30.0f;
            public bool outwards = false;
            public bool silent = true;
        }

        public class PushFanEventCondition
        {
            public string conditionName;
            public float radius = 30.0f;
            public bool outwards = false;
            public bool silent = true;
        }

        public class PushBoxEventCondition
        {
            public string conditionName = "";
            public float size = 30.0f;
            public bool outwards = false;
            public bool silent = true;
        }
    }
}

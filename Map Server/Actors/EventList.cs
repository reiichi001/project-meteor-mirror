/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System.Collections.Generic;

namespace Meteor.Map.actors
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
            public uint bgObj;
            public uint layout;
            public string conditionName = "";
            public string reactName = "";
            public bool outwards = false;
            public bool silent = true;
        }
    }
}

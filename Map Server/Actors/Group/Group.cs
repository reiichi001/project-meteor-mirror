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

using Meteor.Common;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.send.group;
using Meteor.Map.packets.send.groups;
using System;
using System.Collections.Generic;

namespace Meteor.Map.actors.group
{
    class Group
    {
        public const uint PlayerPartyGroup = 10001;
        public const uint CompanyGroup = 20002;

        public const uint GroupInvitationRelationGroup = 50001;
        public const uint TradeRelationGroup = 50002;
        public const uint BazaarBuyItemRelationGroup = 50009;

        public const uint RetainerGroup = 80001;

        public const uint MonsterPartyGroup = 10002;

        public const uint ContentGroup_GuildleveGroup = 30001;
        public const uint ContentGroup_PublicPopGroup = 30002;
        public const uint ContentGroup_SimpleContentGroup24A = 30003;
        public const uint ContentGroup_SimpleContentGroup32A = 30004;
        public const uint ContentGroup_SimpleContentGroup128 = 30005;
        public const uint ContentGroup_SimpleContentGroup24B = 30006;
        public const uint ContentGroup_SimpleContentGroup32B = 30007;
        public const uint ContentGroup_RetainerAccessGroup = 30008;
        public const uint ContentGroup_SimpleContentGroup99999 = 30009;
        public const uint ContentGroup_SimpleContentGroup512 = 30010;
        public const uint ContentGroup_SimpleContentGroup64A = 30011;
        public const uint ContentGroup_SimpleContentGroup64B = 30012;
        public const uint ContentGroup_SimpleContentGroup64C = 30013;
        public const uint ContentGroup_SimpleContentGroup64D = 30014;
        public const uint ContentGroup_SimpleContentGroup64E = 30015;
        public const uint ContentGroup_SimpleContentGroup64F = 30016;
        public const uint ContentGroup_SimpleContentGroup64G = 30017;
        public const uint ContentGroup_SimpleContentGroup24C = 30018;

        public readonly ulong groupIndex;
        
        public Group(ulong groupIndex)
        {
            this.groupIndex = groupIndex;
        }

        public virtual int GetMemberCount()
        {
            return 0;
        }

        public virtual uint GetTypeId()
        {
            return 0;            
        }

        public virtual string GetGroupName()
        {
            return "";
        }

        public virtual int GetGroupLocalizedName()
        {
            return -1;
        }

        public virtual List<GroupMember> BuildMemberList(uint id)
        {
            return new List<GroupMember>();
        }

        public void SendGroupPacketsAll(params uint[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                Session session = Server.GetServer().GetSession(ids[i]);

                if (session != null)
                    SendGroupPackets(session);
            }
        }

        public void SendGroupPacketsAll(List<uint> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                Session session = Server.GetServer().GetSession(ids[i]);

                if (session != null)
                    SendGroupPackets(session);
            }
        }

        public void SendDeletePackets(params uint[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                Session session = Server.GetServer().GetSession(ids[i]);

                if (session != null)
                    SendDeletePacket(session);
            }
        }

        public void SendDeletePackets(List<uint> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                Session session = Server.GetServer().GetSession(ids[i]);

                if (session != null)
                    SendDeletePacket(session);
            }
        }

        public virtual void SendGroupPackets(Session session)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();
            List<GroupMember> members = BuildMemberList(session.id);

            session.QueuePacket(GroupHeaderPacket.buildPacket(session.id, session.GetActor().zoneId, time, this));
            session.QueuePacket(GroupMembersBeginPacket.buildPacket(session.id, session.GetActor().zoneId, time, this));

            int currentIndex = 0;

            while (true)
            {
                int memberCount = Math.Min(GetMemberCount(), members.Count);
                if (memberCount - currentIndex >= 64)
                    session.QueuePacket(GroupMembersX64Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex >= 32)
                    session.QueuePacket(GroupMembersX32Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex >= 16)
                    session.QueuePacket(GroupMembersX16Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex > 0)
                    session.QueuePacket(GroupMembersX08Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex));
                else
                    break;
            }
            
            session.QueuePacket(GroupMembersEndPacket.buildPacket(session.id, session.GetActor().zoneId, time, this));

        }

        public void SendDeletePacket(Session session)
        {            
            if (session != null)
                session.QueuePacket(DeleteGroupPacket.buildPacket(session.id, this));
        }

        public virtual void SendInitWorkValues(Session session)
        {

        }
    }
}

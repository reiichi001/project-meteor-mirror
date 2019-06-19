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

using System;
using System.Collections.Generic;

using Meteor.Common;
using Meteor.World.Packets.Send.Subpackets.Groups;

namespace Meteor.World.DataObjects.Group
{
    class Group
    {
        public const uint PlayerPartyGroup = 10001;
        public const uint CompanyGroup = 20002;

        public const uint GroupInvitationRelationGroup = 50001;
        public const uint TradeRelationGroup = 50002;
        public const uint RetainerMeetingRelationGroup = 50003;
        public const uint BazaarBuyItemRelationGroup = 50009;

        public const uint RetainerGroup = 80001;

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

        public void SendGroupPacketsAll(params uint[] sessionIds)
        {
            for (int i = 0; i < sessionIds.Length; i++)
            {
                Session session = Server.GetServer().GetSession(sessionIds[i]);

                if (session != null)
                    SendGroupPackets(session);
            }
        }

        public void SendGroupPacketsAll(List<uint> sessionIds)
        {
            for (int i = 0; i < sessionIds.Count; i++)
            {
                Session session = Server.GetServer().GetSession(sessionIds[i]);

                if (session != null)
                    SendGroupPackets(session);
            }
        }

        public void SendDeletePackets(params uint[] sessionIds)
        {
            for (int i = 0; i < sessionIds.Length; i++)
            {
                Session session = Server.GetServer().GetSession(sessionIds[i]);

                if (session != null)
                    SendDeletePacket(session);
            }
        }

        public void SendDeletePackets(List<uint> sessionIds)
        {
            for (int i = 0; i < sessionIds.Count; i++)
            {
                Session session = Server.GetServer().GetSession(sessionIds[i]);

                if (session != null)
                    SendDeletePacket(session);
            }
        }

        public void SendGroupPackets(Session session)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();
            List<GroupMember> members = BuildMemberList(session.sessionId);

            session.clientConnection.QueuePacket(GroupHeaderPacket.buildPacket(session.sessionId, session.currentZoneId, time, this));
            session.clientConnection.QueuePacket(GroupMembersBeginPacket.buildPacket(session.sessionId, session.currentZoneId, time, this));

            int currentIndex = 0;

            while (true)
            {
                int memberCount = Math.Min(GetMemberCount(), members.Count);
                if (memberCount - currentIndex >= 64)
                    session.clientConnection.QueuePacket(GroupMembersX64Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex >= 32)
                    session.clientConnection.QueuePacket(GroupMembersX32Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex >= 16)
                    session.clientConnection.QueuePacket(GroupMembersX16Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex));
                else if (memberCount - currentIndex > 0)
                    session.clientConnection.QueuePacket(GroupMembersX08Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex));
                else
                    break;
            }
            
            session.clientConnection.QueuePacket(GroupMembersEndPacket.buildPacket(session.sessionId, session.currentZoneId, time, this));

        }

        public void SendDeletePacket(Session session)
        {            
            if (session != null)
                session.clientConnection.QueuePacket(DeleteGroupPacket.buildPacket(session.sessionId, this));
        }

        public virtual void SendInitWorkValues(Session session)
        {

        }
    }
}

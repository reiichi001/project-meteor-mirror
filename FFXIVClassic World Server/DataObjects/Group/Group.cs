using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server.DataObjects.Group
{
    class Group
    {
        public const uint PlayerPartyGroup = 10001;
        public const uint CompanyGroup = 20002;

        public const uint GroupInvitationRelationGroup = 50001;
        public const uint TradeRelationGroup = 50002;
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

        public void SendGroupPacketsAll(List<uint> sessionIds)
        {
            for (int i = 0; i < sessionIds.Count; i++)
            {
                Session session = Server.GetServer().GetSession(sessionIds[i]);

                if (session != null)
                    SendGroupPackets(session);
            }
        }

        public void SendGroupPackets(Session session)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();
            List<GroupMember> members = BuildMemberList(session.sessionId);

            session.clientConnection.QueuePacket(GroupHeaderPacket.buildPacket(session.sessionId, session.currentZoneId, time, this), true, false);
            session.clientConnection.QueuePacket(GroupMembersBeginPacket.buildPacket(session.sessionId, session.currentZoneId, time, this), true, false);

            int currentIndex = 0;

            while (true)
            {
                if (GetMemberCount() - currentIndex >= 64)
                    session.clientConnection.QueuePacket(GroupMembersX64Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 32)
                    session.clientConnection.QueuePacket(GroupMembersX32Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 16)
                    session.clientConnection.QueuePacket(GroupMembersX16Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex > 0)
                    session.clientConnection.QueuePacket(GroupMembersX08Packet.buildPacket(session.sessionId, session.currentZoneId, time, members, ref currentIndex), true, false);
                else
                    break;
            }
            
            session.clientConnection.QueuePacket(GroupMembersEndPacket.buildPacket(session.sessionId, session.currentZoneId, time, this), true, false);

        }

        public virtual void SendInitWorkValues(Session session)
        {

        }
    }
}

using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.actors.group
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

        public virtual List<GroupMember> BuildMemberList()
        {
            return new List<GroupMember>();
        }

        public void SendGroupPackets(Session session)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();
            List<GroupMember> members = BuildMemberList();

            Server.GetWorldConnection().QueuePacket(GroupHeaderPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);
            Server.GetWorldConnection().QueuePacket(GroupMembersBeginPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);

            int currentIndex = 0;

            while (true)
            {
                if (GetMemberCount() - currentIndex >= 64)
                     Server.GetWorldConnection().QueuePacket(GroupMembersX64Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 32)
                     Server.GetWorldConnection().QueuePacket(GroupMembersX32Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 16)
                     Server.GetWorldConnection().QueuePacket(GroupMembersX16Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex > 0)
                     Server.GetWorldConnection().QueuePacket(GroupMembersX08Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else
                    break;
            }
            
             Server.GetWorldConnection().QueuePacket(GroupMembersEndPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);

        }

        public virtual void SendInitWorkValues(Session session)
        {

        }
    }
}

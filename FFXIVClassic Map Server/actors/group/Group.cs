using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group.work;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class Group
    {
        public const uint PlayerPartyGroup = 10001;
        public const uint CompanyGroup = 20001;

        public const uint GroupInvitationRelationGroup = 50001;
        public const uint TradeRelationGroup = 50002;
        public const uint BazaarBuyItemRelationGroup = 50009;
        
        public const uint RetainerGroup = 80001;

        public ulong groupId;
        public uint groupTypeId;
        public int localizedNamed = -1;
        public string groupName = "";

        public List<GroupMember> members = new List<GroupMember>();

        public Group(ulong id, uint typeId)
        {
            groupId = id;
            groupTypeId = typeId;            
        }

        public Group(ulong id, uint typeId, int nameId, object work)
        {
            groupId = id;
            groupTypeId = typeId;
            localizedNamed = nameId;
        }

        public Group(ulong id, uint typeId, string name, object work)
        {
            groupId = id;
            groupTypeId = typeId;
            groupName = name;
            localizedNamed = -1;
        }

        public void add(Actor actor)
        {
            GroupMember member = new GroupMember(actor.actorId, (int)actor.displayNameId, 0, false, true, actor.customDisplayName);
            members.Add(member);
        }

        public void sendMemberPackets(Player toPlayer)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();

            toPlayer.QueuePacket(GroupHeaderPacket.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, this));
            toPlayer.QueuePacket(GroupMembersBeginPacket.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, this));

            int currentIndex = 0;

            while (true)
            {
                if (members.Count - currentIndex >= 64)
                    toPlayer.QueuePacket(GroupMembersX64Packet.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, members, ref currentIndex));
                else if (members.Count - currentIndex >= 32)
                    toPlayer.QueuePacket(GroupMembersX32Packet.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, members, ref currentIndex));
                else if (members.Count - currentIndex >= 16)
                    toPlayer.QueuePacket(GroupMembersX16Packet.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, members, ref currentIndex));
                else if (members.Count - currentIndex > 0)
                    toPlayer.QueuePacket(GroupMembersX08Packet.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, members, ref currentIndex));               
                else
                    break;
            }


            toPlayer.QueuePacket(GroupMembersEndPacket.buildPacket(toPlayer.actorId, toPlayer.zoneId, time, this));    

        }

        public virtual void sendWorkValues(Player player){}      

    }
}

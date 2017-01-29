using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class ContentGroup : Group
    {
        private List<uint> members = new List<uint>();

        public ContentGroup(ulong groupIndex, uint[] initialMembers) : base(groupIndex)
        {
            for (int i = 0; i < initialMembers.Length; i++)
                members.Add(initialMembers[i]);
        }

        public void AddMember(uint memberId)
        {
            members.Add(memberId);
            SendGroupPacketsAll(members);
        }

        public void RemoveMember(uint memberId)
        {
            members.Remove(memberId);
            SendGroupPacketsAll(members);
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(id).customDisplayName));
            foreach (uint charaId in members)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, Server.GetWorldManager().GetActorInWorld(charaId).customDisplayName));
            }
            return groupMembers;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);            
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id, session.id);
            session.QueuePacket(test, true, false);
        }

        public override uint GetTypeId()
        {
            return Group.ContentGroup_SimpleContentGroup24A;
        }


        public void SendAll()
        {
            SendGroupPacketsAll(members);            
        }

        public void DeleteAll()
        {
            SendDeletePackets(members);
        }

    }
}

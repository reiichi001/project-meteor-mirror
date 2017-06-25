using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.actors.group.Work;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using FFXIVClassic_Map_Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors.group
{
    class ContentGroup : Group
    {
        public ContentGroupWork contentGroupWork = new ContentGroupWork();
        private Director director;
        private List<uint> members = new List<uint>();

        public ContentGroup(ulong groupIndex, Director director, uint[] initialMembers) : base(groupIndex)
        {
            if (initialMembers != null)
            {
                for (int i = 0; i < initialMembers.Length; i++)
                {
                    Session s = Server.GetServer().GetSession(initialMembers[i]);
                    if (s != null)
                        s.GetActor().SetCurrentContentGroup(this);

                    members.Add(initialMembers[i]);
                }
            }

            this.director = director;
            contentGroupWork._globalTemp.director = (ulong)director.actorId << 32;
        }

        public void AddMember(Actor actor)
        {
            if (actor == null)
                return;

            members.Add(actor.actorId);

            if (actor is Character)            
                ((Character)actor).SetCurrentContentGroup(this);          
  
            SendGroupPacketsAll(members);
        }
        
        public void RemoveMember(uint memberId)
        {
            members.Remove(memberId);
            SendGroupPacketsAll(members);
            CheckDestroy();
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();
            groupMembers.Add(new GroupMember(id, -1, 0, false, true, ""));
            foreach (uint charaId in members)
            {
                if (charaId != id)
                    groupMembers.Add(new GroupMember(charaId, -1, 0, false, true, ""));
            }
            return groupMembers;
        }

        public override int GetMemberCount()
        {
            return members.Count;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.addProperty(this, "contentGroupWork._globalTemp.director");
            groupWork.addByte(Utils.MurmurHash2("contentGroupWork.property[0]", 0), 1);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id, session.id);
            test.DebugPrintSubPacket();
            session.QueuePacket(test, true, false);
        }

        public override void SendGroupPackets(Session session)
        {
            ulong time = Utils.MilisUnixTimeStampUTC();
            List<GroupMember> members = BuildMemberList(session.id);

            session.QueuePacket(GroupHeaderPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);
            session.QueuePacket(GroupMembersBeginPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);

            int currentIndex = 0;

            while (true)
            {
                if (GetMemberCount() - currentIndex >= 64)
                    session.QueuePacket(ContentMembersX64Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 32)
                    session.QueuePacket(ContentMembersX32Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex >= 16)
                    session.QueuePacket(ContentMembersX16Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else if (GetMemberCount() - currentIndex > 0)
                    session.QueuePacket(ContentMembersX08Packet.buildPacket(session.id, session.GetActor().zoneId, time, members, ref currentIndex), true, false);
                else
                    break;
            }

            session.QueuePacket(GroupMembersEndPacket.buildPacket(session.id, session.GetActor().zoneId, time, this), true, false);

        }

        public override uint GetTypeId()
        {
            return Group.ContentGroup_SimpleContentGroup24B;
        }


        public void SendAll()
        {
            SendGroupPacketsAll(members);            
        }

        public void DeleteGroup()
        {
            SendDeletePackets();
            for (int i = 0; i < members.Count; i++)
            {
                Session s = Server.GetServer().GetSession(members[i]);
                if (s != null)
                    s.GetActor().SetCurrentContentGroup(null);
                members.Remove(members[i]);
            }
            Server.GetWorldManager().DeleteContentGroup(groupIndex);
        }

        public void CheckDestroy()
        {
            bool foundSession = false;
            foreach (uint memberId in members)
            {
                Session session = Server.GetServer().GetSession(memberId);
                if (session != null)
                {
                    foundSession = true;
                    break;
                }
            }

            if (!foundSession)
                DeleteGroup();
        }

    }
}

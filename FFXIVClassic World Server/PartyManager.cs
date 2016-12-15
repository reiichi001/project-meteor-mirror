using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server
{
    class PartyManager
    {
        private Server mServer;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Party> mPartyList = new Dictionary<ulong, Party>();

        public PartyManager(Server server, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mServer = server;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        public void CreateParty(uint leaderCharaId)
        {
            lock (mGroupLockReference)
            {
                ulong groupId = mServer.GetGroupIndex();
                Party party = new Party(groupId, leaderCharaId);
                mPartyList.Add(groupId, party);
                mCurrentWorldGroupsReference.Add(groupId, party);
                mServer.IncrementGroupIndex();
            }
        }

        public void DeleteParty(ulong groupId)
        {     
            if (mCurrentWorldGroupsReference.ContainsKey(groupId))
                mCurrentWorldGroupsReference.Remove(groupId);
            if (mPartyList.ContainsKey(groupId))
                mPartyList.Remove(groupId);
        }

        public bool AddToParty(ulong groupId, uint charaId)
        {
            if (mPartyList.ContainsKey(groupId))
            {
                Party party = mPartyList[groupId];
                if (!party.members.Contains(charaId))
                    party.members.Add(charaId);
                return true;
            }
            return false;
        }

        public int RemoveFromParty(ulong groupId, uint charaId)
        {
            if (mPartyList.ContainsKey(groupId))
            {
                Party party = mPartyList[groupId];
                if (party.members.Contains(charaId))
                {                    
                    party.members.Remove(charaId);
                    
                    //If current ldr, make a new ldr if not empty pt
                    if (party.GetLeader() == charaId && party.members.Count != 0)
                        party.SetLeader(party.members[0]);
                }
                return party.members.Count;
            }
            return -1;
        }

        public bool ChangeLeader(ulong groupId, uint charaId)
        {
            if (mPartyList.ContainsKey(groupId))
            {
                Party party = mPartyList[groupId];
                if (party.members.Contains(charaId))
                {
                    party.SetLeader(charaId);
                    return true;
                }                
            }
            return false;
        }
    }
}

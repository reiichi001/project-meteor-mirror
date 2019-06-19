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

using Meteor.World.DataObjects.Group;

namespace Meteor.World
{
    class PartyManager
    {
        private WorldManager mWorldManager;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Party> mPartyList = new Dictionary<ulong, Party>();
        private Dictionary<uint, Party> mPlayerPartyLookup = new Dictionary<uint, Party>();

        public PartyManager(WorldManager worldManager, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mWorldManager = worldManager;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        public Party CreateParty(uint leaderCharaId)
        {
            lock (mGroupLockReference)
            {
                ulong groupId = mWorldManager.GetGroupIndex();
                Party party = new Party(groupId, leaderCharaId);
                mPartyList.Add(groupId, party);
                mPlayerPartyLookup.Add(leaderCharaId, party);
                mCurrentWorldGroupsReference.Add(groupId, party);
                mWorldManager.IncrementGroupIndex();
                return party;
            }
        }

        public void DeleteParty(ulong groupId)
        {     
            if (mCurrentWorldGroupsReference.ContainsKey(groupId))
                mCurrentWorldGroupsReference.Remove(groupId);
            if (mPartyList.ContainsKey(groupId))
            {
                foreach (uint id in mPartyList[groupId].members)
                {
                    if (mPlayerPartyLookup.ContainsKey(id))
                        mPlayerPartyLookup.Remove(id);
                }
                mPartyList.Remove(groupId);
            }
        }

        public bool AddToParty(ulong groupId, uint charaId)
        {
            if (mPartyList.ContainsKey(groupId))
            {
                Party party = mPartyList[groupId];
                if (!party.members.Contains(charaId))
                {
                    party.members.Add(charaId);
                    mPlayerPartyLookup.Remove(charaId);
                    mPlayerPartyLookup.Add(charaId, party);
                }
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
                    mPlayerPartyLookup.Remove(charaId);
                    
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

        public Party GetParty(uint charaId)
        {
            if (mPlayerPartyLookup.ContainsKey(charaId))
                return mPlayerPartyLookup[charaId];
            else
                return CreateParty(charaId);
        }
    }
}

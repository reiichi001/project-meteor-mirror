/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server
{
    class RetainerGroupManager
    {
        private WorldManager mWorldManager;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<uint, RetainerGroup> mRetainerGroupList = new Dictionary<uint, RetainerGroup>();

        public RetainerGroupManager(WorldManager worldManager, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mWorldManager = worldManager;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        public RetainerGroup GetRetainerGroup(uint charaId)
        {
            if (!mRetainerGroupList.ContainsKey(charaId))            
                return LoadRetainerGroup(charaId);            
            else
                return mRetainerGroupList[charaId];
        }

        private RetainerGroup LoadRetainerGroup(uint charaId)
        {
            lock(mGroupLockReference)
            {
                ulong groupId = mWorldManager.GetGroupIndex();
                RetainerGroup retainerGroup = new RetainerGroup(groupId, charaId);

                List<RetainerGroupMember> members = Database.GetRetainers(charaId);
                
                retainerGroup.members = members;                
                mRetainerGroupList.Add(charaId, retainerGroup);
                mCurrentWorldGroupsReference.Add(groupId, retainerGroup);

                mWorldManager.IncrementGroupIndex();

                return retainerGroup;
            }
        }

        public void AddRetainerToGroup(ulong charaId, uint retainerId)
        {

        }

        public void RemoveRetainerFromGroup(ulong charaId, uint retainerId)
        {

        }        
    }
}

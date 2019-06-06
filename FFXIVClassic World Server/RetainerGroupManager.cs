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

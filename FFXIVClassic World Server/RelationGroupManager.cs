using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server
{
    class RelationGroupManager
    {
        private Server mServer;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Relation> mRelationList = new Dictionary<ulong, Relation>();

        public RelationGroupManager(Server server, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mServer = server;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        public void CreateRelationGroup(uint hostCharaId, uint otherCharaId, uint command)
        {
            lock (mGroupLockReference)
            {
                ulong groupIndex = mServer.GetGroupIndex();
                Relation relation = new Relation(groupIndex, hostCharaId, otherCharaId, command);
                mRelationList.Add(groupIndex, relation);
                mCurrentWorldGroupsReference.Add(groupIndex, relation);
                mServer.IncrementGroupIndex();
            }
        }

        public void DeleteRelationGroup(ulong groupId)
        {
            if (mRelationList.ContainsKey(groupId))
                mRelationList.Remove(groupId);
            if (mCurrentWorldGroupsReference.ContainsKey(groupId))
                mCurrentWorldGroupsReference.Remove(groupId);
        }
    }
}

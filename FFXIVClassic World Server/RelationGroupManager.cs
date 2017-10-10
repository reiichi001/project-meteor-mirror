using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_World_Server
{
    class RelationGroupManager
    {
        private WorldManager mWorldManager;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Relation> mPartyRelationList = new Dictionary<ulong, Relation>();
        private Dictionary<ulong, Relation> mLinkshellRelationList = new Dictionary<ulong, Relation>();

        public RelationGroupManager(WorldManager worldManager, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mWorldManager = worldManager;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        public Relation CreatePartyRelationGroup(ulong topicGroupId, uint hostCharaId, uint otherCharaId)
        {
            lock (mGroupLockReference)
            {
                ulong groupIndex = mWorldManager.GetGroupIndex();
                Relation relation = new Relation(groupIndex, hostCharaId, otherCharaId, 10001, topicGroupId);
                mPartyRelationList.Add(groupIndex, relation);
                mCurrentWorldGroupsReference.Add(groupIndex, relation);
                mWorldManager.IncrementGroupIndex();
                return relation;
            }
        }

        public Relation CreateLinkshellRelationGroup(ulong topicGroupId, uint hostCharaId, uint otherCharaId)
        {
            lock (mGroupLockReference)
            {
                ulong groupIndex = mWorldManager.GetGroupIndex();
                Relation relation = new Relation(groupIndex, hostCharaId, otherCharaId, 10002, topicGroupId);
                mLinkshellRelationList.Add(groupIndex, relation);
                mCurrentWorldGroupsReference.Add(groupIndex, relation);
                mWorldManager.IncrementGroupIndex();
                return relation;
            }
        }

        public Relation GetPartyRelationGroup(uint charaId)
        {
            lock (mGroupLockReference)
            {
                foreach (Relation relation in mPartyRelationList.Values)
                {                    
                    if (relation.GetHost() == charaId || relation.GetOther() == charaId)
                        return relation;
                }
                return null;
            }
        }

        public Relation GetLinkshellRelationGroup(uint charaId)
        {
            lock (mGroupLockReference)
            {
                foreach (Relation relation in mLinkshellRelationList.Values)
                {
                    if (relation.GetHost() == charaId || relation.GetOther() == charaId)
                        return relation;
                }
                return null;
            }
        }

        public void DeleteRelationGroup(ulong groupId)
        {
            if (mPartyRelationList.ContainsKey(groupId))
                mPartyRelationList.Remove(groupId);
            if (mLinkshellRelationList.ContainsKey(groupId))
                mLinkshellRelationList.Remove(groupId);
            if (mCurrentWorldGroupsReference.ContainsKey(groupId))
                mCurrentWorldGroupsReference.Remove(groupId);
        }
    }
}

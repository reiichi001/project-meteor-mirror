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

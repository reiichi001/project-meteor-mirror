using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server
{
    class LinkshellManager
    {
        private WorldManager mWorldManager;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Linkshell> mLinkshellList = new Dictionary<ulong, Linkshell>();
        private Dictionary<string, ulong> mNameToIdLookup = new Dictionary<string, ulong>();

        public LinkshellManager(WorldManager worldManager, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mWorldManager = worldManager;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        //Creates a new linkshell and adds it to the list
        public ulong CreateLinkshell(string name, ushort crest, uint master)
        {
            lock (mGroupLockReference)
            {
                ulong resultId = Database.CreateLinkshell(name, crest, master);
                if (resultId >= 0)
                {
                    Linkshell newLs = new Linkshell(resultId, mWorldManager.GetGroupIndex(), name, crest, master, 0xa);

                    //Add founder to the LS
                    if (AddMemberToLinkshell(master, newLs.groupIndex))
                    {
                        mLinkshellList.Add(mWorldManager.GetGroupIndex(), newLs);
                        mNameToIdLookup.Add(newLs.name, newLs.groupIndex);
                        mCurrentWorldGroupsReference.Add(mWorldManager.GetGroupIndex(), newLs);
                        mWorldManager.IncrementGroupIndex();
                    }
                }
                return resultId;
            }
        }

        //Modifies the LS master
        public bool ChangeLinkshellMaster(string name, uint newMaster)
        {
            ulong groupInstanceId;
            if (mNameToIdLookup.ContainsKey(name))
                groupInstanceId = mNameToIdLookup[name];
            else
                return false;

            if (mCurrentWorldGroupsReference.ContainsKey(groupInstanceId))
            {
                Linkshell ls = (Linkshell)mCurrentWorldGroupsReference[groupInstanceId];
                return false;
            }

            return false;
        }

        //Modifies the LS crest
        public bool ChangeLinkshellCrest(string name, ushort newCrestId)
        {
            ulong groupInstanceId;
            if (mNameToIdLookup.ContainsKey(name))
                groupInstanceId = mNameToIdLookup[name];
            else
                return false;

            if (mCurrentWorldGroupsReference.ContainsKey(groupInstanceId))
            {
                Linkshell ls = (Linkshell)mCurrentWorldGroupsReference[groupInstanceId];
                return Database.ChangeLinkshellCrest(ls.dbId, newCrestId);                                    
            }

            return false;
        }

        //Deletes a LS
        public bool DeleteLinkshell(string name)
        {
            lock (mGroupLockReference)
            {
                ulong groupInstanceId;
                if (mNameToIdLookup.ContainsKey(name))
                    groupInstanceId = mNameToIdLookup[name];
                else
                    return false;
                
                if (mCurrentWorldGroupsReference.ContainsKey(groupInstanceId))
                {                
                    Linkshell ls = (Linkshell)mCurrentWorldGroupsReference[groupInstanceId];
                    bool result = Database.DeleteLinkshell(ls.dbId);

                    if (result)
                    {
                        mCurrentWorldGroupsReference.Remove(groupInstanceId);
                        mLinkshellList.Remove(groupInstanceId);
                        mNameToIdLookup.Remove(name);
                        return true;
                    }
                }
            }

            return false;
        }

        //Adds a player to the linkshell
        public bool AddMemberToLinkshell(uint charaId, ulong groupId)
        {
            //Get the LS
            Linkshell ls = GetLinkshell(groupId);
            if (ls == null)
                return false;

            //Add player to ls in db
            lock (mGroupLockReference)
            {
                bool result = Database.LinkshellAddPlayer(ls.dbId, charaId);

                if (result)
                {
                    ls.AddMember(charaId);
                    return true;
                }
                else
                    return false;
            }
        }

        //Removes a player from the linkshell
        public bool RemoveMemberFromLinkshell(uint charaId, ulong groupId)
        {
            //Get the LS
            Linkshell ls = GetLinkshell(groupId);
            if (ls == null)
                return false;

            //Delete the player in the db  
            lock (mGroupLockReference)
            {
                bool result = Database.LinkshellRemovePlayer(ls.dbId, charaId);

                if (!result)
                    return false;

                //Remove from group instance
                ls.RemoveMember(charaId);

                return true;
            }
        }

        //Get a single linkshell group either already instantiated or make one from the db
        public Linkshell GetLinkshell(string name)
        {
            if (mNameToIdLookup.ContainsKey(name))
                return (Linkshell)mCurrentWorldGroupsReference[mNameToIdLookup[name]];
            else
                return null;
        }

        //Get a single linkshell group either already instantiated or make one from the db
        public Linkshell GetLinkshell(ulong lsId)
        {
            if (mLinkshellList.ContainsKey(lsId))
                return mLinkshellList[lsId];
            else
            {
                lock (mGroupLockReference)
                {
                    Linkshell ls = Database.GetLinkshell(mWorldManager.GetGroupIndex(), lsId);
                    ls.LoadMembers();

                    if (ls != null)
                    {                        
                        mLinkshellList.Add(ls.groupIndex, ls);
                        mNameToIdLookup.Add(ls.name, ls.groupIndex);
                        mCurrentWorldGroupsReference.Add(ls.groupIndex, ls);
                        mWorldManager.IncrementGroupIndex();
                        return ls;
                    }
                }
            }
            return null;
        }

        //Get the linkshells player is part of
        public List<Linkshell> GetPlayerLinkshellMembership(uint charaId)
        {
            List<LinkshellMember> memberships = Database.GetPlayerLSMembership(charaId);
            List<Linkshell> linkshells = new List<Linkshell>();
            foreach (LinkshellMember membership in memberships)
                linkshells.Add(GetLinkshell(membership.lsId));
            return linkshells;
        }
        
    }
}

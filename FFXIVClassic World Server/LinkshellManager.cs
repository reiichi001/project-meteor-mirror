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
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference; //GroupId, LS
        private Dictionary<ulong, Linkshell> mLinkshellList = new Dictionary<ulong, Linkshell>(); //GroupId, LS
        private Dictionary<ulong, Linkshell> mLSIdToIdLookup = new Dictionary<ulong, Linkshell>(); //Name, GroupId
        private Dictionary<string, ulong> mNameToIdLookup = new Dictionary<string, ulong>(); //Name, GroupId

        public LinkshellManager(WorldManager worldManager, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mWorldManager = worldManager;
            mGroupLockReference = groupLock;
            mCurrentWorldGroupsReference = worldGroupList;
        }

        //Checks if the LS name is in use or banned
        public int CanCreateLinkshell(string name)
        {
            bool nameBanned = Database.LinkshellIsBannedName(name);
            bool alreadyExists = Database.LinkshellExists(name);

            if (nameBanned)
                return 2;
            if (alreadyExists)
                return 1;
            else
                return 0;
        }

        //Creates a new linkshell and adds it to the list
        public Linkshell CreateLinkshell(string name, ushort crest, uint master)
        {
            lock (mGroupLockReference)
            {
                ulong resultId = Database.CreateLinkshell(name, crest, master);
                if (resultId >= 0)
                {
                    Linkshell newLs = new Linkshell(resultId, mWorldManager.GetGroupIndex(), name, crest, master, 0xa);
                    
                    mLinkshellList.Add(mWorldManager.GetGroupIndex(), newLs);
                    mNameToIdLookup.Add(newLs.name, newLs.groupIndex);
                    mLSIdToIdLookup.Add(newLs.dbId, newLs);
                    mCurrentWorldGroupsReference.Add(mWorldManager.GetGroupIndex(), newLs);
                    mWorldManager.IncrementGroupIndex();

                    //Add founder to the LS
                    AddMemberToLinkshell(master, newLs.name);
                  
                    return newLs;
                }
                return null;
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
        public bool AddMemberToLinkshell(uint charaId, string LSName)
        {
            //Get the LS
            Linkshell ls = GetLinkshell(LSName);
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
        public bool RemoveMemberFromLinkshell(uint charaId, string LSName)
        {
            //Get the LS
            Linkshell ls = GetLinkshell(LSName);
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

        //Get a single linkshell group either already instantiated or make one from the db by Name
        public Linkshell GetLinkshell(string name)
        {
            if (mNameToIdLookup.ContainsKey(name))
                return (Linkshell)mCurrentWorldGroupsReference[mNameToIdLookup[name]];
            else
            {
                lock (mGroupLockReference)
                {
                    Linkshell ls = Database.GetLinkshell(mWorldManager.GetGroupIndex(), name);
                    ls.LoadMembers();

                    if (ls != null)
                    {
                        mLinkshellList.Add(ls.groupIndex, ls);
                        mNameToIdLookup.Add(ls.name, ls.groupIndex);
                        mLSIdToIdLookup.Add(ls.dbId, ls);
                        mCurrentWorldGroupsReference.Add(ls.groupIndex, ls);
                        mWorldManager.IncrementGroupIndex();
                        return ls;
                    }
                    else
                        return null;
                }
            }
        }

        //Get a single linkshell group either already instantiated or make one from the db by ID
        public Linkshell GetLinkshell(ulong lsId)
        {
            if (mLSIdToIdLookup.ContainsKey(lsId))
                return (Linkshell)mCurrentWorldGroupsReference[mLSIdToIdLookup[lsId].groupIndex];
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
                        mLSIdToIdLookup.Add(ls.dbId, ls);
                        mCurrentWorldGroupsReference.Add(ls.groupIndex, ls);
                        mWorldManager.IncrementGroupIndex();
                        return ls;
                    }
                    else
                        return null;
                }
            }
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

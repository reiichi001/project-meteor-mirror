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
        private Server mServer;
        private Object mGroupLockReference;
        private Dictionary<ulong, Group> mCurrentWorldGroupsReference;
        private Dictionary<ulong, Linkshell> mLinkshellList = new Dictionary<ulong, Linkshell>();

        public LinkshellManager(Server server, Object groupLock, Dictionary<ulong, Group> worldGroupList)
        {
            mServer = server;
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
                    Linkshell newLs = new Linkshell(resultId, mServer.GetGroupIndex(), name, crest, master, 0xa);

                    //Add founder to the LS
                    if (AddMemberToLinkshell(master, newLs.groupIndex))
                    {
                        mLinkshellList.Add(mServer.GetGroupIndex(), newLs);
                        mCurrentWorldGroupsReference.Add(mServer.GetGroupIndex(), newLs);
                        mServer.IncrementGroupIndex();
                    }
                }
                return resultId;
            }
        }

        //Modifies the LS
        public bool ModifyLinkshell()
        {
            return false;
        }

        //Creates a new linkshell and adds it to the list
        public bool DeleteLinkshell(ulong groupInstanceId)
        {
            if (mCurrentWorldGroupsReference.ContainsKey(groupInstanceId))
            {
                lock (mGroupLockReference)
                {
                    Linkshell ls = (Linkshell)mCurrentWorldGroupsReference[groupInstanceId];
                    bool result = Database.DeleteLinkshell(ls.dbId);

                    if (result)
                    {
                        mCurrentWorldGroupsReference.Remove(groupInstanceId);
                        mLinkshellList.Remove(groupInstanceId);
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
        public Linkshell GetLinkshell(ulong id)
        {
            if (mLinkshellList.ContainsKey(id))
                return mLinkshellList[id];
            else
            {
                lock (mGroupLockReference)
                {
                    Linkshell ls = Database.GetLinkshell(mServer.GetGroupIndex(), id);
                    ls.LoadMembers();

                    if (ls != null)
                    {                        
                        mLinkshellList.Add(id, ls);
                        mCurrentWorldGroupsReference.Add(mServer.GetGroupIndex(), ls);
                        mServer.IncrementGroupIndex();
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

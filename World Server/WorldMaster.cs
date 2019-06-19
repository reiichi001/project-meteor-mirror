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

using Meteor.Common;
using Meteor.World.DataObjects;
using Meteor.World.DataObjects.Group;
using Meteor.World.Packets.Send.Subpackets;
using Meteor.World.Packets.Send.Subpackets.Groups;
using Meteor.World.Packets.WorldPackets.Send;
using Meteor.World.Packets.WorldPackets.Send.Group;
using MySql.Data.MySqlClient;

namespace Meteor.World
{
    class WorldManager
    {
        private Server mServer;
        public Dictionary<string, ZoneServer> mZoneServerList;
        private Dictionary<uint, ZoneEntrance> zoneEntranceList;

        //World Scope Group Management
        private Object mGroupLock = new object();
        private ulong mRunningGroupIndex = 1;
        private Dictionary<ulong, Group> mCurrentWorldGroups = new Dictionary<ulong, Group>();

        private PartyManager mPartyManager;
        private RetainerGroupManager mRetainerGroupManager;
        private LinkshellManager mLinkshellManager;
        private RelationGroupManager mRelationGroupManager;

        public WorldManager(Server server)
        {
            mServer = server;
            mPartyManager = new PartyManager(this, mGroupLock, mCurrentWorldGroups);
            mLinkshellManager = new LinkshellManager(this, mGroupLock, mCurrentWorldGroups);
            mRetainerGroupManager = new RetainerGroupManager(this, mGroupLock, mCurrentWorldGroups);
            mRelationGroupManager = new RelationGroupManager(this, mGroupLock, mCurrentWorldGroups);
        }

        public void LoadZoneServerList()
        {
            mZoneServerList = new Dictionary<string, ZoneServer>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    serverIp,
                                    serverPort
                                    FROM server_zones 
                                    WHERE serverIp IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0); 
                            string ip = reader.GetString(1);
                            int port = reader.GetInt32(2);
                            string address = ip + ":" + port;

                            if (!mZoneServerList.ContainsKey(address))
                            {
                                ZoneServer zone = new ZoneServer(ip, port, id);
                                mZoneServerList.Add(address, zone);
                            }
                            else
                                mZoneServerList[address].AddLoadedZone(id);
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }
            
        }

        public void LoadZoneEntranceList()
        {
            zoneEntranceList = new Dictionary<uint, ZoneEntrance>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    zoneId,
                                    spawnType,
                                    spawnX,
                                    spawnY,
                                    spawnZ,
                                    spawnRotation,
                                    privateAreaName
                                    FROM server_zones_spawnlocations";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);
                            string privArea = null;

                            if (!reader.IsDBNull(7))
                                privArea = reader.GetString(7);

                            ZoneEntrance entance = new ZoneEntrance(reader.GetUInt32(1), privArea, reader.GetByte(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6));
                            zoneEntranceList[id] = entance;
                            count++;
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            Program.Log.Info(String.Format("Loaded {0} zone spawn locations.", count));
        }
        
        public void ConnectToZoneServers()
        {
            Program.Log.Info("--------------------------");
            Program.Log.Info("Connecting to zone servers");
            Program.Log.Info("--------------------------");

            foreach (ZoneServer zs in mZoneServerList.Values)
            {
                zs.Connect();
            }
        }

        public ZoneServer GetZoneServer(uint zoneId)
        {
            foreach (ZoneServer zs in mZoneServerList.Values)
            {
                if (zs.ownedZoneIds.Contains(zoneId))
                    return zs;
            }

            return null;
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed.
        public void DoSeamlessZoneServerChange(Session session, uint destinationZoneId)
        {
            
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneServerChange(Session session, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneServerChange(session, ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneServerChange(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            ZoneServer zs = GetZoneServer(destinationZoneId);

            if (zs == null)
                return;

            session.currentZoneId = destinationZoneId;

            //Intrazone change, just update the id
            if (zs.Equals(session.routing1))            
                return;            

            if (zs.isConnected)
                session.routing1.SendSessionEnd(session, destinationZoneId, destinationPrivateArea, spawnType, spawnX, spawnY, spawnZ, spawnRotation);
            else if (zs.Connect())
                session.routing1.SendSessionEnd(session, destinationZoneId, destinationPrivateArea, spawnType, spawnX, spawnY, spawnZ, spawnRotation);
            else            
                session.routing1.SendPacket(ErrorPacket.BuildPacket(session, 1));            
        }

        //Login Zone In
        public void DoLogin(Session session)
        {
            SendMotD(session);

            //Send party, retainer, ls groups
            Party pt = mPartyManager.GetParty(session.sessionId);
             
            pt.SendGroupPackets(session);
            SendPartySync(pt);   

            mRetainerGroupManager.GetRetainerGroup(session.sessionId).SendGroupPackets(session);

            List<Linkshell> linkshells = mLinkshellManager.GetPlayerLinkshellMembership(session.sessionId);
            foreach (Linkshell ls in linkshells)
                ls.SendGroupPackets(session);

            //Reset to blank if in unknown state
            ulong activeGroupIndex = 0;
            if (!session.activeLinkshellName.Equals(""))
            {
                Linkshell activeLs = mLinkshellManager.GetLinkshell(session.activeLinkshellName);
                if (activeLs != null && activeLs.HasMember(session.sessionId))
                {
                    activeGroupIndex = activeLs.groupIndex;
                }
                else
                {
                    session.activeLinkshellName = "";
                    Database.SetActiveLS(session, "");
                }
            }
            SubPacket activeLsPacket = SetActiveLinkshellPacket.BuildPacket(session.sessionId, activeGroupIndex);
            session.clientConnection.QueuePacket(activeLsPacket);
        }

        private void SendMotD(Session session)
        {
            session.clientConnection.QueuePacket(SendMessagePacket.BuildPacket(session.sessionId, session.sessionId, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "-------- Login Message --------"));
            session.clientConnection.QueuePacket(SendMessagePacket.BuildPacket(session.sessionId, session.sessionId, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Welcome to {0}!", ConfigConstants.PREF_SERVERNAME)));
            session.clientConnection.QueuePacket(SendMessagePacket.BuildPacket(session.sessionId, session.sessionId, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Welcome to Eorzea!"));
            session.clientConnection.QueuePacket(SendMessagePacket.BuildPacket(session.sessionId, session.sessionId, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Here is a test Message of the Day from the World Server!"));
        }

        public void SendPartySync(Party party)
        {
            List<ZoneServer> alreadySent = new List<ZoneServer>();
            foreach (uint member in party.members)
            {
                Session session = Server.GetServer().GetSession(member);                
                if (session == null)
                    continue;

                if (alreadySent.Contains(session.routing1))
                    continue;

                alreadySent.Add(session.routing1);
                SubPacket syncPacket = PartySyncPacket.BuildPacket(session, party);
                session.routing1.SendPacket(syncPacket);
            }
        }

        public class ZoneEntrance
        {
            public uint zoneId;
            public string privateAreaName;
            public byte spawnType;
            public float spawnX;
            public float spawnY;
            public float spawnZ;
            public float spawnRotation;

            public ZoneEntrance(uint zoneId, string privateAreaName, byte spawnType, float x, float y, float z, float rot)
            {
                this.zoneId = zoneId;
                this.privateAreaName = privateAreaName;
                this.spawnType = spawnType;
                this.spawnX = x;
                this.spawnY = y;
                this.spawnZ  = z;
                this.spawnRotation = rot;
            }
        }

        public void SendGroupData(Session session, ulong groupId)
        {
            if (mCurrentWorldGroups.ContainsKey(groupId))
            {
                Group group = mCurrentWorldGroups[groupId];
                group.SendGroupPackets(session);
            }
        }

        public void SendGroupDataToAllMembers(ulong groupId)
        {
            if (mCurrentWorldGroups.ContainsKey(groupId))
            {
                Group group = mCurrentWorldGroups[groupId];
                foreach (GroupMember member in group.BuildMemberList(0))
                    group.SendGroupPackets(mServer.GetSession(member.actorId));
            }
        }

        public void ProcessPartyInvite(Session requestSession, uint invitee)
        {
            if (mServer.GetSession(invitee) == null)
            {
                requestSession.SendGameMessage(30544, 0x20);
            }
            else
            {
                Session inviteeSession = mServer.GetSession(invitee);
                Relation inviteRelation = mRelationGroupManager.CreatePartyRelationGroup(mPartyManager.GetParty(requestSession.sessionId).groupIndex, requestSession.sessionId, invitee);
                inviteRelation.SendGroupPacketsAll(requestSession.sessionId, invitee);          
                inviteeSession.SendGameMessage(30430, 0x20, (object)mServer.GetNameForId(requestSession.sessionId)); //X Invited you
                requestSession.SendGameMessage(30433, 0x20, (object)mServer.GetNameForId(inviteeSession.sessionId)); //You invite X
            }
        }

        public void ProcessPartyInviteResult(Session inviteeSession, uint resultCode)
        {
            Relation relation = mRelationGroupManager.GetPartyRelationGroup(inviteeSession.sessionId);
            Session inviterSession = mServer.GetSession(relation.GetHost());            

            //Accept
            if (resultCode == 1)
            {
                Party oldParty = mPartyManager.GetParty(inviteeSession.sessionId);
                if (oldParty.members.Count == 1)
                {
                    mPartyManager.DeleteParty(oldParty.groupIndex);
                    Party newParty = mPartyManager.GetParty(inviterSession.sessionId);
                    mPartyManager.AddToParty(newParty.groupIndex, inviteeSession.sessionId);
                    newParty.SendGroupPacketsAll(newParty.members);
                    SendPartySync(newParty);
                    newParty.OnPlayerJoin(inviteeSession);                    
                }
            }
            else //Refuse 
            {
                inviterSession.SendGameMessage(30573, 0x20, (object)mServer.GetNameForId(inviteeSession.sessionId)); //X rejects your invite
            }

            //Delete the relation
            mRelationGroupManager.DeleteRelationGroup(relation.groupIndex);
            relation.SendDeletePackets(inviterSession.sessionId, inviteeSession.sessionId);

        }

        public void ProcessLinkshellInvite(Session inviterSession, string lsName, uint invitee)
        {
            Session inviteeSession = mServer.GetSession(invitee);
            Linkshell ls = mLinkshellManager.GetLinkshell(lsName);

            //Something really fucked up
            if (mServer.GetSession(invitee) == null)
            {
                inviterSession.SendGameMessage(30544, 0x20);
            }
            //Check if they are already in your LS
            else if (ls.HasMember(invitee))
            {
                inviterSession.SendGameMessage(25282, 0x20, (object)inviteeSession.characterName, 1); //X already belongs to 
            }
            //Check if you can invite more members
            else if (ls.GetMemberCount() >= LinkshellManager.LS_MAX_MEMBERS)
            {
                inviterSession.SendGameMessage(25158, 0x20, (object)inviteeSession); //This linkshell cannot take on any more members.
            }
            //Check if they currently have an invite
            else if (mRelationGroupManager.GetLinkshellRelationGroup(invitee) != null)
            {
                inviterSession.SendGameMessage(25196, 0x20, (object)inviteeSession); //Unable to invite X another pending
            }
            else
            {
                Relation inviteRelation = mRelationGroupManager.CreateLinkshellRelationGroup(mLinkshellManager.GetLinkshell(lsName).groupIndex, inviterSession.sessionId, invitee);
                inviteRelation.SendGroupPacketsAll(inviterSession.sessionId, invitee);
                inviteeSession.SendGameMessage(25150, 0x20, (object)1, (object)lsName, (object)inviterSession); //X Offers you
                inviterSession.SendGameMessage(25151, 0x20, (object)1, null, (object)inviteeSession); //You offer X
            }
             
        }

        public void ProcessLinkshellInviteResult(Session inviteeSession, uint resultCode)
        {
            Relation relation = mRelationGroupManager.GetLinkshellRelationGroup(inviteeSession.sessionId);
            Session inviterSession = mServer.GetSession(relation.GetHost());

            Linkshell newLS = null;
            if (mCurrentWorldGroups.ContainsKey(relation.groupIndex))
                newLS = (Linkshell)mCurrentWorldGroups[relation.GetTopicGroupIndex()];
            else
            {
                //??? errored
            }

            if (newLS != null)
            {
                //Accept
                if (resultCode == 1)
                {
                    //Check if the invitee has room for more linkshells
                    if (mLinkshellManager.GetPlayerLinkshellMembership(inviteeSession.sessionId).Count >= LinkshellManager.LS_MAX_ALLOWED)
                    {
                        inviteeSession.SendGameMessage(25153, 0x20, (object)inviteeSession); //You are unable to join any more linkshells.
                    }
                    //Did someone invite in the meantime?
                    else if (newLS.GetMemberCount() >= LinkshellManager.LS_MAX_MEMBERS)
                    {
                        inviterSession.SendGameMessage(25158, 0x20, (object)inviteeSession); //This linkshell cannot take on any more members.
                    }
                    //All good, add new member
                    else
                    {
                        mLinkshellManager.AddMemberToLinkshell(inviteeSession.sessionId, newLS.name);
                        newLS.SendGroupPacketsAll(newLS.GetMemberIds());
                        newLS.OnPlayerJoin(inviteeSession);
                    }
                }
                else //Refuse 
                {
                    inviteeSession.SendGameMessage(25189, 0x20); //You decline the linkpearl offer.
                    inviterSession.SendGameMessage(25190, 0x20); //Your linkpearl offer is declined.
                }
            }           

            //Delete the relation
            //mRelationGroupManager.DeleteRelationGroup(relation.groupIndex);
            relation.SendDeletePackets(inviterSession.sessionId, inviteeSession.sessionId);
        }

        public void ProcessLinkshellInviteCancel(Session inviterSession)
        {
            Relation relation = mRelationGroupManager.GetLinkshellRelationGroup(inviterSession.sessionId);
            Session inviteeSession = mServer.GetSession(relation.GetOther());

            inviterSession.SendGameMessage(25191, 0x20); //You cancel the linkpearl offer.
            inviteeSession.SendGameMessage(25192, 0x20); //The linkpearl offer has been canceled.
            
            //Delete the relation
            mRelationGroupManager.DeleteRelationGroup(relation.groupIndex);
            relation.SendDeletePackets(inviterSession.sessionId, inviteeSession.sessionId);
        }

        public void ProcessLinkshellSetActive(Session requestSession, string lsName)
        {
            //Deactivate all
            if (lsName.Equals(""))
            {
                requestSession.SetActiveLS(lsName);
                SubPacket activeLsPacket = SetActiveLinkshellPacket.BuildPacket(requestSession.sessionId, 0);
                requestSession.clientConnection.QueuePacket(activeLsPacket);
                requestSession.SendGameMessage(25132, 0x20, (object)1);
            }
            else
            {
                Linkshell ls = mLinkshellManager.GetLinkshell(lsName);

                if (ls == null || !ls.HasMember(requestSession.sessionId))
                {
                    requestSession.SendGameMessage(25167, 0x20, (object)1, (object)lsName);
                }                
                else
                {
                    requestSession.SetActiveLS(lsName);
                    SubPacket activeLsPacket = SetActiveLinkshellPacket.BuildPacket(requestSession.sessionId, ls.groupIndex);
                    requestSession.clientConnection.QueuePacket(activeLsPacket);
                    requestSession.SendGameMessage(25131, 0x20, (object)1, (object)lsName);                    
                }
            }
        }

        public void IncrementGroupIndex()
        {
            mRunningGroupIndex++;
        }

        public ulong GetGroupIndex()
        {
            return mRunningGroupIndex | 0x8000000000000000;
        }

        public bool SendGroupInit(Session session, ulong groupId)
        {
            if (mCurrentWorldGroups.ContainsKey(groupId))
            {
                mCurrentWorldGroups[groupId].SendInitWorkValues(session);
                return true;
            }
            return false;
        }

        public PartyManager GetPartyManager()
        {
            return mPartyManager;
        }

        public RetainerGroupManager GetRetainerManager()
        {
            return mRetainerGroupManager;
        }

        public LinkshellManager GetLinkshellManager()
        {
            return mLinkshellManager;
        }

    }

}

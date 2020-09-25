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
using System.Net;
using System.Net.Sockets;

using Meteor.Common;
using Meteor.World.DataObjects;
using Meteor.World.DataObjects.Group;
using Meteor.World.Packets.WorldPackets.Receive;
using Meteor.World.Packets.WorldPackets.Receive.Group;
using Meteor.World.Packets.WorldPackets.Send;

namespace Meteor.World
{
    class Server
    {
        public const int FFXIV_MAP_PORT = 54992;
        public const int BUFFER_SIZE = 0xFFFF; //Max basepacket size is 0xFFFF
        public const int BACKLOG = 100;
        private static Server mSelf;

        //Connections
        private Socket mServerSocket;
        WorldManager mWorldManager;
        PacketProcessor mPacketProcessor;

        //Preloaded Maps
        private Dictionary<uint, string> mIdToNameMap = new Dictionary<uint, string>();

        //Session Management
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private Dictionary<uint, Session> mZoneSessionList = new Dictionary<uint, Session>();
        private Dictionary<uint, Session> mChatSessionList = new Dictionary<uint, Session>();        

        public Server()
        {
            mSelf = this;            
        }

        public static Server GetServer()
        {
            return mSelf;
        }

        public bool StartServer()
        {
            mPacketProcessor = new PacketProcessor(this);
            
            LoadCharaNames();

            mWorldManager = new WorldManager(this);
            mWorldManager.LoadZoneServerList();
            mWorldManager.LoadZoneEntranceList();
            mWorldManager.ConnectToZoneServers();

            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), int.Parse(ConfigConstants.OPTIONS_PORT));

            try
            {
                mServerSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not Create socket, check to make sure not duplicating port", e);
            }
            try
            {
                mServerSocket.Bind(serverEndPoint);
                mServerSocket.Listen(BACKLOG);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured while binding socket, check inner exception", e);
            }
            try
            {
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured starting listeners, check inner exception", e);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Program.Log.Info("World Server accepting connections @ {0}:{1}", (mServerSocket.LocalEndPoint as IPEndPoint).Address, (mServerSocket.LocalEndPoint as IPEndPoint).Port);
            Console.ForegroundColor = ConsoleColor.Gray;            

            return true;
        }

        public void AddSession(ClientConnection connection, Session.Channel type, uint id)
        {
            Session session = new Session(id, connection, type);
            
            switch (type)
            {
                case Session.Channel.ZONE:
                    //New character since world server loaded
                    if (!mIdToNameMap.ContainsKey(id))
                        AddNameToMap(id, session.characterName);
                    //TODO: this is technically wrong!!! Should kick out player and wait till auto-removed.
                    if (mZoneSessionList.ContainsKey(id))
                        mZoneSessionList.Remove(id);

                    mZoneSessionList.Add(id, session);
                    break;
                case Session.Channel.CHAT:
                    if (!mChatSessionList.ContainsKey(id))
                        mChatSessionList.Add(id, session);
                    break;
            }
        }

        public void RemoveSession(Session.Channel type, uint id)
        {
            switch (type)
            {
                case Session.Channel.ZONE:
                    if (mZoneSessionList.ContainsKey(id))
                    {
                        mZoneSessionList[id].clientConnection.Disconnect();
                        mConnectionList.Remove(mZoneSessionList[id].clientConnection);
                        mZoneSessionList.Remove(id);
                    }
                    break;
                case Session.Channel.CHAT:
                    if (mChatSessionList.ContainsKey(id))
                    {
                        mChatSessionList[id].clientConnection.Disconnect();
                        mConnectionList.Remove(mChatSessionList[id].clientConnection);
                        mChatSessionList.Remove(id);
                    }
                    break;
            }
        }

        public Session GetSession(uint targetSession, Session.Channel type = Session.Channel.ZONE)
        {
            switch (type)
            {
                case Session.Channel.ZONE:
                    if (mZoneSessionList.ContainsKey(targetSession))
                        return mZoneSessionList[targetSession];
                    break;
                case Session.Channel.CHAT:
                    if (mChatSessionList.ContainsKey(targetSession))
                        return mChatSessionList[targetSession];
                    break;
            }

            return null;
        }

        public Session GetSession(string targetSessionName)
        {
            lock (mZoneSessionList)
            {
                foreach (Session s in mZoneSessionList.Values)
                {
                    if (s.characterName != null && s.characterName.Equals(targetSessionName))
                        return s;
                }
            }

            return null;
        }

        public void OnReceiveSubPacketFromZone(ZoneServer zoneServer, SubPacket subpacket)
        {
            uint sessionId = subpacket.header.targetId;
            Session session = GetSession(sessionId);

            if (subpacket.gameMessage.opcode != 0x1 && subpacket.gameMessage.opcode != 0xca)
                subpacket.DebugPrintSubPacket();     

            if (subpacket.gameMessage.opcode >= 0x1000)
            {
                //subpacket.DebugPrintSubPacket();                

                switch (subpacket.gameMessage.opcode)
                {
                    //Session Begin Confirm
                    case 0x1000:
                        SessionBeginConfirmPacket beginConfirmPacket = new SessionBeginConfirmPacket(subpacket.data);

                        if (beginConfirmPacket.invalidPacket || beginConfirmPacket.errorCode == 0)
                            Program.Log.Error("Session {0} had a error beginning session.", beginConfirmPacket.sessionId);

                        break;
                    //Session End Confirm
                    case 0x1001:
                        SessionEndConfirmPacket endConfirmPacket = new SessionEndConfirmPacket(subpacket.data);

                        if (!endConfirmPacket.invalidPacket && endConfirmPacket.errorCode == 0)
                        {
                            //Check destination, if != 0, update route and start new session
                            if (endConfirmPacket.destinationZone != 0)
                            {
                                session.routing1 = Server.GetServer().GetWorldManager().GetZoneServer(endConfirmPacket.destinationZone);
                                session.routing1.SendSessionStart(session);
                            }
                            else
                            {
                                RemoveSession(Session.Channel.ZONE, endConfirmPacket.sessionId);
                                RemoveSession(Session.Channel.CHAT, endConfirmPacket.sessionId);
                            }
                        }
                        else
                            Program.Log.Error("Session {0} had an error ending session.", endConfirmPacket.sessionId);

                        break;
                    //Zone Change Request
                    case 0x1002:
                        WorldRequestZoneChangePacket zoneChangePacket = new WorldRequestZoneChangePacket(subpacket.data);

                        if (!zoneChangePacket.invalidPacket)
                        {
                            GetWorldManager().DoZoneServerChange(session, zoneChangePacket.destinationZoneId, "", zoneChangePacket.destinationSpawnType, zoneChangePacket.destinationX, zoneChangePacket.destinationY, zoneChangePacket.destinationZ, zoneChangePacket.destinationRot);
                        }

                        break;
                    //Change leader or kick
                    case 0x1020:
                        PartyModifyPacket partyModifyPacket = new PartyModifyPacket(subpacket.data);

                        Party pt = mWorldManager.GetPartyManager().GetParty(subpacket.header.targetId);

                        if (pt.GetMemberCount() <= 1)
                            return;

                        if (partyModifyPacket.command == PartyModifyPacket.MODIFY_LEADER)
                            pt.SetLeaderPlayerRequest(GetSession(subpacket.header.sourceId), partyModifyPacket.name);
                        else if (partyModifyPacket.command == PartyModifyPacket.MODIFY_KICKPLAYER)
                            pt.KickPlayerRequest(GetSession(subpacket.header.sourceId), partyModifyPacket.name); 
                        else if (partyModifyPacket.command == PartyModifyPacket.MODIFY_LEADER + 2)
                            pt.SetLeaderPlayerRequest(GetSession(subpacket.header.sourceId), partyModifyPacket.actorId);
                        else if (partyModifyPacket.command == PartyModifyPacket.MODIFY_KICKPLAYER + 2)
                            pt.KickPlayerRequest(GetSession(subpacket.header.sourceId), partyModifyPacket.actorId); 

                        break;
                    //Party Resign or Disband
                    case 0x1021:
                        PartyLeavePacket partyLeavePacket = new PartyLeavePacket(subpacket.data);
                        Party leavePt = mWorldManager.GetPartyManager().GetParty(subpacket.header.sourceId);

                        if (!partyLeavePacket.isDisband)
                            leavePt.LeavePlayerRequest(GetSession(subpacket.header.sourceId));
                        else
                            leavePt.DisbandPlayerRequest(GetSession(subpacket.header.sourceId));

                        break;
                    //Party Invite Request
                    case 0x1022:
                        PartyInvitePacket partyInvitePacket = new PartyInvitePacket(subpacket.data);
                        if (partyInvitePacket.command == 1)                        
                            mWorldManager.ProcessPartyInvite(GetSession(subpacket.header.sourceId), partyInvitePacket.actorId);                        
                        else if (partyInvitePacket.command == 0)                        
                        {
                            Session inviteeByNamesSession = GetSession(partyInvitePacket.name);
                            if (inviteeByNamesSession != null)
                                mWorldManager.ProcessPartyInvite(GetSession(subpacket.header.sourceId), inviteeByNamesSession.sessionId);
                            else
                            {
                                //Show not found msg
                            }
                        }
                        break;
                    //Group Invite Result
                    case 0x1023:
                        GroupInviteResultPacket groupInviteResultPacket = new GroupInviteResultPacket(subpacket.data);

                        switch (groupInviteResultPacket.groupType)
                        {
                            case 0x2711:
                                mWorldManager.ProcessPartyInviteResult(GetSession(subpacket.header.sourceId), groupInviteResultPacket.result);
                                break;
                            case 0x2712:
                                mWorldManager.ProcessLinkshellInviteResult(GetSession(subpacket.header.sourceId), groupInviteResultPacket.result);
                                break;
                        }
                        
                        break;
                    //Linkshell create request
                    case 0x1025:
                        CreateLinkshellPacket createLinkshellPacket = new CreateLinkshellPacket(subpacket.data);

                        Linkshell newLs = null;
                        int lsError = mWorldManager.GetLinkshellManager().CanCreateLinkshell(createLinkshellPacket.name);

                        if (lsError == 0)                       
                        {
                            newLs = mWorldManager.GetLinkshellManager().CreateLinkshell(createLinkshellPacket.name, createLinkshellPacket.crestid, createLinkshellPacket.master);
                    
                            if (newLs != null)
                                newLs.SendGroupPackets(session);
                            else
                                lsError = 3;
                        }

                        SubPacket resultPacket = LinkshellResultPacket.BuildPacket(session, lsError);
                        zoneServer.SendPacket(resultPacket);
                        break;
                    //Linkshell modify request
                    case 0x1026:
                        ModifyLinkshellPacket modifyLinkshellPacket = new ModifyLinkshellPacket(subpacket.data);
                        switch (modifyLinkshellPacket.argCode)
                        {
                            case 0:                                
                                break;
                            case 1:
                                mWorldManager.GetLinkshellManager().ChangeLinkshellCrest(modifyLinkshellPacket.currentName, modifyLinkshellPacket.crestid);
                                break;
                            case 2:
                                mWorldManager.GetLinkshellManager().ChangeLinkshellMaster(modifyLinkshellPacket.currentName, modifyLinkshellPacket.master);
                                break;
                        }                        
                        break;
                    //Linkshell delete request
                    case 0x1027:
                        DeleteLinkshellPacket deleteLinkshellPacket = new DeleteLinkshellPacket(subpacket.data);
                        mWorldManager.GetLinkshellManager().DeleteLinkshell(deleteLinkshellPacket.name);
                        break;
                    //Linkshell set active
                    case 0x1028:
                        LinkshellChangePacket linkshellChangePacket = new LinkshellChangePacket(subpacket.data);
                        mWorldManager.ProcessLinkshellSetActive(GetSession(subpacket.header.sourceId), linkshellChangePacket.lsName);
                        break;
                    //Linkshell invite member
                    case 0x1029:
                        LinkshellInvitePacket linkshellInvitePacket = new LinkshellInvitePacket(subpacket.data);
                        mWorldManager.ProcessLinkshellInvite(GetSession(subpacket.header.sourceId), linkshellInvitePacket.lsName, linkshellInvitePacket.actorId);
                        break;
                    //Linkshell cancel invite
                    case 0x1030:
                        LinkshellInviteCancelPacket linkshellInviteCancelPacket = new LinkshellInviteCancelPacket(subpacket.data);
                        mWorldManager.ProcessLinkshellInviteCancel(GetSession(subpacket.header.sourceId));
                        break;
                    //Linkshell resign/kicked
                    case 0x1031:
                        LinkshellLeavePacket linkshellLeavePacket = new LinkshellLeavePacket(subpacket.data);
                        Linkshell lsLeave = mWorldManager.GetLinkshellManager().GetLinkshell(linkshellLeavePacket.lsName);
                        if (linkshellLeavePacket.isKicked)
                            lsLeave.KickRequest(GetSession(subpacket.header.sourceId), linkshellLeavePacket.kickedName);
                        else
                            lsLeave.LeaveRequest(GetSession(subpacket.header.sourceId));
                        break;
                    //Linkshell rank change
                    case 0x1032:
                        LinkshellRankChangePacket linkshellRankChangePacket = new LinkshellRankChangePacket(subpacket.data);
                        Linkshell lsRankChange = mWorldManager.GetLinkshellManager().GetLinkshell(linkshellRankChangePacket.lsName);                        
                        lsRankChange.RankChangeRequest(GetSession(subpacket.header.sourceId), linkshellRankChangePacket.name, linkshellRankChangePacket.rank);                       
                        break;
                }
            }
            else if (mZoneSessionList.ContainsKey(sessionId))
            {
                ClientConnection conn = mZoneSessionList[sessionId].clientConnection;
                conn.QueuePacket(subpacket);
                conn.FlushQueuedSendPackets();
            }

        }

        public WorldManager GetWorldManager()
        {
            return mWorldManager;
        }

        #region Socket Handling
        private void AcceptCallback(IAsyncResult result)
        {
            ClientConnection conn = null;
            Socket socket = (System.Net.Sockets.Socket)result.AsyncState;

            try
            {
                conn = new ClientConnection();
                conn.socket = socket.EndAccept(result);
                conn.buffer = new byte[BUFFER_SIZE];

                lock (mConnectionList)
                {
                    mConnectionList.Add(conn);
                }

                Program.Log.Info("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port);
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                //Queue the accept of the next incomming connection
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (SocketException)
            {
                if (conn != null)
                {

                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (Exception)
            {
                if (conn != null)
                {
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
        }

        /// <summary>
        /// Receive Callback. Reads in incoming data, converting them to base packets. Base packets are sent to be parsed. If not enough data at the end to build a basepacket, move to the beginning and prepend.
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;

            //Check if disconnected
            if ((conn.socket.Poll(1, SelectMode.SelectRead) && conn.socket.Available == 0))
            {
                lock (mConnectionList)
                {
                    mConnectionList.Remove(conn);
                }

                return;
            }

            try
            {
                int bytesRead = conn.socket.EndReceive(result);

                bytesRead += conn.lastPartialSize;

                if (bytesRead >= 0)
                {
                    int offset = 0;

                    //Build packets until can no longer or out of data
                    while (true)
                    {
                        BasePacket basePacket = BasePacket.CreatePacket(ref offset, conn.buffer, bytesRead);

                        //If can't build packet, break, else process another
                        if (basePacket == null)
                            break;
                        else
                        {
                            mPacketProcessor.ProcessPacket(conn, basePacket);
                        }

                    }

                    //Not all bytes consumed, transfer leftover to beginning
                    if (offset < bytesRead)
                        Array.Copy(conn.buffer, offset, conn.buffer, 0, bytesRead - offset);

                    conn.lastPartialSize = bytesRead - offset;

                    //Build any queued subpackets into basepackets and send
                    conn.FlushQueuedSendPackets();

                    if (offset < bytesRead)
                        //Need offset since not all bytes consumed
                        conn.socket.BeginReceive(conn.buffer, bytesRead - offset, conn.buffer.Length - (bytesRead - offset), SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                    else
                        //All bytes consumed, full buffer available
                        conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                }
                else
                {

                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
            }
            catch (SocketException)
            {
                if (conn.socket != null)
                {

                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
            }
        }

        #endregion        

        public void LoadCharaNames()
        {
            Database.GetAllCharaNames(mIdToNameMap);
        }

        public void AddNameToMap(uint charaId, string name)
        {
            mIdToNameMap.Add(charaId, name);
        }
        
        public string GetNameForId(uint charaId)
        {
            if (mIdToNameMap.ContainsKey(charaId))           
                return mIdToNameMap[charaId];
            return null;  
        }        


    }
}

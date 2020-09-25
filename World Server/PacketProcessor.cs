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

using System.Collections.Generic;

using Meteor.Common;
using Meteor.World.DataObjects;
using Meteor.World.DataObjects.Group;
using Meteor.World.Packets.Receive;
using Meteor.World.Packets.Receive.Subpackets;
using Meteor.World.Packets.Send;
using Meteor.World.Packets.Send.Login;
using Meteor.World.Packets.Send.Subpackets;
using Meteor.World.Packets.WorldPackets.Receive;

namespace Meteor.World
{
    class PacketProcessor
    {
        /*
        Session Creation:

            Get 0x1 from server
            Send 0x7
            Send 0x2

        Zone Change:

            Send 0x7
            Get 0x8 - Wait??
            Send 0x2
        */


        Server mServer;

        public PacketProcessor(Server server)
        {           
            mServer = server;
        }     

        public void ProcessPacket(ClientConnection client, BasePacket packet)
        {                      
            if (packet.header.isCompressed == 0x01)                       
                BasePacket.DecompressPacket(ref packet);
            
            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                //Initial Connect Packet, Create session
                if (subpacket.header.type == 0x01)
                {                    
                    HelloPacket hello = new HelloPacket(packet.data);

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                    {
                        mServer.AddSession(client, Session.Channel.ZONE, hello.sessionId);
                        Session session = mServer.GetSession(hello.sessionId);
                        session.routing1 = mServer.GetWorldManager().GetZoneServer(session.currentZoneId);
                        session.routing1.SendSessionStart(session, true);       
                    }
                    else if (packet.header.connectionType == BasePacket.TYPE_CHAT)
                        mServer.AddSession(client, Session.Channel.CHAT, hello.sessionId);

                    client.QueuePacket(_0x7Packet.BuildPacket(0x0E016EE5));
                    client.QueuePacket(_0x2Packet.BuildPacket(hello.sessionId));
                }
                //Ping from World Server
                else if (subpacket.header.type == 0x07)
                {
                    SubPacket init = _0x8PingPacket.BuildPacket(client.owner.sessionId);
                    client.QueuePacket(BasePacket.CreatePacket(init, true, false));
                }
                //Zoning Related
                else if (subpacket.header.type == 0x08)
                {
                    //Response, client's current [actorID][time]
                    //BasePacket init = Login0x7ResponsePacket.BuildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC(), 0x07);
                    //client.QueuePacket(init);
                    packet.DebugPrintPacket();
                }
                //Game Message
                else if (subpacket.header.type == 0x03)
                {                
                    //Send to the correct zone server
                    uint targetSession = subpacket.header.targetId;

                    InterceptProcess(mServer.GetSession(targetSession), subpacket);

                    if (mServer.GetSession(targetSession).routing1 != null)
                        mServer.GetSession(targetSession).routing1.SendPacket(subpacket);

                    if (mServer.GetSession(targetSession).routing2 != null)
                        mServer.GetSession(targetSession).routing2.SendPacket(subpacket);
                }
                //World Server Type
                else if (subpacket.header.type >= 0x1000)
                {
                    uint targetSession = subpacket.header.targetId;
                    Session session = mServer.GetSession(targetSession);

                    switch (subpacket.header.type)
                    {
                        //Session Begin Confirm
                        case 0x1000:
                            SessionBeginConfirmPacket beginConfirmPacket = new SessionBeginConfirmPacket(packet.data);

                            if (beginConfirmPacket.invalidPacket || beginConfirmPacket.errorCode == 0)                            
                                Program.Log.Error("Session {0} had a error beginning session.", beginConfirmPacket.sessionId);                            

                            break;
                        //Session End Confirm
                        case 0x1001:
                            SessionEndConfirmPacket endConfirmPacket = new SessionEndConfirmPacket(packet.data);
                            
                            if (!endConfirmPacket.invalidPacket && endConfirmPacket.errorCode != 0)
                            {
                                //Check destination, if != 0, update route and start new session
                                if (endConfirmPacket.destinationZone != 0)
                                {
                                    session.currentZoneId = endConfirmPacket.destinationZone;
                                    session.routing1 = Server.GetServer().GetWorldManager().GetZoneServer(endConfirmPacket.destinationZone);
                                    session.routing1.SendSessionStart(session);
                                }
                                else
                                {                                    
                                    mServer.RemoveSession(Session.Channel.ZONE, endConfirmPacket.sessionId);
                                    mServer.RemoveSession(Session.Channel.CHAT, endConfirmPacket.sessionId);
                                }
                            }
                            else
                                Program.Log.Error("Session {0} had an error ending session.", endConfirmPacket.sessionId);

                            break;                        
                        //Zone Change Request
                        case 0x1002:
                            WorldRequestZoneChangePacket zoneChangePacket = new WorldRequestZoneChangePacket(packet.data);

                            if (!zoneChangePacket.invalidPacket)
                            {
                                mServer.GetWorldManager().DoZoneServerChange(session, zoneChangePacket.destinationZoneId, "", zoneChangePacket.destinationSpawnType, zoneChangePacket.destinationX, zoneChangePacket.destinationY, zoneChangePacket.destinationZ, zoneChangePacket.destinationRot);
                            }
                           
                            break;
                    }

                }
                else
                    packet.DebugPrintPacket();
            }
        }    

        public void InterceptProcess(Session session, SubPacket subpacket)
        {
            switch (subpacket.gameMessage.opcode)
            {
                case 0x00C9:
                    subpacket.DebugPrintSubPacket();
                    PartyChatMessagePacket partyChatMessagePacket = new PartyChatMessagePacket(subpacket.data);                 
                    Party playerParty = mServer.GetWorldManager().GetPartyManager().GetParty(session.sessionId);
                    for (int i = 0; i < playerParty.members.Count; i++)
                    {
                        Session thatSession = mServer.GetSession(playerParty.members[i]);
                        if (thatSession != null && !session.Equals(thatSession))
                        {
                            thatSession.clientConnection.QueuePacket(SendMessagePacket.BuildPacket(session.sessionId, thatSession.sessionId, SendMessagePacket.MESSAGE_TYPE_PARTY, mServer.GetNameForId(session.sessionId), partyChatMessagePacket.message));
                        }
                    }                    
                    break;
                case 0x6:
                    mServer.GetWorldManager().DoLogin(session);
                    break;
                    //Special case for groups. If it's a world group, send values, else send to zone server
                case 0x133:            
                    GroupCreatedPacket groupCreatedPacket = new GroupCreatedPacket(subpacket.data);
                    if (!mServer.GetWorldManager().SendGroupInit(session, groupCreatedPacket.groupId))                                    
                        session.clientConnection.QueuePacket(subpacket);
                    break;
            }
        }
    }
}

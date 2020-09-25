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

using Meteor.Common;

using System;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.receive;
using Meteor.Map.packets.send;
using Meteor.Map.packets.send.login;
using Meteor.Map.packets.send.actor;
using Meteor.Map.packets.send.supportdesk;
using Meteor.Map.packets.receive.social;
using Meteor.Map.packets.send.social;
using Meteor.Map.packets.receive.supportdesk;
using Meteor.Map.packets.receive.recruitment;
using Meteor.Map.packets.send.recruitment;
using Meteor.Map.packets.receive.events;
using Meteor.Map.lua;
using Meteor.Map.Actors;
using Meteor.Map.packets.WorldPackets.Send;
using Meteor.Map.packets.WorldPackets.Receive;
using Meteor.Map.actors.director;

namespace Meteor.Map
{
    class PacketProcessor
    {
        Server mServer;

        public PacketProcessor(Server server)
        {
            mServer = server;
        }     

        public void ProcessPacket(ZoneConnection client, SubPacket subpacket)
        {                          
                Session session = mServer.GetSession(subpacket.header.sourceId);

                if (session == null && subpacket.gameMessage.opcode != 0x1000)
                    return;

                //Normal Game Opcode
                switch (subpacket.gameMessage.opcode)
                {
                    //World Server - Error
                    case 0x100A:
                        ErrorPacket worldError = new ErrorPacket(subpacket.data);
                        switch (worldError.errorCode)
                        {
                            case 0x01:
                                session.GetActor().SendGameMessage(Server.GetWorldManager().GetActor(), 60005, 0x20);
                                break;
                        }
                        break;
                    //World Server - Session Begin
                    case 0x1000:
                        subpacket.DebugPrintSubPacket();

                        SessionBeginPacket beginSessionPacket = new SessionBeginPacket(subpacket.data);

                        session = mServer.AddSession(subpacket.header.sourceId);

                        if (!beginSessionPacket.isLogin)
                            Server.GetWorldManager().DoZoneIn(session.GetActor(), false, session.GetActor().destinationSpawnType);

                        Program.Log.Info("{0} has been added to the session list.", session.GetActor().customDisplayName);

                        client.FlushQueuedSendPackets();
                        break;
                    //World Server - Session End
                    case 0x1001:
                        SessionEndPacket endSessionPacket = new SessionEndPacket(subpacket.data);

                        if (endSessionPacket.destinationZoneId == 0)
                            session.GetActor().CleanupAndSave();
                        else                        
                            session.GetActor().CleanupAndSave(endSessionPacket.destinationZoneId, endSessionPacket.destinationSpawnType, endSessionPacket.destinationX, endSessionPacket.destinationY, endSessionPacket.destinationZ, endSessionPacket.destinationRot);

                        Server.GetServer().RemoveSession(session.id);
                        Program.Log.Info("{0} has been removed from the session list.", session.GetActor().customDisplayName);

                        session.QueuePacket(SessionEndConfirmPacket.BuildPacket(session, endSessionPacket.destinationZoneId));
                        client.FlushQueuedSendPackets();
                        break;
                    //World Server - Party Synch
                    case 0x1020:
                        PartySyncPacket partySyncPacket = new PartySyncPacket(subpacket.data);
                        Server.GetWorldManager().PartyMemberListRecieved(partySyncPacket);
                        break;
                    //World Server - Linkshell Creation Result
                    case 0x1025:
                        LinkshellResultPacket lsResult = new LinkshellResultPacket(subpacket.data);
                        LuaEngine.GetInstance().OnSignal("ls_result", lsResult.resultCode);
                        break;
                    //Ping
                    case 0x0001:
                        //subpacket.DebugPrintSubPacket();
                        PingPacket pingPacket = new PingPacket(subpacket.data);
                        session.QueuePacket(PongPacket.BuildPacket(session.id, pingPacket.time));
                        session.Ping();
                        break;
                    //Unknown
                    case 0x0002:

                        subpacket.DebugPrintSubPacket();
                        session.QueuePacket(_0x2Packet.BuildPacket(session.id));
                        client.FlushQueuedSendPackets();

                        break;
                    //Chat Received
                    case 0x0003:
                        ChatMessagePacket chatMessage = new ChatMessagePacket(subpacket.data);
                        //Program.Log.Info("Got type-{5} message: {0} @ {1}, {2}, {3}, Rot: {4}", chatMessage.message, chatMessage.posX, chatMessage.posY, chatMessage.posZ, chatMessage.posRot, chatMessage.logType);
                   
                        if (chatMessage.message.StartsWith("!"))
                        {
                            if (Server.GetCommandProcessor().DoCommand(chatMessage.message, session))
                                return; ;
                        }

                        if (chatMessage.logType == SendMessagePacket.MESSAGE_TYPE_SAY || chatMessage.logType == SendMessagePacket.MESSAGE_TYPE_SHOUT)
                            session.GetActor().BroadcastPacket(SendMessagePacket.BuildPacket(session.id, chatMessage.logType, session.GetActor().customDisplayName, chatMessage.message), false);

                        break;
                    //Langauge Code (Client safe to send packets to now)
                    case 0x0006:
                        LangaugeCodePacket langCode = new LangaugeCodePacket(subpacket.data);
                        LuaEngine.GetInstance().CallLuaFunction(session.GetActor(), session.GetActor(), "onBeginLogin", true);                    
                        Server.GetWorldManager().DoZoneIn(session.GetActor(), true, 0x1);
                        LuaEngine.GetInstance().CallLuaFunction(session.GetActor(), session.GetActor(), "onLogin", true);
                        session.languageCode = langCode.languageCode;
                        break;
                    //Unknown - Happens a lot at login, then once every time player zones
                    case 0x0007:
                        //subpacket.DebugPrintSubPacket();
                        ZoneInCompletePacket zoneInCompletePacket = new ZoneInCompletePacket(subpacket.data);                        
                        break;
                    //Update Position
                    case 0x00CA:
                        //Update Position
                        UpdatePlayerPositionPacket posUpdate = new UpdatePlayerPositionPacket(subpacket.data);
                        session.UpdatePlayerActorPosition(posUpdate.x, posUpdate.y, posUpdate.z, posUpdate.rot, posUpdate.moveState);
                        session.GetActor().SendInstanceUpdate();

                        if (session.GetActor().IsInZoneChange())
                            session.GetActor().SetZoneChanging(false);

                        break;
                    //Set Target 
                    case 0x00CD:
                        //subpacket.DebugPrintSubPacket();

                        SetTargetPacket setTarget = new SetTargetPacket(subpacket.data);
                        session.GetActor().currentTarget = setTarget.actorID;
                        session.GetActor().isAutoAttackEnabled = setTarget.attackTarget != 0xE0000000;
                        session.GetActor().BroadcastPacket(SetActorTargetAnimatedPacket.BuildPacket(session.id, setTarget.actorID), true);
                        break;
                    //Lock Target
                    case 0x00CC:
                        LockTargetPacket lockTarget = new LockTargetPacket(subpacket.data);
                        session.GetActor().currentLockedTarget = lockTarget.actorID;
                        break;
                    //Start Event
                    case 0x012D:
                        subpacket.DebugPrintSubPacket();
                        EventStartPacket eventStart = new EventStartPacket(subpacket.data);

                        /*
                        if (eventStart.error != null)
                        {
                            player.errorMessage += eventStart.error;

                            if (eventStart.errorIndex == eventStart.errorNum - 1)
                                Program.Log.Error("\n"+player.errorMessage);


                            break;
                        }
                        */

                        Actor ownerActor = Server.GetStaticActors(eventStart.ownerActorID);
                            
                        if (ownerActor == null)
                        {
                            //Is it your retainer?
                            if (session.GetActor().currentSpawnedRetainer != null && session.GetActor().currentSpawnedRetainer.actorId == eventStart.ownerActorID)
                                ownerActor = session.GetActor().currentSpawnedRetainer;
                            //Is it a instance actor?
                            if (ownerActor == null)
                                ownerActor = session.GetActor().zone.FindActorInArea(eventStart.ownerActorID);
                            if (ownerActor == null)
                            {
                                //Is it a Director?
                                Director director = session.GetActor().GetDirector(eventStart.ownerActorID);
                                if (director != null)
                                    ownerActor = director;
                                else
                                {
                                    Program.Log.Debug("\n===Event START===\nCould not find actor 0x{0:X} for event started by caller: 0x{1:X}\nEvent Starter: {2}\nParams: {3}", eventStart.triggerActorID, eventStart.ownerActorID, eventStart.eventName, LuaUtils.DumpParams(eventStart.luaParams));
                                    break;
                                }
                            }                                    
                        }

                        session.GetActor().StartEvent(ownerActor, eventStart);

                        Program.Log.Debug("\n===Event START===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nEvent Starter: {4}\nParams: {5}", eventStart.triggerActorID, eventStart.ownerActorID, eventStart.serverCodes, eventStart.unknown, eventStart.eventName, LuaUtils.DumpParams(eventStart.luaParams));
                        break;
                    //Unknown, happens at npc spawn and cutscene play????
                    case 0x00CE:
                        subpacket.DebugPrintSubPacket();
                        break;
                    //Countdown requested
                    case 0x00CF:
                        CountdownRequestPacket countdownPacket = new CountdownRequestPacket(subpacket.data);
                        session.GetActor().BroadcastCountdown(countdownPacket.countdownLength, countdownPacket.syncTime);
                        break;
                    //Event Result
                    case 0x012E:
                        subpacket.DebugPrintSubPacket();
                        EventUpdatePacket eventUpdate = new EventUpdatePacket(subpacket.data);
                        Program.Log.Debug("\n===Event UPDATE===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nStep: 0x{4:X}\nParams: {5}", eventUpdate.triggerActorID, eventUpdate.serverCodes, eventUpdate.unknown1, eventUpdate.unknown2, eventUpdate.eventType, LuaUtils.DumpParams(eventUpdate.luaParams));
                        /*
                        //Is it a static actor? If not look in the player's instance
                        Actor updateOwnerActor = Server.GetStaticActors(session.GetActor().currentEventOwner);
                        if (updateOwnerActor == null)
                        {
                            updateOwnerActor = Server.GetWorldManager().GetActorInWorld(session.GetActor().currentEventOwner);

                            if (session.GetActor().currentDirector != null && session.GetActor().currentEventOwner == session.GetActor().currentDirector.actorId)
                                updateOwnerActor = session.GetActor().currentDirector;

                            if (updateOwnerActor == null)
                                break;
                        }
                        */
                        session.GetActor().UpdateEvent(eventUpdate);

                        //LuaEngine.DoActorOnEventUpdated(session.GetActor(), updateOwnerActor, eventUpdate);
                            
                        break;
                    case 0x012F:
                        subpacket.DebugPrintSubPacket();
                        ParameterDataRequestPacket paramRequest = new ParameterDataRequestPacket(subpacket.data);
                        if (paramRequest.paramName.Equals("charaWork/exp"))
                            session.GetActor().SendCharaExpInfo();
                        break;
                    //Item Package Request
                    case 0x0131:
                        UpdateItemPackagePacket packageRequest = new UpdateItemPackagePacket(subpacket.data);
                        if (Server.GetWorldManager().GetActorInWorld(packageRequest.actorID) != null)
                        {
                            ((Character)Server.GetWorldManager().GetActorInWorld(packageRequest.actorID)).SendItemPackage(session.GetActor(), packageRequest.packageId);
                            break;
                        }
                        if (session.GetActor().GetSpawnedRetainer() != null && session.GetActor().GetSpawnedRetainer().actorId == packageRequest.actorID)
                            session.GetActor().GetSpawnedRetainer().SendItemPackage(session.GetActor(), packageRequest.packageId);
                        break;
                    //Group Created Confirm
                    case 0x0133:
                        GroupCreatedPacket groupCreated = new GroupCreatedPacket(subpacket.data);
                        Server.GetWorldManager().SendGroupInit(session, groupCreated.groupId);
                        break;
                    //Achievement Progress Request
                    case 0x0135:
                        AchievementProgressRequestPacket progressRequest = new AchievementProgressRequestPacket(subpacket.data);
                        session.QueuePacket(Database.GetAchievementProgress(session.GetActor(), progressRequest.achievementId));
                        break;
                    /* RECRUITMENT */
                    //Start Recruiting
                    case 0x01C3:
                        StartRecruitingRequestPacket recruitRequestPacket = new StartRecruitingRequestPacket(subpacket.data);
                        session.QueuePacket(StartRecruitingResponse.BuildPacket(session.id, true));
                        break;
                    //End Recruiting
                    case 0x01C4:
                        session.QueuePacket(EndRecruitmentPacket.BuildPacket(session.id));
                        break;
                    //Party Window Opened, Request State
                    case 0x01C5:
                        session.QueuePacket(RecruiterStatePacket.BuildPacket(session.id, false, false, 0));
                        break;
                    //Search Recruiting
                    case 0x01C7:
                        RecruitmentSearchRequestPacket recruitSearchPacket = new RecruitmentSearchRequestPacket(subpacket.data);
                        break;
                    //Get Recruitment Details
                    case 0x01C8:
                        RecruitmentDetailsRequestPacket currentRecruitDetailsPacket = new RecruitmentDetailsRequestPacket(subpacket.data);
                        RecruitmentDetails details = new RecruitmentDetails();
                        details.recruiterName = "Localhost Character";
                        details.purposeId = 2;
                        details.locationId = 1;
                        details.subTaskId = 1;
                        details.comment = "This is a test details packet sent by the server. No implementation has been Created yet...";
                        details.num[0] = 1;
                        session.QueuePacket(CurrentRecruitmentDetailsPacket.BuildPacket(session.id, details));
                        break;
                    //Accepted Recruiting
                    case 0x01C6:
                        subpacket.DebugPrintSubPacket();
                        break;
                    /* SOCIAL STUFF */
                    case 0x01C9:
                        AddRemoveSocialPacket addBlackList = new AddRemoveSocialPacket(subpacket.data);
                        session.QueuePacket(BlacklistAddedPacket.BuildPacket(session.id, true, addBlackList.name));
                        break;
                    case 0x01CA:
                        AddRemoveSocialPacket RemoveBlackList = new AddRemoveSocialPacket(subpacket.data);
                        session.QueuePacket(BlacklistRemovedPacket.BuildPacket(session.id, true, RemoveBlackList.name));
                        break;
                    case 0x01CB:
                        int offset1 = 0;
                        session.QueuePacket(SendBlacklistPacket.BuildPacket(session.id, new String[] { "Test" }, ref offset1));
                        break;
                    case 0x01CC:
                        AddRemoveSocialPacket addFriendList = new AddRemoveSocialPacket(subpacket.data);
                        session.QueuePacket(FriendlistAddedPacket.BuildPacket(session.id, true, (uint)addFriendList.name.GetHashCode(), true, addFriendList.name));
                        break;
                    case 0x01CD:
                        AddRemoveSocialPacket RemoveFriendList = new AddRemoveSocialPacket(subpacket.data);
                        session.QueuePacket(FriendlistRemovedPacket.BuildPacket(session.id, true, RemoveFriendList.name));
                        break;
                    case 0x01CE:
                        int offset2 = 0;
                        session.QueuePacket(SendFriendlistPacket.BuildPacket(session.id, new Tuple<long, string>[] { new Tuple<long, string>(01, "Test2") }, ref offset2));
                        break;
                    case 0x01CF:
                        session.QueuePacket(FriendStatusPacket.BuildPacket(session.id, null));
                        break;
                    /* SUPPORT DESK STUFF */
                    //Request for FAQ/Info List
                    case 0x01D0:
                        FaqListRequestPacket faqRequest = new FaqListRequestPacket(subpacket.data);
                        session.QueuePacket(FaqListResponsePacket.BuildPacket(session.id, new string[] { "Testing FAQ1", "Coded style!" }));
                        break;
                    //Request for body of a faq/info selection
                    case 0x01D1:
                        FaqBodyRequestPacket faqBodyRequest = new FaqBodyRequestPacket(subpacket.data);
                        session.QueuePacket(FaqBodyResponsePacket.BuildPacket(session.id, "HERE IS A GIANT BODY. Nothing else to say!"));
                        break;
                    //Request issue list
                    case 0x01D2:
                        GMTicketIssuesRequestPacket issuesRequest = new GMTicketIssuesRequestPacket(subpacket.data);
                        session.QueuePacket(IssueListResponsePacket.BuildPacket(session.id, new string[] { "Test1", "Test2", "Test3", "Test4", "Test5" }));
                        break;
                    //Request if GM ticket exists
                    case 0x01D3:
                        session.QueuePacket(StartGMTicketPacket.BuildPacket(session.id, false));
                        break;
                    //Request for GM response message
                    case 0x01D4:
                        session.QueuePacket(GMTicketPacket.BuildPacket(session.id, "This is a GM Ticket Title", "This is a GM Ticket Body."));
                        break;
                    //GM Ticket Sent
                    case 0x01D5:
                        GMSupportTicketPacket gmTicket = new GMSupportTicketPacket(subpacket.data);
                        Program.Log.Info("Got GM Ticket: \n" + gmTicket.ticketTitle + "\n" + gmTicket.ticketBody);
                        session.QueuePacket(GMTicketSentResponsePacket.BuildPacket(session.id, true));
                        break;
                    //Request to end ticket
                    case 0x01D6:
                        session.QueuePacket(EndGMTicketPacket.BuildPacket(session.id));
                        break;
                    default:
                        Program.Log.Debug("Unknown command 0x{0:X} received.", subpacket.gameMessage.opcode);
                        subpacket.DebugPrintSubPacket();
                        break;
                }
            
        }        

    }
}


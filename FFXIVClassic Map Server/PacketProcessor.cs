using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.receive;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.login;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.social;
using FFXIVClassic_Map_Server.packets.send.social;
using FFXIVClassic_Map_Server.packets.receive.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.recruitment;
using FFXIVClassic_Map_Server.packets.send.recruitment;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server
{
    class PacketProcessor
    {
        Server mServer;
        CommandProcessor cp;
        Dictionary<uint, ConnectedPlayer> mPlayers;
        List<ClientConnection> mConnections;

        public PacketProcessor(Server server, Dictionary<uint, ConnectedPlayer> playerList, List<ClientConnection> connectionList)
        {
            mPlayers = playerList;
            mConnections = connectionList;
            mServer = server;
            cp = new CommandProcessor(playerList);
        }     

        public void ProcessPacket(ClientConnection client, BasePacket packet)
        {                      
            if (packet.header.isCompressed == 0x01)                       
                BasePacket.DecryptPacket(client.blowfish, ref packet);

            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                if (subpacket.header.type == 0x01)
                {                 
                    packet.DebugPrintPacket();
                    byte[] reply1Data = {
                                            0x01, 0x00, 0x00, 0x00, 0x28, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                            0x18, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xFD, 0xFF, 0xFF,
                                            0xE5, 0x6E, 0x01, 0xE0, 0x00, 0x00, 0x00, 0x0
                                        };

                    byte[] reply2Data = {
                                            0x01, 0x00, 0x00, 0x00, 0x28, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                            0x38, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0x2B, 0x5F, 0x26,
                                            0x66, 0x00, 0x00, 0x00, 0xC8, 0xD6, 0xAF, 0x2B, 0x38, 0x2B, 0x5F, 0x26, 0xB8, 0x8D, 0xF0, 0x2B,
                                            0xC8, 0xFD, 0x85, 0xFE, 0xA8, 0x7C, 0x5B, 0x09, 0x38, 0x2B, 0x5F, 0x26, 0xC8, 0xD6, 0xAF, 0x2B,
                                            0xB8, 0x8D, 0xF0, 0x2B, 0x88, 0xAF, 0x5E, 0x26
                                        };

                    BasePacket reply1 = new BasePacket(reply1Data);
                    BasePacket reply2 = new BasePacket(reply2Data);

                    //Write Timestamp into Reply1
                    using (MemoryStream mem = new MemoryStream(reply1.data))
                    {
                        using (BinaryWriter binReader = new BinaryWriter(mem))
                        {
                            binReader.BaseStream.Seek(0x14, SeekOrigin.Begin);
                            binReader.Write((UInt32)Utils.UnixTimeStampUTC());
                        }
                    }                   

                    //Read in Actor Id that owns this connection
                    uint actorID = 0;
                    using (MemoryStream mem = new MemoryStream(packet.data))
                    {
                        using (BinaryReader binReader = new BinaryReader(mem))
                        {
                            try
                            {
                                byte[] readIn = new byte[12];
                                binReader.BaseStream.Seek(0x14, SeekOrigin.Begin);
                                binReader.Read(readIn, 0, 12);
                                actorID = UInt32.Parse(Encoding.ASCII.GetString(readIn));
                            }
                            catch (Exception)
                            { }
                        }
                    }

                    //Should never happen.... unless actor id IS 0!
                    if (actorID == 0)
                        break;

                    client.owner = actorID;
                  
                    //Write Actor ID into reply2
                    using (MemoryStream mem = new MemoryStream(reply2.data))
                    {
                        using (BinaryWriter binReader = new BinaryWriter(mem))
                        {
                            binReader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                            binReader.Write(actorID);
                        }
                    }

                    ConnectedPlayer player = null;

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                    {
                        while (mPlayers != null && !mPlayers.ContainsKey(client.owner))
                        { }
                        player = mPlayers[client.owner];
                    }

                    //Create connected player if not Created
                    if (player == null)
                    { 
                        player = new ConnectedPlayer(actorID);
                        mPlayers[actorID] = player;
                    }
                    
                    player.SetConnection(packet.header.connectionType, client);

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                        Program.Log.Info("Got {0} connection for ActorID {1} @ {2}.", "zone", actorID, client.GetAddress());
                    else if (packet.header.connectionType == BasePacket.TYPE_CHAT)
                        Program.Log.Info("Got {0} connection for ActorID {1} @ {2}.", "chat", actorID, client.GetAddress());

                    //Create player actor
                    reply1.DebugPrintPacket();
                    client.QueuePacket(reply1);
                    client.QueuePacket(reply2);
                    break;
                }
                else if (subpacket.header.type == 0x07)
                {
                    BasePacket init = Login0x7ResponsePacket.BuildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC(), 0x08);
                    //client.QueuePacket(init);
                }
                else if (subpacket.header.type == 0x08)
                {
                    //Response, client's current [actorID][time]
                    //BasePacket init = Login0x7ResponsePacket.BuildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC(), 0x07);
                    //client.QueuePacket(init);
                    packet.DebugPrintPacket();
                }
                else if (subpacket.header.type == 0x03)
                {
                    ConnectedPlayer player = null;

                    if(mPlayers.ContainsKey(client.owner))
                        player = mPlayers[client.owner];

                    if (player == null || !player.IsClientConnectionsReady())
                        return;

                    //Normal Game Opcode
                    switch (subpacket.gameMessage.opcode)
                    {
                        //Ping
                        case 0x0001:
                            //subpacket.DebugPrintSubPacket();
                            PingPacket pingPacket = new PingPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(PongPacket.BuildPacket(player.actorID, pingPacket.time), true, false));
                            player.Ping();
                            break;
                        //Unknown
                        case 0x0002:

                            subpacket.DebugPrintSubPacket();
                            client.QueuePacket(_0x2Packet.BuildPacket(player.actorID), true, false);

                            Server.GetWorldManager().DoLogin(player.GetActor());


                            break;
                        //Chat Received
                        case 0x0003:
                            ChatMessagePacket chatMessage = new ChatMessagePacket(subpacket.data);
                            Program.Log.Info("Got type-{5} message: {0} @ {1}, {2}, {3}, Rot: {4}", chatMessage.message, chatMessage.posX, chatMessage.posY, chatMessage.posZ, chatMessage.posRot, chatMessage.logType);
                            subpacket.DebugPrintSubPacket();

                            if (chatMessage.message.StartsWith("!"))
                            {
                                if (cp.DoCommand(chatMessage.message, player))
                                    continue;
                            }                           

                            player.GetActor().BroadcastPacket(SendMessagePacket.BuildPacket(player.actorID, player.actorID, chatMessage.logType, player.GetActor().customDisplayName, chatMessage.message), false);

                            break;
                        //Langauge Code
                        case 0x0006:
                            LangaugeCodePacket langCode = new LangaugeCodePacket(subpacket.data);
                            player.languageCode = langCode.languageCode;
                            break;
                        //Unknown - Happens a lot at login, then once every time player zones
                        case 0x0007:
                            //subpacket.DebugPrintSubPacket();
                            _0x07Packet unknown07 = new _0x07Packet(subpacket.data);
                            break;
                        //Update Position
                        case 0x00CA:
                            //Update Position
                            //subpacket.DebugPrintSubPacket();
                            UpdatePlayerPositionPacket posUpdate = new UpdatePlayerPositionPacket(subpacket.data);
                            player.UpdatePlayerActorPosition(posUpdate.x, posUpdate.y, posUpdate.z, posUpdate.rot, posUpdate.moveState);
                            player.GetActor().SendInstanceUpdate();

                            if (player.GetActor().IsInZoneChange())
                                player.GetActor().SetZoneChanging(false);

                            break;
                        //Set Target 
                        case 0x00CD:
                            //subpacket.DebugPrintSubPacket();

                            SetTargetPacket setTarget = new SetTargetPacket(subpacket.data);
                            player.GetActor().currentTarget = setTarget.actorID;
                            player.GetActor().BroadcastPacket(SetActorTargetAnimatedPacket.BuildPacket(player.actorID, player.actorID, setTarget.actorID), true);
                            break;
                        //Lock Target
                        case 0x00CC:
                            LockTargetPacket lockTarget = new LockTargetPacket(subpacket.data);
                            player.GetActor().currentLockedTarget = lockTarget.actorID;
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

                            Actor ownerActor = Server.GetStaticActors(eventStart.scriptOwnerActorID);
                            
                     
                            player.GetActor().currentEventOwner = eventStart.scriptOwnerActorID;
                            player.GetActor().currentEventName = eventStart.triggerName;
                    

                            if (ownerActor == null)
                            {
                                //Is it a instance actor?
                                ownerActor = Server.GetWorldManager().GetActorInWorld(player.GetActor().currentEventOwner);
                                if (ownerActor == null)
                                {
                                    //Is it a Director?
                                    if (player.GetActor().currentDirector != null && player.GetActor().currentEventOwner == player.GetActor().currentDirector.actorId)
                                        ownerActor = player.GetActor().currentDirector;
                                    else
                                    {
                                        Program.Log.Debug("\n===Event START===\nCould not find actor 0x{0:X} for event started by caller: 0x{1:X}\nEvent Starter: {2}\nParams: {3}", eventStart.actorID, eventStart.scriptOwnerActorID, eventStart.triggerName, LuaUtils.DumpParams(eventStart.luaParams));
                                        break;
                                    }
                                }                                    
                            }

                            player.GetActor().StartEvent(ownerActor, eventStart);

                            Program.Log.Debug("\n===Event START===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nEvent Starter: {4}\nParams: {5}", eventStart.actorID, eventStart.scriptOwnerActorID, eventStart.val1, eventStart.val2, eventStart.triggerName, LuaUtils.DumpParams(eventStart.luaParams));
                            break;
                        //Unknown, happens at npc spawn and cutscene play????
                        case 0x00CE:
                            break;
                        //Event Result
                        case 0x012E:
                            subpacket.DebugPrintSubPacket();
                            EventUpdatePacket eventUpdate = new EventUpdatePacket(subpacket.data);
                            Program.Log.Debug("\n===Event UPDATE===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nStep: 0x{4:X}\nParams: {5}", eventUpdate.actorID, eventUpdate.scriptOwnerActorID, eventUpdate.val1, eventUpdate.val2, eventUpdate.step, LuaUtils.DumpParams(eventUpdate.luaParams));
                            /*
                            //Is it a static actor? If not look in the player's instance
                            Actor updateOwnerActor = Server.GetStaticActors(player.GetActor().currentEventOwner);
                            if (updateOwnerActor == null)
                            {
                                updateOwnerActor = Server.GetWorldManager().GetActorInWorld(player.GetActor().currentEventOwner);

                                if (player.GetActor().currentDirector != null && player.GetActor().currentEventOwner == player.GetActor().currentDirector.actorId)
                                    updateOwnerActor = player.GetActor().currentDirector;

                                if (updateOwnerActor == null)
                                    break;
                            }
                            */
                            player.GetActor().UpdateEvent(eventUpdate);

                            //LuaEngine.DoActorOnEventUpdated(player.GetActor(), updateOwnerActor, eventUpdate);
                            
                            break;
                        case 0x012F:
                            //subpacket.DebugPrintSubPacket();
                            ParameterDataRequestPacket paramRequest = new ParameterDataRequestPacket(subpacket.data);
                            if (paramRequest.paramName.Equals("charaWork/exp"))
                                player.GetActor().SendCharaExpInfo();
                            break;
                        /* RECRUITMENT */
                        //Start Recruiting
                        case 0x01C3:
                            StartRecruitingRequestPacket recruitRequestPacket = new StartRecruitingRequestPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(StartRecruitingResponse.BuildPacket(player.actorID, true), true, false));
                            break;
                        //End Recruiting
                        case 0x01C4:
                            client.QueuePacket(BasePacket.CreatePacket(EndRecruitmentPacket.BuildPacket(player.actorID), true, false));
                            break;
                        //Party Window Opened, Request State
                        case 0x01C5:
                            client.QueuePacket(BasePacket.CreatePacket(RecruiterStatePacket.BuildPacket(player.actorID, true, true, 1), true, false));
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
                            client.QueuePacket(BasePacket.CreatePacket(CurrentRecruitmentDetailsPacket.BuildPacket(player.actorID, details), true, false));
                            break;
                        //Accepted Recruiting
                        case 0x01C6:
                            subpacket.DebugPrintSubPacket();
                            break;
                        /* SOCIAL STUFF */
                        case 0x01C9:
                            AddRemoveSocialPacket addBlackList = new AddRemoveSocialPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(BlacklistAddedPacket.BuildPacket(player.actorID, true, addBlackList.name), true, false));
                            break;
                        case 0x01CA:
                            AddRemoveSocialPacket RemoveBlackList = new AddRemoveSocialPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(BlacklistRemovedPacket.BuildPacket(player.actorID, true, RemoveBlackList.name), true, false));
                            break;
                        case 0x01CB:
                            int offset1 = 0;
                            client.QueuePacket(BasePacket.CreatePacket(SendBlacklistPacket.BuildPacket(player.actorID, new String[] { "Test" }, ref offset1), true, false));
                            break;
                        case 0x01CC:
                            AddRemoveSocialPacket addFriendList = new AddRemoveSocialPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(FriendlistAddedPacket.BuildPacket(player.actorID, true, (uint)addFriendList.name.GetHashCode(), true, addFriendList.name), true, false));
                            break;
                        case 0x01CD:
                            AddRemoveSocialPacket RemoveFriendList = new AddRemoveSocialPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(FriendlistRemovedPacket.BuildPacket(player.actorID, true, RemoveFriendList.name), true, false));
                            break;
                        case 0x01CE:
                            int offset2 = 0;
                            client.QueuePacket(BasePacket.CreatePacket(SendFriendlistPacket.BuildPacket(player.actorID, new Tuple<long, string>[] { new Tuple<long, string>(01, "Test2") }, ref offset2), true, false));
                            break;
                        case 0x01CF:
                            client.QueuePacket(BasePacket.CreatePacket(FriendStatusPacket.BuildPacket(player.actorID, null), true, false));
                            break;
                        /* SUPPORT DESK STUFF */
                        //Request for FAQ/Info List
                        case 0x01D0:
                            FaqListRequestPacket faqRequest = new FaqListRequestPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(FaqListResponsePacket.BuildPacket(player.actorID, Database.getFAQNames(faqRequest.langCode)), true, false));
                            break;
                        //Request for body of a faq/info selection
                        case 0x01D1:
                            FaqBodyRequestPacket faqBodyRequest = new FaqBodyRequestPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(FaqBodyResponsePacket.BuildPacket(player.actorID, Database.getFAQBody(faqBodyRequest.faqIndex, faqBodyRequest.langCode)), true, false));
                            break;
                        //Request issue list
                        case 0x01D2:
                            GMTicketIssuesRequestPacket issuesRequest = new GMTicketIssuesRequestPacket(subpacket.data);
                            client.QueuePacket(BasePacket.CreatePacket(IssueListResponsePacket.BuildPacket(player.actorID, Database.getIssues(issuesRequest.langCode)), true, false));
                            break;
                        //Request if GM ticket exists
                        case 0x01D3:
                            client.QueuePacket(BasePacket.CreatePacket(StartGMTicketPacket.BuildPacket(player.actorID, false), true, false));
                            break;
                        //Request for GM response message
                        case 0x01D4:
                            client.QueuePacket(BasePacket.CreatePacket(GMTicketPacket.BuildPacket(player.actorID, "Ticket Title", "Enter your Help request here."), true, false));
                            break;
                        //GM Ticket Sent
                        case 0x01D5:
                            GMSupportTicketPacket gmTicket = new GMSupportTicketPacket(subpacket.data);
                            Program.Log.Info("Got GM Ticket: \n" + gmTicket.ticketTitle + "\n" + gmTicket.ticketBody);
                            Database.SaveSupportTicket(gmTicket);
                            client.QueuePacket(BasePacket.CreatePacket(GMTicketSentResponsePacket.BuildPacket(player.actorID, true), true, false));
                            break;
                        //Request to end ticket
                        case 0x01D6:
                            client.QueuePacket(BasePacket.CreatePacket(EndGMTicketPacket.BuildPacket(player.actorID), true, false));
                            break;
                        default:
                            Program.Log.Debug("Unknown command 0x{0:X} received.", subpacket.gameMessage.opcode);
                            subpacket.DebugPrintSubPacket();
                            break;
                    }
                }
                else
                    packet.DebugPrintPacket();
            }
        }        

    }
}

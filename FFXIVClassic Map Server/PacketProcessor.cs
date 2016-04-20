using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.packets.receive;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.login;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
using FFXIVClassic_Map_Server.packets.send.Actor;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.packets.send.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.social;
using FFXIVClassic_Map_Server.packets.send.social;
using FFXIVClassic_Map_Server.packets.receive.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.recruitment;
using FFXIVClassic_Map_Server.packets.send.recruitment;
using FFXIVClassic_Map_Server.packets.send.list;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send.events;
using FFXIVClassic_Map_Server.lua;
using System.Net;
using FFXIVClassic_Map_Server.common.EfficientHashTables;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Lobby_Server
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

        public void processPacket(ClientConnection client, BasePacket packet)
        {                      
            if (packet.header.isCompressed == 0x01)                       
                BasePacket.decryptPacket(client.blowfish, ref packet);

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                if (subpacket.header.type == 0x01)
                {                 
                    packet.debugPrintPacket();
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
                        while (!mPlayers.ContainsKey(client.owner))
                        { }
                        player = mPlayers[client.owner];
                    }

                    //Create connected player if not created
                    if (player == null)
                    { 
                        player = new ConnectedPlayer(actorID);
                        mPlayers[actorID] = player;
                    }
                    
                    player.setConnection(packet.header.connectionType, client);

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                        Log.debug(String.Format("Got {0} connection for ActorID {1} @ {2}.", "zone", actorID, client.getAddress()));
                    else if (packet.header.connectionType == BasePacket.TYPE_CHAT)
                        Log.debug(String.Format("Got {0} connection for ActorID {1} @ {2}.", "chat", actorID, client.getAddress()));

                    //Create player actor
                    reply1.debugPrintPacket();
                    client.queuePacket(reply1);
                    client.queuePacket(reply2);
                    break;
                }
                else if (subpacket.header.type == 0x07)
                {
                    BasePacket init = Login0x7ResponsePacket.buildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC());
                    //client.queuePacket(init);
                }
                else if (subpacket.header.type == 0x08)
                {
                    //Response, client's current [actorID][time]
                    packet.debugPrintPacket();
                }
                else if (subpacket.header.type == 0x03)
                {
                    ConnectedPlayer player = null;

                    if(mPlayers.ContainsKey(client.owner))
                        player = mPlayers[client.owner];

                    if (player == null)
                        return;

                    //Normal Game Opcode
                    switch (subpacket.gameMessage.opcode)
                    {
                        //Ping
                        case 0x0001:
                            //subpacket.debugPrintSubPacket();
                            PingPacket pingPacket = new PingPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(PongPacket.buildPacket(player.actorID, pingPacket.time), true, false));
                            player.ping();
                            break;
                        //Unknown
                        case 0x0002:

                            subpacket.debugPrintSubPacket();
                            client.queuePacket(_0x2Packet.buildPacket(player.actorID), true, false);

                            Server.GetWorldManager().DoLogin(player.getActor());


                            break;
                        //Chat Received
                        case 0x0003:
                            ChatMessagePacket chatMessage = new ChatMessagePacket(subpacket.data);
                            Log.info(String.Format("Got type-{5} message: {0} @ {1}, {2}, {3}, Rot: {4}", chatMessage.message, chatMessage.posX, chatMessage.posY, chatMessage.posZ, chatMessage.posRot, chatMessage.logType));
                            subpacket.debugPrintSubPacket();

                            if (chatMessage.message.StartsWith("!"))
                            {
                                if (cp.doCommand(chatMessage.message, player))
                                    continue;
                            }                           

                            player.getActor().broadcastPacket(SendMessagePacket.buildPacket(player.actorID, player.actorID, chatMessage.logType, player.getActor().customDisplayName, chatMessage.message), false);

                            break;
                        //Langauge Code
                        case 0x0006:
                            LangaugeCodePacket langCode = new LangaugeCodePacket(subpacket.data);
                            player.languageCode = langCode.languageCode;
                            break;
                        //Unknown - Happens a lot at login, then once every time player zones
                        case 0x0007:
                            //subpacket.debugPrintSubPacket();
                            _0x07Packet unknown07 = new _0x07Packet(subpacket.data);
                            break;
                        //Update Position
                        case 0x00CA:
                            //Update Position
                            //subpacket.debugPrintSubPacket();
                            UpdatePlayerPositionPacket posUpdate = new UpdatePlayerPositionPacket(subpacket.data);
                            player.updatePlayerActorPosition(posUpdate.x, posUpdate.y, posUpdate.z, posUpdate.rot, posUpdate.moveState);
                            player.getActor().sendInstanceUpdate();

                            if (player.getActor().isInZoneChange())
                                player.getActor().setZoneChanging(false);

                            break;
                        //Set Target 
                        case 0x00CD:
                            //subpacket.debugPrintSubPacket();

                            SetTargetPacket setTarget = new SetTargetPacket(subpacket.data);
                            player.getActor().currentTarget = setTarget.actorID;
                            player.getActor().broadcastPacket(SetActorTargetAnimatedPacket.buildPacket(player.actorID, player.actorID, setTarget.actorID), true);
                            break;
                        //Lock Target
                        case 0x00CC:
                            LockTargetPacket lockTarget = new LockTargetPacket(subpacket.data);
                            player.getActor().currentLockedTarget = lockTarget.actorID;
                            break;
                        //Start Event
                        case 0x012D:
                            subpacket.debugPrintSubPacket();
                            EventStartPacket eventStart = new EventStartPacket(subpacket.data);

                            /*
                            if (eventStart.error != null)
                            {
                                player.errorMessage += eventStart.error;

                                if (eventStart.errorIndex == eventStart.errorNum - 1)
                                    Log.error("\n"+player.errorMessage);


                                break;
                            }
                            */

                            Actor ownerActor = Server.getStaticActors(eventStart.scriptOwnerActorID);
                            if (ownerActor != null && ownerActor is Command)
                            {
                                player.getActor().currentCommand = eventStart.scriptOwnerActorID;
                                player.getActor().currentCommandName = eventStart.triggerName;
                            }
                            else
                            {
                                player.getActor().currentEventOwner = eventStart.scriptOwnerActorID;
                                player.getActor().currentEventName = eventStart.triggerName;
                            }

                            if (ownerActor == null)
                            {
                                //Is it a instance actor?
                                ownerActor = Server.GetWorldManager().GetActorInWorld(player.getActor().currentEventOwner);
                                if (ownerActor == null)
                                {
                                    //Is it a Director?
                                    if (player.getActor().currentDirector != null && player.getActor().currentEventOwner == player.getActor().currentDirector.actorId)
                                        ownerActor = player.getActor().currentDirector;
                                    else
                                    {
                                        Log.debug(String.Format("\n===Event START===\nCould not find actor 0x{0:X} for event started by caller: 0x{1:X}\nEvent Starter: {2}\nParams: {3}", eventStart.actorID, eventStart.scriptOwnerActorID, eventStart.triggerName, LuaUtils.dumpParams(eventStart.luaParams)));
                                        break;
                                    }
                                }                                    
                            }
                            
                            LuaEngine.doActorOnEventStarted(player.getActor(), ownerActor, eventStart);

                            Log.debug(String.Format("\n===Event START===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nEvent Starter: {4}\nParams: {5}", eventStart.actorID, eventStart.scriptOwnerActorID, eventStart.val1, eventStart.val2, eventStart.triggerName, LuaUtils.dumpParams(eventStart.luaParams)));
                            break;
                        //Unknown, happens at npc spawn and cutscene play????
                        case 0x00CE:
                            break;
                        //Event Result
                        case 0x012E:
                            subpacket.debugPrintSubPacket();
                            EventUpdatePacket eventUpdate = new EventUpdatePacket(subpacket.data);
                            Log.debug(String.Format("\n===Event UPDATE===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nStep: 0x{4:X}\nParams: {5}", eventUpdate.actorID, eventUpdate.scriptOwnerActorID, eventUpdate.val1, eventUpdate.val2, eventUpdate.step, LuaUtils.dumpParams(eventUpdate.luaParams)));

                            //Is it a static actor? If not look in the player's instance
                            Actor updateOwnerActor = Server.getStaticActors(player.getActor().currentEventOwner);
                            if (updateOwnerActor == null)
                            {
                                updateOwnerActor = Server.GetWorldManager().GetActorInWorld(player.getActor().currentEventOwner);

                                if (player.getActor().currentDirector != null && player.getActor().currentEventOwner == player.getActor().currentDirector.actorId)
                                    updateOwnerActor = player.getActor().currentDirector;

                                if (updateOwnerActor == null)
                                    break;
                            }

                            LuaEngine.doActorOnEventUpdated(player.getActor(), updateOwnerActor, eventUpdate);
                            
                            break;
                        case 0x012F:
                            subpacket.debugPrintSubPacket();
                            ParameterDataRequestPacket paramRequest = new ParameterDataRequestPacket(subpacket.data);
                            if (paramRequest.paramName.Equals("charaWork/exp"))
                                player.getActor().sendCharaExpInfo();
                            break;
                        /* RECRUITMENT */
                        //Start Recruiting
                        case 0x01C3:
                            StartRecruitingRequestPacket recruitRequestPacket = new StartRecruitingRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(StartRecruitingResponse.buildPacket(player.actorID, true), true, false));
                            break;
                        //End Recruiting
                        case 0x01C4:
                            client.queuePacket(BasePacket.createPacket(EndRecruitmentPacket.buildPacket(player.actorID), true, false));
                            break;
                        //Party Window Opened, Request State
                        case 0x01C5:
                            client.queuePacket(BasePacket.createPacket(RecruiterStatePacket.buildPacket(player.actorID, true, true, 1), true, false));
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
                            details.comment = "This is a test details packet sent by the server. No implementation has been created yet...";
                            details.num[0] = 1;
                            client.queuePacket(BasePacket.createPacket(CurrentRecruitmentDetailsPacket.buildPacket(player.actorID, details), true, false));
                            break;
                        //Accepted Recruiting
                        case 0x01C6:
                            subpacket.debugPrintSubPacket();
                            break;
                        /* SOCIAL STUFF */
                        case 0x01C9:
                            AddRemoveSocialPacket addBlackList = new AddRemoveSocialPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(BlacklistAddedPacket.buildPacket(player.actorID, true, addBlackList.name), true, false));
                            break;
                        case 0x01CA:
                            AddRemoveSocialPacket removeBlackList = new AddRemoveSocialPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(BlacklistRemovedPacket.buildPacket(player.actorID, true, removeBlackList.name), true, false));
                            break;
                        case 0x01CB:
                            int offset1 = 0;
                            client.queuePacket(BasePacket.createPacket(SendBlacklistPacket.buildPacket(player.actorID, new String[] { "Test" }, ref offset1), true, false));
                            break;
                        case 0x01CC:
                            AddRemoveSocialPacket addFriendList = new AddRemoveSocialPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(FriendlistAddedPacket.buildPacket(player.actorID, true, (uint)addFriendList.name.GetHashCode(), true, addFriendList.name), true, false));
                            break;
                        case 0x01CD:
                            AddRemoveSocialPacket removeFriendList = new AddRemoveSocialPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(FriendlistRemovedPacket.buildPacket(player.actorID, true, removeFriendList.name), true, false));
                            break;
                        case 0x01CE:
                            int offset2 = 0;
                            client.queuePacket(BasePacket.createPacket(SendFriendlistPacket.buildPacket(player.actorID, new Tuple<long, string>[] { new Tuple<long, string>(01, "Test2") }, ref offset2), true, false));
                            break;
                        case 0x01CF:
                            client.queuePacket(BasePacket.createPacket(FriendStatusPacket.buildPacket(player.actorID, null), true, false));
                            break;
                        /* SUPPORT DESK STUFF */
                        //Request for FAQ/Info List
                        case 0x01D0:
                            FaqListRequestPacket faqRequest = new FaqListRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(FaqListResponsePacket.buildPacket(player.actorID, new string[] { "Testing FAQ1", "Coded style!" }), true, false));
                            break;
                        //Request for body of a faq/info selection
                        case 0x01D1:
                            FaqBodyRequestPacket faqBodyRequest = new FaqBodyRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(FaqBodyResponsePacket.buildPacket(player.actorID, "HERE IS A GIANT BODY. Nothing else to say!"), true, false));
                            break;
                        //Request issue list
                        case 0x01D2:
                            GMTicketIssuesRequestPacket issuesRequest = new GMTicketIssuesRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(IssueListResponsePacket.buildPacket(player.actorID, new string[] { "Test1", "Test2", "Test3", "Test4", "Test5" }), true, false));
                            break;
                        //Request if GM ticket exists
                        case 0x01D3:
                            client.queuePacket(BasePacket.createPacket(StartGMTicketPacket.buildPacket(player.actorID, false), true, false));
                            break;
                        //Request for GM response message
                        case 0x01D4:
                            client.queuePacket(BasePacket.createPacket(GMTicketPacket.buildPacket(player.actorID, "This is a GM Ticket Title", "This is a GM Ticket Body."), true, false));
                            break;
                        //GM Ticket Sent
                        case 0x01D5:
                            GMSupportTicketPacket gmTicket = new GMSupportTicketPacket(subpacket.data);
                            Log.info("Got GM Ticket: \n" + gmTicket.ticketTitle + "\n" + gmTicket.ticketBody);
                            client.queuePacket(BasePacket.createPacket(GMTicketSentResponsePacket.buildPacket(player.actorID, true), true, false));
                            break;
                        //Request to end ticket
                        case 0x01D6:
                            client.queuePacket(BasePacket.createPacket(EndGMTicketPacket.buildPacket(player.actorID), true, false));
                            break;
                        default:
                            Log.debug(String.Format("Unknown command 0x{0:X} received.", subpacket.gameMessage.opcode));
                            subpacket.debugPrintSubPacket();
                            break;
                    }
                }
                else
                    packet.debugPrintPacket();
            }
        }        

    }
}

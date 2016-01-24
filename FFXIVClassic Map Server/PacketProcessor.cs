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

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
        Server mServer;
        Dictionary<uint, ConnectedPlayer> mPlayers;
        List<ClientConnection> mConnections;

        public PacketProcessor(Server server, Dictionary<uint, ConnectedPlayer> playerList, List<ClientConnection> connectionList)
        {
            mPlayers = playerList;
            mConnections = connectionList;
            mServer = server;
        }     

        public void processPacket(ClientConnection client, BasePacket packet)
        {                      
            if (packet.header.isCompressed == 0x01)                       
                BasePacket.decryptPacket(client.blowfish, ref packet);

          // packet.debugPrintPacket();

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                if (subpacket.header.type == 0x01)
                {                 
                    packet.debugPrintPacket();
                    byte[] reply1Data = {
                                            0x01, 0x00, 0x00, 0x00, 0x28, 0x0, 0x01, 0x0, 0x0, 0x0, 0x0, 0x0, 0x00, 0x00, 0x00, 0x00,
                                            0x18, 0x00, 0x07, 0x00, 0x00, 0x0, 0x00, 0x0, 0x0, 0x0, 0x0, 0x0, 0x7F, 0xFD, 0xFF, 0xFF,
                                            0x43, 0xEC, 0x00, 0xE0, 0x00, 0x0, 0x00, 0x0
                                        };

                    BasePacket reply1 = new BasePacket(reply1Data);
                    BasePacket reply2 = new BasePacket("./packets/login/login2.bin");

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
                    //Ping?
                    //packet.debugPrintPacket();
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
                    ConnectedPlayer player = mPlayers[client.owner];

                    //Normal Game Opcode
                    switch (subpacket.gameMessage.opcode)
                    {
                        //Ping
                        case 0x0001:
                            //subpacket.debugPrintSubPacket();
                            PingPacket pingPacket = new PingPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(PongPacket.buildPacket(player.actorID, pingPacket.time), true, false));
                            break;
                        //Unknown
                        case 0x0002:

                            client.queuePacket(SendMessagePacket.buildPacket(player.actorID, player.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "-------- Login Message --------\nWelcome to the 1.0 Dev Server"), true, false);
                            mServer.GetWorldManager().DoLogin(player.getActor());


                            break;
                        //Chat Received
                        case 0x0003:
                            ChatMessagePacket chatMessage = new ChatMessagePacket(subpacket.data);
                            Log.info(String.Format("Got type-{5} message: {0} @ {1}, {2}, {3}, Rot: {4}", chatMessage.message, chatMessage.posX, chatMessage.posY, chatMessage.posZ, chatMessage.posRot, chatMessage.logType));
                            //subpacket.debugPrintSubPacket();

                            mServer.doCommand(chatMessage.message, player);

                            break;
                        //Unknown
                        case 0x0007:
                            break;
                        //Update Position
                        case 0x00CA:
                            //Update Position
                            UpdatePlayerPositionPacket posUpdate = new UpdatePlayerPositionPacket(subpacket.data);
                            player.updatePlayerActorPosition(posUpdate.x, posUpdate.y, posUpdate.z, posUpdate.rot, posUpdate.moveState);
                            
                            //Update Instance
                            List<BasePacket> instanceUpdatePackets = player.updateInstance(player.getActor().zone.getActorsAroundActor(player.getActor(), 50));
                            foreach (BasePacket bp in instanceUpdatePackets)
                            {
                            //    bp.debugPrintPacket();
                                client.queuePacket(bp);
                            }
                            break;
                        //Set Target 
                        case 0x00CD:
                            //subpacket.debugPrintSubPacket();

                            SetTargetPacket setTarget = new SetTargetPacket(subpacket.data);
                            player.getActor().currentTarget = setTarget.actorID;
                            client.queuePacket(BasePacket.createPacket(SetActorTargetAnimatedPacket.buildPacket(player.actorID, player.actorID, setTarget.actorID), true, false));
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
                            player.eventCurrentOwner = eventStart.scriptOwnerActorID;
                            player.eventCurrentStarter = eventStart.eventStarter;

                            //Is it a static actor? If not look in the player's instance
                            //Actor ownerActor = findActor(player, player.eventCurrentOwner);

                            //if (ownerActor == null)
                              //  break;

                            //luaEngine.doEventStart(player, ownerActor, eventStart);

                            //Log.debug(String.Format("\n===Event START===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nEvent Starter: {4}\nParams: {5}", eventStart.actorID, eventStart.scriptOwnerActorID, eventStart.val1, eventStart.val2, eventStart.eventStarter, LuaParamReader.dumpParams(eventStart.luaParams)));
                            break;
                        //Event Result
                        case 0x012E:
                            subpacket.debugPrintSubPacket();
                            EventUpdatePacket eventUpdate = new EventUpdatePacket(subpacket.data);
                            Log.debug(String.Format("\n===Event UPDATE===\nSource Actor: 0x{0:X}\nCaller Actor: 0x{1:X}\nVal1: 0x{2:X}\nVal2: 0x{3:X}\nFunction ID: 0x{4:X}\nParams: {5}", eventUpdate.actorID, eventUpdate.scriptOwnerActorID, eventUpdate.val1, eventUpdate.val2, eventUpdate.step, LuaUtils.dumpParams(eventUpdate.luaParams)));

                            /*Actor updateOwnerActor = findActor(player, player.eventCurrentOwner);
                            if (updateOwnerActor == null)
                                break;

                            luaEngine.doEventUpdated(player, updateOwnerActor, eventUpdate);
                            */
                            break;
                        case 0x012F:
                            subpacket.debugPrintSubPacket();
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

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
using FFXIVClassic_Map_Server.packets.send.script;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.packets.send.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.social;
using FFXIVClassic_Map_Server.packets.send.social;
using FFXIVClassic_Map_Server.packets.receive.supportdesk;
using FFXIVClassic_Map_Server.packets.receive.recruitment;
using FFXIVClassic_Map_Server.packets.send.recruitment;

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
        Dictionary<uint, ConnectedPlayer> mPlayers;
        List<ClientConnection> mConnections;

        Zone inn = new Zone();

        public PacketProcessor(Dictionary<uint, ConnectedPlayer> playerList, List<ClientConnection> connectionList)
        {
            mPlayers = playerList;
            mConnections = connectionList;
        }     

        public void processPacket(ClientConnection client, BasePacket packet)
        {
            ConnectedPlayer player = null;
            if (client.owner != 0 && mPlayers.ContainsKey(client.owner))
                player = mPlayers[client.owner];
            
            if (packet.header.isEncrypted == 0x01)                       
                BasePacket.decryptPacket(client.blowfish, ref packet);

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                if (subpacket.header.type == 0x01)
                {                                   
                    BasePacket reply2 = new BasePacket("./packets/login/login2.bin");

                    //Already Handshaked
                    if (client.owner != 0)
                    {
                        using (MemoryStream mem = new MemoryStream(reply2.data))
                        {
                            using (BinaryWriter binReader = new BinaryWriter(mem))
                            {
                                binReader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                                binReader.Write(player.actorID);
                            }
                        }

                        
                        client.queuePacket(reply2);
                        break;
                    }

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

                    if (actorID == 0)
                        break;

                    //Second connection
                    if (mPlayers.ContainsKey(actorID))
                        player = mPlayers[actorID];

                    using (MemoryStream mem = new MemoryStream(reply2.data))
                    {
                        using (BinaryWriter binReader = new BinaryWriter(mem))
                        {
                            binReader.BaseStream.Seek(0x10, SeekOrigin.Begin);
                            binReader.Write(actorID);
                        }
                    }

                    if (player == null)
                    {
                        player = new ConnectedPlayer(actorID);
                        mPlayers[actorID] = player;
                        client.owner = actorID;
                        client.connType = 0;
                        player.setConnection1(client);
                    Log.debug(String.Format("Got actorID {0} for conn {1}.", actorID, client.getAddress()));
                    }
                    else
                    {
                        client.owner = actorID;
                        client.connType = 1;
                        player.setConnection2(client);
                    }

                    //Get Character info
                    //Create player actor
                    client.queuePacket(reply2);
                    break;
                }
                else if (subpacket.header.type == 0x07)
                {
                    BasePacket init = InitPacket.buildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC());
                    client.queuePacket(init);
                }
                else if (subpacket.header.type == 0x03)
                {
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
                            
                            subpacket.debugPrintSubPacket();

                            BasePacket reply5 = new BasePacket("./packets/login/login5.bin");
                            BasePacket reply6 = new BasePacket("./packets/login/login6_data.bin");
                            BasePacket reply7 = new BasePacket("./packets/login/login7_data.bin");
                            BasePacket reply8 = new BasePacket("./packets/login/login8_data.bin");
                            BasePacket reply9 = new BasePacket("./packets/login/login9_zonesetup.bin");
                            BasePacket reply10 = new BasePacket("./packets/login/login10.bin");
                            BasePacket reply11 = new BasePacket("./packets/login/login11.bin");
                            BasePacket reply12 = new BasePacket("./packets/login/login12.bin");

                          //  BasePacket keyitems = new BasePacket("./packets/login/keyitems.bin");
                          //  BasePacket currancy = new BasePacket("./packets/login/currancy.bin");

                            #region replaceid
                            //currancy.replaceActorID(player.actorID);
                            //keyitems.replaceActorID(player.actorID);

                            reply5.replaceActorID(player.actorID);
                            reply6.replaceActorID(player.actorID);
                            reply7.replaceActorID(player.actorID);
                            reply8.replaceActorID(player.actorID);
                            reply9.replaceActorID(player.actorID);
                            reply10.replaceActorID(player.actorID);
                            reply11.replaceActorID(player.actorID);
                            reply12.replaceActorID(player.actorID);
                            #endregion

                            client.queuePacket(BasePacket.createPacket(_0x2Packet.buildPacket(player.actorID), true, false));                           
                          //  return;
                            client.queuePacket(BasePacket.createPacket(SetMapPacket.buildPacket(player.actorID, 0xD1, 0xF4), true, false));
                            client.queuePacket(BasePacket.createPacket(SetMusicPacket.buildPacket(player.actorID, 0x3D, 0x01), true, false));

                            client.queuePacket(reply5);

                            client.queuePacket(BasePacket.createPacket(AddActorPacket.buildPacket(player.actorID, player.actorID, 0), true, false));

                            BasePacket actorPacket = player.getActor().createActorSpawnPackets(player.actorID);
                            //actorPacket.debugPrintPacket();
                            client.queuePacket(actorPacket);
                            client.queuePacket(reply6);
                            client.queuePacket(BasePacket.createPacket(new SubPacket(0xCC, player.actorID, player.actorID,  File.ReadAllBytes("./packets/playerscript")),true, false));
                            client.queuePacket(BasePacket.createPacket(player.getActor().createInitSubpackets(player.actorID), true, false));

                            /*
                            client.queuePacket(BasePacket.createPacket(SetActorPositionPacket.buildPacket(player.actorID, player.actorID, SetActorPositionPacket.INNPOS_X, SetActorPositionPacket.INNPOS_Y, SetActorPositionPacket.INNPOS_Z, SetActorPositionPacket.INNPOS_ROT, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE), true, false));
                            client.queuePacket(BasePacket.createPacket(player.getActor().createSpeedPacket(player.actorID), true, false));
                            client.queuePacket(BasePacket.createPacket(player.getActor().createStatePacket(player.actorID), true, false));

                            client.queuePacket(BasePacket.createPacket(player.getActor().createNamePacket(player.actorID), true, false));
                            client.queuePacket(BasePacket.createPacket(player.getActor().createAppearancePacket(player.actorID), true, false));
                            */

                            ////////ITEMS////////
                            client.queuePacket(BasePacket.createPacket(InventoryBeginChangePacket.buildPacket(player.actorID), true, false));

                            #region itemsetup

                            //TEST
                            List<Item> items = new List<Item>();
                            items.Add(new Item(1337, 8030920, 5)); //Leather Jacket
                            items.Add(new Item(1338, 8013626, 1)); //Chocobo Mask
                            items.Add(new Item(1339, 5030402, 2)); //Thyrus
                            items.Add(new Item(1340, 8013635, 3)); //Dalamud Horn
                            items.Add(new Item(1341, 10100132, 4)); //Savage Might 4
                            items.Add(new Item(1342, 8032407, 6)); //Green Summer Halter (Female)
                            items.Add(new Item(1343, 8051307, 7)); //Green Summer Tanga (Female)
                            items.Add(new Item(1344, 8050766, 8)); //Flame Private's Saroul

                            int count = 0;

                            items[2].isHighQuality = true;
                            items[0].durability = 9999;
                            items[0].spiritbind = 10000;
                            items[0].materia1 = 6;
                            items[0].materia2 = 7;
                            items[0].materia3 = 8;
                            items[0].materia4 = 9;
                            items[0].materia5 = 10;
                            items[1].durability = 9999;
                            items[2].durability = 0xFFFFFFF;
                            items[3].durability = 9999;
                            items[4].quantity = 99;

                            //Reused
                            SubPacket endInventory = InventorySetEndPacket.buildPacket(player.actorID);
                            SubPacket beginInventory = InventorySetBeginPacket.buildPacket(player.actorID, 200, 00);
                            SubPacket setInventory = InventoryItemPacket.buildPacket(player.actorID, items, ref count);

                            List<SubPacket> setinvPackets = new List<SubPacket>();
                            setinvPackets.Add(beginInventory);
                            setinvPackets.Add(setInventory);
                            setinvPackets.Add(endInventory);
                            #endregion

                            client.queuePacket(BasePacket.createPacket(setinvPackets, true, false));

                            //client.queuePacket(currancy);
                            //client.queuePacket(keyitems);

                            #region equipsetup
                            EquipmentSetupPacket initialEqupmentPacket = new EquipmentSetupPacket();
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_BODY, 5);
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_HEAD, 3);
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_UNDERSHIRT, 6);
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_UNDERGARMENT, 7);
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_MAINHAND, 2);
                            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_LEGS, 8);
                            #endregion

                            //Equip Init
                            client.queuePacket(BasePacket.createPacket(InventorySetBeginPacket.buildPacket(player.actorID, 0x23, InventorySetBeginPacket.CODE_EQUIPMENT), true, false));
                            client.queuePacket(BasePacket.createPacket(initialEqupmentPacket.buildPackets(player.actorID), true, false));
                            client.queuePacket(BasePacket.createPacket(InventorySetEndPacket.buildPacket(player.actorID), true, false));

                            client.queuePacket(BasePacket.createPacket(InventoryEndChangePacket.buildPacket(player.actorID), true, false));
                            ////////ITEMS////////

                            //The rest of hardcode
                            client.queuePacket(BasePacket.createPacket(SetGrandCompanyPacket.buildPacket(player.actorID, player.actorID, 0x01, 0x1B, 0x1B, 0x1B), true, false));                  
                            client.queuePacket(BasePacket.createPacket(SetPlayerTitlePacket.buildPacket(player.actorID, player.actorID, 0x00), true, false));
                            client.queuePacket(BasePacket.createPacket(SetHasChocoboPacket.buildPacket(player.actorID, true), true, false));
                            client.queuePacket(BasePacket.createPacket(SetChocoboNamePacket.buildPacket(player.actorID, player.actorID, "Boco"), true, false));                            
                            client.queuePacket(BasePacket.createPacket(SetPlayerTitlePacket.buildPacket(player.actorID, player.actorID, 0x00), true, false));

                            SetCompletedAchievementsPacket cheevos = new SetCompletedAchievementsPacket();

                            for (int i = 0; i < cheevos.achievementFlags.Length; i++)
                                cheevos.achievementFlags[i] = true;

                            client.queuePacket(BasePacket.createPacket(cheevos.buildPacket(player.actorID), true, false));
                            client.queuePacket(BasePacket.createPacket(SetLatestAchievementsPacket.buildPacket(player.actorID, new uint[5]), true, false));
                            client.queuePacket(BasePacket.createPacket(SetAchievementPointsPacket.buildPacket(player.actorID, 0x00), true, false));
                            SetCutsceneBookPacket book = new SetCutsceneBookPacket();
                            client.queuePacket(BasePacket.createPacket(book.buildPacket(player.actorID), true, false));
                            //client.queuePacket(BasePacket.createPacket(SetPlayerDreamPacket.buildPacket(player.actorID, player.actorID, 0x00), true, false));                            

                            client.queuePacket(BasePacket.createPacket(SetStatusPacket.buildPacket(player.actorID, player.actorID, new ushort[] { 23263, 23264 }), true, false));
                            BasePacket tpacket = BasePacket.createPacket(player.getActor().createInitSubpackets(player.actorID), true, false);
                            client.queuePacket(tpacket);
                            

                            client.queuePacket(reply8);
                            client.queuePacket(reply9);
                            client.queuePacket(reply10);
                           // client.queuePacket(reply11);
                            client.queuePacket(reply12);

                            inn.addActorToZone(player.getActor());

                            break;
                        //Chat Received
                        case 0x0003:
                            subpacket.debugPrintSubPacket();
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
                            List<BasePacket> instanceUpdatePackets = player.updateInstance(inn.getActorsAroundActor(player.getActor(), 50));
                            foreach (BasePacket bp in instanceUpdatePackets)
                                client.queuePacket(bp);

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
                        //Start Script
                        case 0x012D:
                            subpacket.debugPrintSubPacket();
                            CommandStartRequestPacket commandStart = new CommandStartRequestPacket(subpacket.data);
                            
                            client.queuePacket(BasePacket.createPacket(ActorDoEmotePacket.buildPacket(player.actorID, player.getActor().currentTarget, 137), true, false));
                            break;
                        //Script Result
                        case 0x012E:
                            subpacket.debugPrintSubPacket();
                            ScriptResultPacket scriptResult = new ScriptResultPacket(subpacket.data);
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
                            client.queuePacket(BasePacket.createPacket(FaqListResponsePacket.buildPacket(player.actorID, new string[]{"Testing FAQ1", "Coded style!"}), true, false));
                            break;
                        //Request for body of a faq/info selection
                        case 0x01D1:
                            FaqBodyRequestPacket faqBodyRequest = new FaqBodyRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(FaqBodyResponsePacket.buildPacket(player.actorID, "HERE IS A GIANT BODY. Nothing else to say!"), true, false));                            
                            break;
                        //Request issue list
                        case 0x01D2:
                            GMTicketIssuesRequestPacket issuesRequest = new GMTicketIssuesRequestPacket(subpacket.data);
                            client.queuePacket(BasePacket.createPacket(IssueListResponsePacket.buildPacket(player.actorID, new string[] { "Test1", "Test2", "Test3", "Test4", "Test5"}), true, false));                            
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
            }
        }        

        public void sendPacket(string path, int conn)
        {
            BasePacket packet = new BasePacket(path);

            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mPlayers)
            {                
                packet.replaceActorID(entry.Value.actorID);
                if (conn == 1 || conn == 3)
                    entry.Value.getConnection1().queuePacket(packet);
                if (conn == 2 || conn == 3)
                    entry.Value.getConnection2().queuePacket(packet);
            }
        }

        public void processScriptResult(SubPacket subpacket)
        {
            uint someId1 = 0;
	        uint someId2 = 0;
	        uint someId3 = 0;

            using (MemoryStream mem = new MemoryStream(subpacket.data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    binReader.BaseStream.Seek(0x2C, SeekOrigin.Begin);
                    someId1 = binReader.ReadUInt32();
                    someId2 = binReader.ReadUInt32();
                    someId3 = binReader.ReadUInt32();
                }
            }

	        Log.info(String.Format("ProcessScriptResult: Id1 = {0}, Id2 = {1}, Id3 = {2}", someId1, someId2, someId3));

	        
        }

        /*
        public void sendTeleportSequence(ClientConnection client, uint levelId, float x, float y, float z, float angle)
        {
	        BasePacket reply1 = new BasePacket("./packets/move/move1.bin");
            BasePacket reply2 = new BasePacket("./packets/move/move2.bin");
            BasePacket reply3 = new BasePacket("./packets/move/move3.bin");
            BasePacket reply4 = new BasePacket("./packets/move/move4.bin");
            BasePacket reply5 = new BasePacket("./packets/move/move5.bin");
            BasePacket reply6 = new BasePacket("./packets/move/move6.bin");
            BasePacket reply7 = new BasePacket("./packets/move/move7.bin");
            BasePacket reply8 = new BasePacket("./packets/move/move8.bin");
            BasePacket reply9 = new BasePacket("./packets/move/move9.bin");

            client.queuePacket(reply1);
            client.queuePacket(reply2);
            client.queuePacket(reply3);
            client.queuePacket(reply4);
            client.queuePacket(reply5);
            client.queuePacket(reply6);
            client.queuePacket(reply7);
            client.queuePacket(reply8);
            client.queuePacket(reply9);
        
	        
	        {
		        CCompositePacket result;

		        {
			        CSetMusicPacket packet;
			        packet.SetSourceId(PLAYER_ID);
			        packet.SetTargetId(PLAYER_ID);
			        packet.SetMusicId(zone->backgroundMusicId);
			        result.AddPacket(packet.ToPacketData());
		        }

		        {
			        CSetWeatherPacket packet;
			        packet.SetSourceId(PLAYER_ID);
			        packet.SetTargetId(PLAYER_ID);
			        packet.SetWeatherId(CSetWeatherPacket::WEATHER_CLEAR);
			        result.AddPacket(packet.ToPacketData());
		        }

		        {
			        CSetMapPacket packet;
			        packet.SetSourceId(PLAYER_ID);
			        packet.SetTargetId(PLAYER_ID);
			        packet.SetMapId(levelId);
			        result.AddPacket(packet.ToPacketData());
		        }

		        QueuePacket(0, result.ToPacketData());
	        }

	        QueuePacket(0, PacketData(std::begin(g_client0_moor11), std::end(g_client0_moor11)));
	        QueuePacket(0, PacketData(std::begin(g_client0_moor12), std::end(g_client0_moor12)));

	        {
		        PacketData outgoingPacket(std::begin(g_client0_moor13), std::end(g_client0_moor13));

		        {
			        const uint32 setInitialPositionBase = 0x360;

			        CSetInitialPositionPacket setInitialPosition;
			        setInitialPosition.SetSourceId(PLAYER_ID);
			        setInitialPosition.SetTargetId(PLAYER_ID);
			        setInitialPosition.SetX(x);
			        setInitialPosition.SetY(y);
			        setInitialPosition.SetZ(z);
			        setInitialPosition.SetAngle(angle);
			        auto setInitialPositionPacket = setInitialPosition.ToPacketData();

			        memcpy(outgoingPacket.data() + setInitialPositionBase, setInitialPositionPacket.data(), setInitialPositionPacket.size());
		        }

		        QueuePacket(0, outgoingPacket);
	        }

	        QueuePacket(0, GetInventoryInfo());
	        QueuePacket(0, PacketData(std::begin(g_client0_moor21), std::end(g_client0_moor21)));
	        //QueuePacket(0, PacketData(std::begin(g_client0_moor22), std::end(g_client0_moor22)));
	
	        if(!m_zoneMasterCreated)
	        {
		        //Zone Master
		        QueuePacket(0, PacketData(std::begin(g_client0_moor23), std::end(g_client0_moor23)));

	        /*
		        QueuePacket(0, PacketData(std::begin(g_client0_moor24), std::end(g_client0_moor24)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor25), std::end(g_client0_moor25)));

		        QueuePacket(0, PacketData(std::begin(g_client0_moor26), std::end(g_client0_moor26)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor27), std::end(g_client0_moor27)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor28), std::end(g_client0_moor28)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor29), std::end(g_client0_moor29)));

		        QueuePacket(0, PacketData(std::begin(g_client0_moor30), std::end(g_client0_moor30)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor31), std::end(g_client0_moor31)));

		        QueuePacket(0, PacketData(std::begin(g_client0_moor32), std::end(g_client0_moor32)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor33), std::end(g_client0_moor33)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor34), std::end(g_client0_moor34)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor35), std::end(g_client0_moor35)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor36), std::end(g_client0_moor36)));
		        QueuePacket(0, PacketData(std::begin(g_client0_moor37), std::end(g_client0_moor37)));
	        */
		        //Enables chat?
	        //	QueuePacket(0, PacketData(std::begin(g_client0_moor38), std::end(g_client0_moor38)));
        /*
		        {
			        CCompositePacket packet;
			        packet.AddPacket(PacketData(std::begin(g_client0_moor38), std::end(g_client0_moor38)));
			        QueuePacket(0, packet.ToPacketData());
		        }

	        //	QueuePacket(0, PacketData(std::begin(g_client0_moor39), std::end(g_client0_moor39)));

	        //	QueuePacket(0, PacketData(std::begin(g_client0_moor40), std::end(g_client0_moor40)));

		

		        m_zoneMasterCreated = true;
	        }

	        if(zone != nullptr)
	        {
		        for(const auto& actorInfo : zone->actors)
		        {
			        SpawnNpc(actorInfo.id, actorInfo.baseModelId, actorInfo.nameStringId, 
				        std::get<0>(actorInfo.pos), std::get<1>(actorInfo.pos), std::get<2>(actorInfo.pos), 0);
		        }
	        }

	        m_curMap = levelId;
	        m_posX = x;
	        m_posY = y;
	        m_posZ = z;
        }*/
    }
}

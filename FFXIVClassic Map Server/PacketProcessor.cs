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

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
        Dictionary<uint, Player> mPlayers;
        List<ClientConnection> mConnections;
        Boolean isAlive = true;

        public PacketProcessor(Dictionary<uint, Player> playerList, List<ClientConnection> connectionList)
        {
            mPlayers = playerList;
            mConnections = connectionList;
        }

        public void update()
        {
            Console.WriteLine("Packet processing thread has started");
            while (isAlive)
            {
                lock (mConnections)
                {
                    foreach (ClientConnection conn in mConnections)
                    {
                        //Receive conn1 packets
                        while (true)
                        {
                            if (conn == null || conn.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                break;

                            try {
                                if (conn.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                    break;
                                BasePacketHeader header = BasePacket.getHeader(conn.incomingStream.Peek(BasePacket.BASEPACKET_SIZE));

                                if (conn.incomingStream.Size < header.packetSize)
                                    break;

                                BasePacket packet = new BasePacket(conn.incomingStream.Get(header.packetSize));
                                processPacket(conn, packet);

                            }
                            catch(OverflowException)
                            { break; }
                        }
                        
                        //Send packets
                        if (conn != null && conn.sendPacketQueue.Count != 0)
                            conn.flushQueuedSendPackets();
                    }
                }

                //Don't waste CPU if isn't needed
                if (mConnections.Count == 0)
                    Thread.Sleep(2000);
                else
                    Thread.Sleep(100);
            }
        }

        private void processPacket(ClientConnection client, BasePacket packet)
        {
            Player player = null;
            if (client.owner != 0 && mPlayers.ContainsKey(client.owner))
                player = mPlayers[client.owner];
            
            if (packet.header.isEncrypted == 0x01)                       
                BasePacket.decryptPacket(client.blowfish, ref packet);


            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                //Console.WriteLine(client.getAddress());
                switch (subpacket.header.opcode)
                {             
                    //Initial
                    case 0x0000:
                        BasePacket init = InitPacket.buildPacket(0, Utils.UnixTimeStampUTC());
                        BasePacket reply2 = new BasePacket("./packets/login2.bin");                        

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

                            client.sendPacketQueue.Add(init);
                            client.sendPacketQueue.Add(reply2);
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
                                {}
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

                        Log.debug(String.Format("Got actorID {0} for conn {1}.", actorID, client.getAddress()));

                        if (player == null)
                        {
                            player = new Player(actorID);                            
                            mPlayers[actorID] = player;
                            client.owner = actorID;
                            client.connType = 0;
                            player.setConnection1(client);
                        }
                        else
                        {
                            client.owner = actorID;
                            client.connType = 1;
                            player.setConnection2(client);
                        }

                        //Get Character info
                        //Create player actor
                        client.queuePacket(init);
                        client.queuePacket(reply2);
                        break;
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

                        BasePacket setinv = new BasePacket("./packets/login/inventory_backup.bin");
                        BasePacket keyitems = new BasePacket("./packets/login/keyitems.bin");
                        BasePacket currancy = new BasePacket("./packets/login/currancy.bin");

#region replaceid
                        setinv.replaceActorID(player.actorID);
                        currancy.replaceActorID(player.actorID);
                        keyitems.replaceActorID(player.actorID);

                        reply5.replaceActorID(player.actorID);
                        reply6.replaceActorID(player.actorID);
                        reply7.replaceActorID(player.actorID);
                        reply8.replaceActorID(player.actorID);
                        reply9.replaceActorID(player.actorID);
#endregion

                        client.queuePacket(BasePacket.createPacket(SetMapPacket.buildPacket(player.actorID, 0xD1), true, false));
                        client.queuePacket(BasePacket.createPacket(_0x2Packet.buildPacket(player.actorID), true, false));
                        client.queuePacket(reply5);
                        client.queuePacket(reply6);
                        
                        client.queuePacket(BasePacket.createPacket(player.getActor().createNamePacket(player.actorID), true, false));
                        client.queuePacket(BasePacket.createPacket(player.getActor().createAppearancePacket(player.actorID), true, false));

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
                        
                        //client.queuePacket(setinv);
                        //client.queuePacket(currancy);
                        //client.queuePacket(keyitems);

#region equipsetup
                        SetInitialEquipmentPacket initialEqupmentPacket = new SetInitialEquipmentPacket();
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_BODY, 5);
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_HEAD, 3);
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_UNDERSHIRT, 6);
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_UNDERGARMENT, 7);
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_MAINHAND, 2);
                        initialEqupmentPacket.setItem(SetInitialEquipmentPacket.SLOT_LEGS, 8);
#endregion

                        //Equip Init
                        client.queuePacket(BasePacket.createPacket(InventorySetBeginPacket.buildPacket(player.actorID, 0x23, InventorySetBeginPacket.CODE_EQUIPMENT), true, false));
                        client.queuePacket(BasePacket.createPacket(initialEqupmentPacket.buildPackets(player.actorID), true, false));
                        client.queuePacket(BasePacket.createPacket(InventorySetEndPacket.buildPacket(player.actorID), true, false));

                        client.queuePacket(BasePacket.createPacket(InventoryEndChangePacket.buildPacket(player.actorID), true, false));
                        ////////ITEMS////////

                        //The rest of hardcode
                        client.queuePacket(reply7);
                        client.queuePacket(reply8);
                        client.queuePacket(reply9);
                        client.queuePacket(reply10);                        

                        break;
                    //Chat Received
                    case 0x0003:                        
                        subpacket.debugPrintSubPacket();
                        break;
                    //Update Position
                    case 0x00CA:
                        UpdatePlayerPositionPacket posUpdate = new UpdatePlayerPositionPacket(subpacket.data);
                        player.updatePlayerActorPosition(posUpdate.x, posUpdate.y, posUpdate.z, posUpdate.rot, posUpdate.moveState);
                        break;
                    case 0x00CD:
                        subpacket.debugPrintSubPacket();
				        //ProcessSetSelection(subPacket);				
				        break;
                    case 0x012D:
                        subpacket.debugPrintSubPacket();
				        //ProcessScriptCommand(subPacket);				    
				        break;
                    case 0x012E:
                        subpacket.debugPrintSubPacket();
				        processScriptResult(subpacket);				    
				        break;
                    case 0x012F:
                        subpacket.debugPrintSubPacket();
                        BasePacket scriptReply = new BasePacket("./packets/charawork2");
                        BasePacket scriptReply2 = new BasePacket("./packets/charawork3");
                        BasePacket scriptReply3 = new BasePacket("./packets/charawork4");                        
                        client.queuePacket(scriptReply);
                        client.queuePacket(scriptReply2);
                        client.queuePacket(scriptReply3);
                        break;
                    default:
                        Log.debug(String.Format("Unknown command 0x{0:X} received.", subpacket.header.opcode));
                        subpacket.debugPrintSubPacket();
                        break;
                }
            }
        }


        public void sendPacket(string path, int conn)
        {
            if (mPlayers.Count == 0)
                return;

            BasePacket packet = new BasePacket(path);

            foreach (KeyValuePair<uint, Player> entry in mPlayers)
            {
                packet.replaceActorID(entry.Value.actorID);
                if (conn == 1 || conn == 3)
                    entry.Value.getConnection1().sendPacketQueue.Add(packet);
                if (conn == 2 || conn == 3)
                    entry.Value.getConnection2().sendPacketQueue.Add(packet);
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

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

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
        Dictionary<uint, Player> mPlayers = new Dictionary<uint, Player>();
        List<ClientConnection> mConnections;
        Boolean isAlive = true;

        public PacketProcessor(List<ClientConnection> connectionList)
        {
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
                        client.sendPacketQueue.Add(init);
                        client.sendPacketQueue.Add(reply2);
                        break;
                    //Ping
                    case 0x0001:
                        subpacket.debugPrintSubPacket();
                        PingPacket pingPacket = new PingPacket(subpacket.data);
                        client.queuePacket(BasePacket.createPacket(PongPacket.buildPacket(player.actorID, pingPacket.time), true, false));
                        break;
                    //Unknown
                    case 0x0002:
                        subpacket.debugPrintSubPacket();

                        BasePacket setMapPacket = BasePacket.createPacket(SetMapPacket.buildPacket(player.actorID, 0xD1), true, false);
                        BasePacket setPlayerActorPacket = BasePacket.createPacket(_0x2Packet.buildPacket(player.actorID), true, false);
             
                        BasePacket reply5 = new BasePacket("./packets/asd/login5.bin");
                        BasePacket reply6 = new BasePacket("./packets/asd/login6_data.bin");
                        BasePacket reply7 = new BasePacket("./packets/asd/login7_data.bin");
                        BasePacket reply8 = new BasePacket("./packets/asd/login8_data.bin");
                        BasePacket reply9 = new BasePacket("./packets/asd/login9_zonesetup.bin");
                        BasePacket reply10 = new BasePacket("./packets/asd/login10.bin");


                        client.sendPacketQueue.Add(setMapPacket);
                        client.sendPacketQueue.Add(setPlayerActorPacket);
                        client.sendPacketQueue.Add(reply5);
                        client.sendPacketQueue.Add(reply6);
                        client.sendPacketQueue.Add(reply7);
                        client.sendPacketQueue.Add(reply8);
                        client.sendPacketQueue.Add(reply9);
                        //client.sendPacketQueue.Add(reply10);

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
				        //ProcessScriptResult(subPacket);				    
				        break;
                    default:
                        Log.debug(String.Format("Unknown command 0x{0:X} received.", subpacket.header.opcode));
                        subpacket.debugPrintSubPacket();
                        break;
                }
            }
        }        
      
    }
}

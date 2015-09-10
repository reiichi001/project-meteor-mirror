using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
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
                    foreach (ClientConnection client in mConnections)
                    {
                        //Receive packets
                        while (true)
                        {
                            if (client.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                break;

                            try {
                                if (client.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                    break;
                                BasePacketHeader header = BasePacket.getHeader(client.incomingStream.Peek(BasePacket.BASEPACKET_SIZE));

                                if (client.incomingStream.Size < header.packetSize)
                                    break;

                                BasePacket packet = new BasePacket(client.incomingStream.Get(header.packetSize));
                                processPacket(client, packet);

                            }
                            catch(OverflowException)
                            { break; }
                        }

                        //Send packets
                        while (client.sendPacketQueue.Count != 0)
                            client.flushQueuedSendPackets();
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
            
            if ((packet.header.packetSize == 0x288) && (packet.data[0x34] == 'T'))		//Test Ticket Data
            {
                //Crypto handshake
                ProcessStartSession(client, packet);
                return;
            }
            
            BasePacket.decryptPacket(client.blowfish, ref packet);

            packet.debugPrintPacket();

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {                             

                switch (subpacket.header.opcode)
                {
                    case 0x03:
                        ProcessGetCharacters(client, subpacket);
                        break;
                    case 0x04:
                        ProcessSelectCharacter(client, subpacket);
                        break;
                    case 0x05:
                        ProcessSessionAcknowledgement(client, subpacket);
                        break;
                    case 0x0B:
                        ProcessModifyCharacter(client, subpacket);
                        break;  
                    case 0x0F:
                        //Mod Retainers
                    default:
                        Debug.WriteLine("Unknown command 0x{0:X} received.", subpacket.header.opcode);
                        break;
                }
            }
        }

        private void ProcessStartSession(ClientConnection client, BasePacket packet)
        {
            UInt32 clientTime = BitConverter.ToUInt32(packet.data, 0x74);

            //We assume clientTime is 0x50E0E812, but we need to generate a proper key later
            byte[] blowfishKey = { 0xB4, 0xEE, 0x3F, 0x6C, 0x01, 0x6F, 0x5B, 0xD9, 0x71, 0x50, 0x0D, 0xB1, 0x85, 0xA2, 0xAB, 0x43};
            client.blowfish = new Blowfish(blowfishKey);

            Console.WriteLine("Received encryption key: 0x{0:X}", clientTime);

            //Respond with acknowledgment
            BasePacket outgoingPacket = new BasePacket(HardCoded_Packets.g_secureConnectionAcknowledgment);
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessSessionAcknowledgement(ClientConnection client, SubPacket packet)
        {
            PacketStructs.SessionPacket sessionPacket = PacketStructs.toSessionStruct(packet.data);
            String sessionId = sessionPacket.session;
            String clientVersion = sessionPacket.version;

            Console.WriteLine("Got acknowledgment for secure session.");
            Console.WriteLine("SESSION ID: {0}", sessionId);
            Console.WriteLine("CLIENT VERSION: {0}", clientVersion);

            uint userId = Database.getUserIdFromSession(sessionId);
            client.currentUserId = userId;

            if (userId == 0)
            {
                //client.disconnect();
                Console.WriteLine("Invalid session, kicking...");
            }

            Console.WriteLine("USER ID: {0}", userId);
            BasePacket outgoingPacket = new BasePacket("./packets/loginAck.bin");
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessGetCharacters(ClientConnection client, SubPacket packet)
        {   
	        Console.WriteLine("{0} => Get characters", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId);

            sendWorldList(client, packet);
            sendImportList(client, packet);
            sendRetainerList(client, packet);
            sendCharacterList(client, packet);

        }

        private void ProcessSelectCharacter(ClientConnection client, SubPacket packet)
        {
            uint characterId = 0;
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(packet.data)))
            {
                binReader.BaseStream.Seek(0x8, SeekOrigin.Begin);
                characterId = binReader.ReadUInt32();
                binReader.Close();
            }

            Console.WriteLine("{0} => Select character id {1}", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId, characterId);	        

	        String serverIp = "141.117.162.99";
            ushort port = 54992;
            BitConverter.GetBytes(port);
	        BasePacket outgoingPacket = new BasePacket("./packets/selectChar.bin");

            //Write Character ID and Server info
            using (BinaryWriter binWriter = new BinaryWriter(new MemoryStream(outgoingPacket.data)))
            {
                binWriter.Seek(0x28, SeekOrigin.Begin);
                binWriter.Write(characterId);
                binWriter.Seek(0x78, SeekOrigin.Begin);
                binWriter.Write(System.Text.Encoding.ASCII.GetBytes(serverIp));
                binWriter.Seek(0x76, SeekOrigin.Begin);
                binWriter.Write(port);
                binWriter.Close();
            }

            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
	        client.queuePacket(outgoingPacket);
        }

        private void ProcessModifyCharacter(ClientConnection client, SubPacket packet)
        {

            PacketStructs.CharacterRequestPacket charaReq = PacketStructs.toCharacterRequestStruct(packet.data);
            var slot = charaReq.slot;
            var code = charaReq.command;
            var name = charaReq.characterName;
            var worldId = charaReq.worldId;

            uint pid = 0, cid = 0;

            if (worldId == 0)
                worldId = client.newCharaWorldId;

            string worldName = null;           
            World world = Database.getServer(worldId);
            if (world != null)
                worldName = world.name;

            if (worldName == null)
            {
                ErrorPacket errorPacket = new ErrorPacket(charaReq.sequence, 0, 0, 13001, "World does not exist or is inactive.");
                SubPacket subpacket = errorPacket.buildPacket();
                BasePacket basePacket = BasePacket.createPacket(subpacket, true, false);
                BasePacket.encryptPacket(client.blowfish, basePacket);
                client.queuePacket(basePacket);

                Console.WriteLine("User {0} => Error; invalid server id: \"{1}\"", client.currentUserId, worldId);
                return;
            }

            switch (code)
            {
                case 0x01://Reserve
                    
                    var alreadyTaken = Database.reserveCharacter(client.currentUserId, slot, worldId, name, out pid, out cid);

                    if (alreadyTaken)
                    {
                        ErrorPacket errorPacket = new ErrorPacket(charaReq.sequence, 1003, 0, 13005, ""); //BDB - Chara Name Used, //1003 - Bad Word
                        SubPacket subpacket = errorPacket.buildPacket();
                        BasePacket basePacket = BasePacket.createPacket(subpacket, true, false);
                        BasePacket.encryptPacket(client.blowfish, basePacket);
                        client.queuePacket(basePacket);

                        Log.info(String.Format("User {0} => Error; name taken: \"{1}\"", client.currentUserId, charaReq.characterName));
                        return;
                    }
                    else
                    {
                        pid = 0;
                        client.newCharaCid = cid;
                        client.newCharaSlot = slot;
                        client.newCharaWorldId = worldId;
                        client.newCharaName = name;
                    }

                    Log.info(String.Format("User {0} => Character reserved \"{1}\"", client.currentUserId, charaReq.characterName));
                    break;
                case 0x02://Make                    
                    CharaInfo info = new CharaInfo();

                    Database.makeCharacter(client.currentUserId, client.newCharaCid, info);

                    pid = 1;
                    cid = client.newCharaCid;
                    name = client.newCharaName; 

                    Log.info(String.Format("User {0} => Character created \"{1}\"", client.currentUserId, charaReq.characterName));
                    break;
                case 0x03://Rename
                    
                    Log.info(String.Format("User {0} => Character renamed \"{1}\"", client.currentUserId, charaReq.characterName));
                    break;
                case 0x04://Delete
                    Database.deleteCharacter(charaReq.characterId, charaReq.characterName);
                    
                    Log.info(String.Format("User {0} => Character deleted \"{1}\"", client.currentUserId, charaReq.characterName));
                    break;
                case 0x06://Rename Retainer

                    Log.info(String.Format("User {0} => Retainer renamed \"{1}\"", client.currentUserId, charaReq.characterName));
                    break;
            }

            CharaCreatorPacket charaCreator = new CharaCreatorPacket(charaReq.sequence, code, pid, cid, 1, name, worldName);
            BasePacket charaCreatorPacket = BasePacket.createPacket(charaCreator.buildPacket(), true, false);
            charaCreatorPacket.debugPrintPacket();
            BasePacket.encryptPacket(client.blowfish, charaCreatorPacket);
            client.queuePacket(charaCreatorPacket);

        }        

        private void sendWorldList(ClientConnection client, SubPacket packet)
        {            
            List<World> serverList = Database.getServers();
            WorldListPacket worldlistPacket = new WorldListPacket(2, serverList);
            List<SubPacket> subPackets = worldlistPacket.buildPackets();

            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);

        }

        private void sendImportList(ClientConnection client, SubPacket packet)
        {
            List<String> names = Database.getReservedNames(client.currentUserId);

            ImportListPacket importListPacket = new ImportListPacket(2, names);
            List<SubPacket> subPackets = importListPacket.buildPackets();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private void sendRetainerList(ClientConnection client, SubPacket packet)
        {
            List<Retainer> retainers = Database.getRetainers(client.currentUserId);

            RetainerListPacket retainerListPacket = new RetainerListPacket(2, retainers);
            List<SubPacket> subPackets = retainerListPacket.buildPackets();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private void sendCharacterList(ClientConnection client, SubPacket packet)
        {
            List<Character> characterList = Database.getCharacters(client.currentUserId);

            CharacterListPacket characterlistPacket = new CharacterListPacket(2, characterList);
            List<SubPacket> subPackets = characterlistPacket.buildPackets();
            subPackets[0].debugPrintSubPacket();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

    }
}

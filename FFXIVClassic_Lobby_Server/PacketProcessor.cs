using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            //String sessionId = Utils.unsafeAsciiBytesToString(packet.data, 0x30);
            //String clientVersion = Utils.unsafeAsciiBytesToString(packet.data, 0x70);

            Debug.WriteLine("Got acknowledgment for secure session.");
            // Debug.WriteLine("SESSION_ID: {0}", sessionId);
            // Debug.WriteLine("CLIENT_VERSION: {0}", clientVersion);

            //Check if got MYSQL Conn


            //auto query = string_format("SELECT userId FROM ffxiv_sessions WHERE id = '%s' AND expiration > NOW()", sessionId.c_str());

            //Console.WriteLine("UserId {0} logged in.", id);
            BasePacket outgoingPacket = new BasePacket("./packets/loginAck.bin");
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessGetCharacters(ClientConnection client, SubPacket packet)
        {   
	        Console.WriteLine("{0} => Get characters", client.getAddress());
	        BasePacket outgoingPacket = new BasePacket("./packets/getCharsPacket.bin");
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
	        client.queuePacket(outgoingPacket);
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

            Console.WriteLine("{0} => Select character id {1}", client.getAddress(), characterId);	        

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
            packet.debugPrintSubPacket();

            CharacterRequestPacket.CharacterRequest charaReq = CharacterRequestPacket.toStruct(packet.data);
            var slot = charaReq.slot;
            var code = charaReq.command;
            var name = charaReq.characterName;
            var worldId = charaReq.worldId;

            switch (code)
            {
                case 0x01://Reserve
                    //Database.reserveCharacter(0, slot, worldId, name);

                    //Confirm Reserve
                    BasePacket confirmReservePacket = new BasePacket("./packets/chara/confirmReserve.bin");
                    BasePacket.encryptPacket(client.blowfish, confirmReservePacket);
                    client.queuePacket(confirmReservePacket);
                    Console.WriteLine("Reserving character \"{0}\"", charaReq.characterName);
                    break;
                case 0x02://Make                    
                    Character character = Character.EncodedToCharacter(charaReq.characterInfoEncoded);
                    Database.makeCharacter(0, name, character);

                    //Confirm
                    BasePacket confirmMakePacket = new BasePacket("./packets/chara/confirmMake.bin");
                    BasePacket.encryptPacket(client.blowfish, confirmMakePacket);
                    client.queuePacket(confirmMakePacket);
                    Console.WriteLine("Character created!");
                    break;
                case 0x03://Rename
                    break;
                case 0x04://Delete
                    Database.deleteCharacter(charaReq.characterId, charaReq.characterName);

                    //Confirm
                    BasePacket deleteConfirmPacket = new BasePacket("./packets/chara/confirmDelete.bin");
                    BasePacket.encryptPacket(client.blowfish, deleteConfirmPacket);
                    client.queuePacket(deleteConfirmPacket);
                    Console.WriteLine("Character deleted \"{0}\"", charaReq.characterName);
                    break;
                case 0x06://Rename Retainer
                    break;
            }           
        }

    }
}

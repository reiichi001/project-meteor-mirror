using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Lobby_Server.packets.receive;
using FFXIVClassic_Lobby_Server.utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {

        public void processPacket(ClientConnection client, BasePacket packet)
        {

            if ((packet.header.packetSize == 0x288) && (packet.data[0x34] == 'T'))		//Test Ticket Data
            {
                packet.debugPrintPacket();
                //Crypto handshake
                ProcessStartSession(client, packet);
                return;
            }

            BasePacket.decryptPacket(client.blowfish, ref packet);

            packet.debugPrintPacket();

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                subpacket.debugPrintSubPacket();

                if (subpacket.header.type == 3)
                {
                    switch (subpacket.gameMessage.opcode)
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
                            Log.debug(String.Format("Unknown command 0x{0:X} received.", subpacket.gameMessage.opcode));
                            break;
                    }
                }
            }
        }

        private void ProcessStartSession(ClientConnection client, BasePacket packet)
        {
            SecurityHandshakePacket securityHandshake = new SecurityHandshakePacket(packet.data);

            byte[] blowfishKey = GenerateKey(securityHandshake.ticketPhrase, securityHandshake.clientNumber);
            client.blowfish = new Blowfish(blowfishKey);

            Log.info(String.Format("SecCNum: 0x{0:X}", securityHandshake.clientNumber));

            //Respond with acknowledgment
            BasePacket outgoingPacket = new BasePacket(HardCoded_Packets.g_secureConnectionAcknowledgment);
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessSessionAcknowledgement(ClientConnection client, SubPacket packet)
        {
            packet.debugPrintSubPacket();
            SessionPacket sessionPacket = new SessionPacket(packet.data);
            String clientVersion = sessionPacket.version;

            Log.info(String.Format("Got acknowledgment for secure session."));         
            Log.info(String.Format("CLIENT VERSION: {0}", clientVersion));

            uint userId = Database.getUserIdFromSession(sessionPacket.session);
            client.currentUserId = userId;
            client.currentSessionToken = sessionPacket.session; ;

            if (userId == 0)
            {
                    ErrorPacket errorPacket = new ErrorPacket(sessionPacket.sequence, 0, 0, 13001, "Your session has expired, please login again.");
                    SubPacket subpacket = errorPacket.buildPacket();
                    BasePacket errorBasePacket = BasePacket.createPacket(subpacket, true, false);
                    BasePacket.encryptPacket(client.blowfish, errorBasePacket);
                    client.queuePacket(errorBasePacket);

                    Log.info(String.Format("Invalid session, kicking..."));
                    return;
            }

            Log.info(String.Format("USER ID: {0}", userId));

            List<Account> accountList = new List<Account>();
            Account defaultAccount = new Account();
            defaultAccount.id = 1;
            defaultAccount.name = "FINAL FANTASY XIV";
            accountList.Add(defaultAccount);
            AccountListPacket listPacket = new AccountListPacket(1, accountList);
            BasePacket basePacket = BasePacket.createPacket(listPacket.buildPackets(), true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private void ProcessGetCharacters(ClientConnection client, SubPacket packet)
        {   
	        Log.info(String.Format("{0} => Get characters", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId));

            sendWorldList(client, packet);
            sendImportList(client, packet);
            sendRetainerList(client, packet);
            sendCharacterList(client, packet);                        

        }

        private void ProcessSelectCharacter(ClientConnection client, SubPacket packet)
        {
            SelectCharacterPacket selectCharRequest = new SelectCharacterPacket(packet.data);

            Log.info(String.Format("{0} => Select character id {1}", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId, selectCharRequest.characterId));

            Character chara = Database.getCharacter(client.currentUserId, selectCharRequest.characterId);
            World world = null;

            if (chara != null)
                world = Database.getServer(chara.serverId);

            if (world == null)
            {
                ErrorPacket errorPacket = new ErrorPacket(selectCharRequest.sequence, 0, 0, 13001, "World does not exist or is inactive.");
                SubPacket subpacket = errorPacket.buildPacket();
                BasePacket basePacket = BasePacket.createPacket(subpacket, true, false);
                BasePacket.encryptPacket(client.blowfish, basePacket);
                client.queuePacket(basePacket);
                return;
            }

            SelectCharacterConfirmPacket connectCharacter = new SelectCharacterConfirmPacket(selectCharRequest.sequence, selectCharRequest.characterId, client.currentSessionToken, world.address, world.port, selectCharRequest.ticket);

            BasePacket outgoingPacket = BasePacket.createPacket(connectCharacter.buildPackets(), true, false);
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
	        client.queuePacket(outgoingPacket);
        }

        private void ProcessModifyCharacter(ClientConnection client, SubPacket packet)
        {
            CharacterModifyPacket charaReq = new CharacterModifyPacket(packet.data);
            var slot = charaReq.slot;
            var name = charaReq.characterName;
            var worldId = charaReq.worldId;

            uint pid = 0, cid = 0;

            //Get world from new char instance
            if (worldId == 0)
                worldId = client.newCharaWorldId;

            //Check if this character exists, get world from there
            if (worldId == 0 && charaReq.characterId != 0)
            {
                Character chara = Database.getCharacter(client.currentUserId, charaReq.characterId);
                if (chara != null)
                    worldId = chara.serverId;                
            }

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

                Log.info(String.Format("User {0} => Error; invalid server id: \"{1}\"", client.currentUserId, worldId));
                return;
            }

            bool alreadyTaken;

            switch (charaReq.command)
            {
                case 0x01://Reserve
                    
                    alreadyTaken = Database.reserveCharacter(client.currentUserId, slot, worldId, name, out pid, out cid);

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

                    Log.info(String.Format("User {0} => Character reserved \"{1}\"", client.currentUserId, name));
                    break;
                case 0x02://Make                    
                    CharaInfo info = CharaInfo.getFromNewCharRequest(charaReq.characterInfoEncoded);

                    //Set Initial Appearance (items will be loaded in by map server)                    
                    uint[] classAppearance = CharacterCreatorUtils.GetEquipmentForClass(info.currentClass);
                    info.weapon1 = classAppearance[0];
                    info.weapon2 = classAppearance[1];
                    info.head = classAppearance[7];

                    if (classAppearance[8] != 0)
                        info.body = classAppearance[8];
                    else
                        info.body = CharacterCreatorUtils.GetUndershirtForTribe(info.tribe);

                    info.legs = classAppearance[9];
                    info.hands = classAppearance[10];
                    info.feet = classAppearance[11];
                    info.belt = classAppearance[12];
                    
                    //Set Initial Position
                    switch (info.initialTown)
                    {
                        case 1: //ocn0Battle02 (Limsa)
                            info.zoneId = 193;
                            info.x = 0.016f;
                            info.y = 10.35f;
                            info.z = -36.91f;
                            info.rot = 0.025f;  
                            break;
                        case 2: //fst0Battle03 (Gridania)
                            info.zoneId = 166;
                            info.x = 369.5434f;
                            info.y = 4.21f;
                            info.z = -706.1074f;
                            info.rot = -1.26721f;
                            break;
                        case 3: //wil0Battle01 (Ul'dah)
                            info.zoneId = 184;
                            info.x = 5.364327f;
                            info.y = 196.0f;
                            info.z = 133.6561f;
                            info.rot = -2.849384f;
                            break;
                    }

                    Database.makeCharacter(client.currentUserId, client.newCharaCid, info);

                    pid = 1;
                    cid = client.newCharaCid;
                    name = client.newCharaName;

                    Log.info(String.Format("User {0} => Character created \"{1}\"", client.currentUserId, name));
                    break;
                case 0x03://Rename

                    alreadyTaken = Database.renameCharacter(client.currentUserId, charaReq.characterId, worldId, charaReq.characterName);

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

                    Log.info(String.Format("User {0} => Character renamed \"{1}\"", client.currentUserId, name));
                    break;
                case 0x04://Delete
                    Database.deleteCharacter(charaReq.characterId, charaReq.characterName);

                    Log.info(String.Format("User {0} => Character deleted \"{1}\"", client.currentUserId, name));
                    break;
                case 0x06://Rename Retainer

                    Log.info(String.Format("User {0} => Retainer renamed \"{1}\"", client.currentUserId, name));
                    break;
            }

            CharaCreatorPacket charaCreator = new CharaCreatorPacket(charaReq.sequence, charaReq.command, pid, cid, 1, name, worldName);
            BasePacket charaCreatorPacket = BasePacket.createPacket(charaCreator.buildPacket(), true, false);            
            BasePacket.encryptPacket(client.blowfish, charaCreatorPacket);
            client.queuePacket(charaCreatorPacket);

        }        

        private void sendWorldList(ClientConnection client, SubPacket packet)
        {            
            List<World> serverList = Database.getServers();
            WorldListPacket worldlistPacket = new WorldListPacket(0, serverList);
            List<SubPacket> subPackets = worldlistPacket.buildPackets();

            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);

        }

        private void sendImportList(ClientConnection client, SubPacket packet)
        {
            List<String> names = Database.getReservedNames(client.currentUserId);

            ImportListPacket importListPacket = new ImportListPacket(0, names);
            List<SubPacket> subPackets = importListPacket.buildPackets();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private void sendRetainerList(ClientConnection client, SubPacket packet)
        {
            List<Retainer> retainers = Database.getRetainers(client.currentUserId);

            RetainerListPacket retainerListPacket = new RetainerListPacket(0, retainers);
            List<SubPacket> subPackets = retainerListPacket.buildPackets();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private void sendCharacterList(ClientConnection client, SubPacket packet)
        {
            List<Character> characterList = Database.getCharacters(client.currentUserId);

            if (characterList.Count > 8)
                Log.error("Warning, got more than 8 characters. List truncated, check DB for issues.");

            CharacterListPacket characterlistPacket = new CharacterListPacket(0, characterList);
            List<SubPacket> subPackets = characterlistPacket.buildPackets();
            BasePacket basePacket = BasePacket.createPacket(subPackets, true, false);
            BasePacket.encryptPacket(client.blowfish, basePacket);
            client.queuePacket(basePacket);
        }

        private byte[] GenerateKey(string ticketPhrase, uint clientNumber)
        {
            byte[] key;
            using (MemoryStream memStream = new MemoryStream(0x2C))
            {
                using (BinaryWriter binWriter = new BinaryWriter(memStream))
                {
                    binWriter.Write((Byte)0x78);
                    binWriter.Write((Byte)0x56);
                    binWriter.Write((Byte)0x34);
                    binWriter.Write((Byte)0x12);
                    binWriter.Write((UInt32)clientNumber);
                    binWriter.Write((Byte)0xE8);
                    binWriter.Write((Byte)0x03);
                    binWriter.Write((Byte)0x00);
                    binWriter.Write((Byte)0x00);
                    binWriter.Write(Encoding.ASCII.GetBytes(ticketPhrase), 0, Encoding.ASCII.GetByteCount(ticketPhrase) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(ticketPhrase));                    
                }
                byte[] nonMD5edKey = memStream.GetBuffer();

                using (MD5 md5Hash = MD5.Create())
                {
                    key = md5Hash.ComputeHash(nonMD5edKey);
                }
            }
            return key;
        }
    }
}

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Meteor.Common;
using Meteor.Lobby.DataObjects;
using Meteor.Lobby.Packets;
using Meteor.Lobby.Packets.Receive;

namespace Meteor.Lobby
{
    class PacketProcessor
    {

        public void ProcessPacket(ClientConnection client, BasePacket packet)
        {

            if ((packet.header.packetSize == 0x288) && (packet.data[0x34] == 'T'))		//Test Ticket Data
            {
                packet.DebugPrintPacket();
                //Crypto handshake
                ProcessStartSession(client, packet);
                return;
            }

            BasePacket.DecryptPacket(client.blowfish, ref packet);

            packet.DebugPrintPacket();

            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                subpacket.DebugPrintSubPacket();

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
                            Program.Log.Debug("Unknown command 0x{0:X} received.", subpacket.gameMessage.opcode);
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

            Program.Log.Info("SecCNum: 0x{0:X}", securityHandshake.clientNumber);

            //Respond with acknowledgment
            BasePacket outgoingPacket = new BasePacket(HardCoded_Packets.g_secureConnectionAcknowledgment);
            BasePacket.EncryptPacket(client.blowfish, outgoingPacket);
            client.QueuePacket(outgoingPacket);
        }

        private void ProcessSessionAcknowledgement(ClientConnection client, SubPacket packet)
        {
            packet.DebugPrintSubPacket();
            SessionPacket sessionPacket = new SessionPacket(packet.data);
            String clientVersion = sessionPacket.version;

            Program.Log.Info("Got acknowledgment for secure session.");         
            Program.Log.Info("CLIENT VERSION: {0}", clientVersion);

            uint userId = Database.GetUserIdFromSession(sessionPacket.session);
            client.currentUserId = userId;
            client.currentSessionToken = sessionPacket.session; ;

            if (userId == 0)
            {
                ErrorPacket errorPacket = new ErrorPacket(sessionPacket.sequence, 0, 0, 13001, "Your session has expired, please login again.");
                SubPacket subpacket = errorPacket.BuildPacket();
                subpacket.SetTargetId(0xe0006868);
                BasePacket errorBasePacket = BasePacket.CreatePacket(subpacket, true, false);
                BasePacket.EncryptPacket(client.blowfish, errorBasePacket);
                client.QueuePacket(errorBasePacket);

                Program.Log.Info("Invalid session, kicking...");
                return;
            }

            Program.Log.Info("USER ID: {0}", userId);

            List<Account> accountList = new List<Account>();
            Account defaultAccount = new Account();
            defaultAccount.id = 1;
            defaultAccount.name = "FINAL FANTASY XIV";
            accountList.Add(defaultAccount);
            AccountListPacket listPacket = new AccountListPacket(1, accountList);
            BasePacket basePacket = BasePacket.CreatePacket(listPacket.BuildPackets(), true, false);
            BasePacket.EncryptPacket(client.blowfish, basePacket);
            client.QueuePacket(basePacket);
        }

        private void ProcessGetCharacters(ClientConnection client, SubPacket packet)
        {   
	        Program.Log.Info("{0} => Get characters", client.currentUserId == 0 ? client.GetAddress() : "User " + client.currentUserId);

            SendWorldList(client, packet);
            SendImportList(client, packet);
            SendRetainerList(client, packet);
            SendCharacterList(client, packet);                        

        }

        private void ProcessSelectCharacter(ClientConnection client, SubPacket packet)
        {
            SelectCharacterPacket selectCharRequest = new SelectCharacterPacket(packet.data);

            Program.Log.Info("{0} => Select character id {1}", client.currentUserId == 0 ? client.GetAddress() : "User " + client.currentUserId, selectCharRequest.characterId);

            Character chara = Database.GetCharacter(client.currentUserId, selectCharRequest.characterId);
            World world = null;

            if (chara != null)
                world = Database.GetServer(chara.serverId);

            if (world == null)
            {
                ErrorPacket errorPacket = new ErrorPacket(selectCharRequest.sequence, 0, 0, 13001, "World Does not exist or is inactive.");
                SubPacket subpacket = errorPacket.BuildPacket();
                BasePacket basePacket = BasePacket.CreatePacket(subpacket, true, false);
                BasePacket.EncryptPacket(client.blowfish, basePacket);
                client.QueuePacket(basePacket);
                return;
            }

            SelectCharacterConfirmPacket connectCharacter = new SelectCharacterConfirmPacket(selectCharRequest.sequence, selectCharRequest.characterId, client.currentSessionToken, world.address, world.port, selectCharRequest.ticket);

            BasePacket outgoingPacket = BasePacket.CreatePacket(connectCharacter.BuildPackets(), true, false);
            BasePacket.EncryptPacket(client.blowfish, outgoingPacket);
	        client.QueuePacket(outgoingPacket);
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

            //Check if this character exists, Get world from there
            if (worldId == 0 && charaReq.characterId != 0)
            {
                Character chara = Database.GetCharacter(client.currentUserId, charaReq.characterId);
                if (chara != null)
                    worldId = chara.serverId;                
            }

            string worldName = null;           
            World world = Database.GetServer(worldId);
            if (world != null)
                worldName = world.name;

            if (worldName == null)
            {
                ErrorPacket errorPacket = new ErrorPacket(charaReq.sequence, 0, 0, 13001, "World Does not exist or is inactive.");
                SubPacket subpacket = errorPacket.BuildPacket();
                BasePacket basePacket = BasePacket.CreatePacket(subpacket, true, false);
                BasePacket.EncryptPacket(client.blowfish, basePacket);
                client.QueuePacket(basePacket);

                Program.Log.Info("User {0} => Error; invalid server id: \"{1}\"", client.currentUserId, worldId);
                return;
            }

            bool alreadyTaken;

            switch (charaReq.command)
            {
                case 0x01://Reserve
                    
                    alreadyTaken = Database.ReserveCharacter(client.currentUserId, slot, worldId, name, out pid, out cid);

                    if (alreadyTaken)
                    {
                        ErrorPacket errorPacket = new ErrorPacket(charaReq.sequence, 1003, 0, 13005, ""); //BDB - Chara Name Used, //1003 - Bad Word
                        SubPacket subpacket = errorPacket.BuildPacket();
                        BasePacket basePacket = BasePacket.CreatePacket(subpacket, true, false);
                        BasePacket.EncryptPacket(client.blowfish, basePacket);
                        client.QueuePacket(basePacket);

                        Program.Log.Info("User {0} => Error; name taken: \"{1}\"", client.currentUserId, charaReq.characterName);
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

                    Program.Log.Info("User {0} => Character reserved \"{1}\"", client.currentUserId, name);
                    break;
                case 0x02://Make                    
                    CharaInfo info = CharaInfo.GetFromNewCharRequest(charaReq.characterInfoEncoded);

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

                    Database.MakeCharacter(client.currentUserId, client.newCharaCid, info);

                    pid = 1;
                    cid = client.newCharaCid;
                    name = client.newCharaName;

                    Program.Log.Info("User {0} => Character Created \"{1}\"", client.currentUserId, name);
                    break;
                case 0x03://Rename

                    alreadyTaken = Database.RenameCharacter(client.currentUserId, charaReq.characterId, worldId, charaReq.characterName);

                    if (alreadyTaken)
                    {
                        ErrorPacket errorPacket = new ErrorPacket(charaReq.sequence, 1003, 0, 13005, ""); //BDB - Chara Name Used, //1003 - Bad Word
                        SubPacket subpacket = errorPacket.BuildPacket();
                        BasePacket basePacket = BasePacket.CreatePacket(subpacket, true, false);
                        BasePacket.EncryptPacket(client.blowfish, basePacket);
                        client.QueuePacket(basePacket);

                        Program.Log.Info("User {0} => Error; name taken: \"{1}\"", client.currentUserId, charaReq.characterName);
                        return;
                    }

                    Program.Log.Info("User {0} => Character renamed \"{1}\"", client.currentUserId, name);
                    break;
                case 0x04://Delete
                    Database.DeleteCharacter(charaReq.characterId, charaReq.characterName);

                    Program.Log.Info("User {0} => Character deleted \"{1}\"", client.currentUserId, name);
                    break;
                case 0x06://Rename Retainer

                    Program.Log.Info("User {0} => Retainer renamed \"{1}\"", client.currentUserId, name);
                    break;
            }

            CharaCreatorPacket charaCreator = new CharaCreatorPacket(charaReq.sequence, charaReq.command, pid, cid, 1, name, worldName);
            BasePacket charaCreatorPacket = BasePacket.CreatePacket(charaCreator.BuildPacket(), true, false);            
            BasePacket.EncryptPacket(client.blowfish, charaCreatorPacket);
            client.QueuePacket(charaCreatorPacket);

        }        

        private void SendWorldList(ClientConnection client, SubPacket packet)
        {            
            List<World> serverList = Database.GetServers();
            WorldListPacket worldlistPacket = new WorldListPacket(0, serverList);
            List<SubPacket> subPackets = worldlistPacket.BuildPackets();

            BasePacket basePacket = BasePacket.CreatePacket(subPackets, true, false);
            BasePacket.EncryptPacket(client.blowfish, basePacket);
            client.QueuePacket(basePacket);

        }

        private void SendImportList(ClientConnection client, SubPacket packet)
        {
            List<String> names = Database.GetReservedNames(client.currentUserId);

            ImportListPacket importListPacket = new ImportListPacket(0, names);
            List<SubPacket> subPackets = importListPacket.BuildPackets();
            BasePacket basePacket = BasePacket.CreatePacket(subPackets, true, false);
            BasePacket.EncryptPacket(client.blowfish, basePacket);
            client.QueuePacket(basePacket);
        }

        private void SendRetainerList(ClientConnection client, SubPacket packet)
        {
            List<Retainer> retainers = Database.GetRetainers(client.currentUserId);

            RetainerListPacket retainerListPacket = new RetainerListPacket(0, retainers);
            List<SubPacket> subPackets = retainerListPacket.BuildPackets();
            BasePacket basePacket = BasePacket.CreatePacket(subPackets, true, false);
            BasePacket.EncryptPacket(client.blowfish, basePacket);
            client.QueuePacket(basePacket);
        }

        private void SendCharacterList(ClientConnection client, SubPacket packet)
        {
            List<Character> characterList = Database.GetCharacters(client.currentUserId);

            if (characterList.Count > 8)
                Program.Log.Error("Warning, got more than 8 characters. List truncated, check DB for issues.");

            CharacterListPacket characterlistPacket = new CharacterListPacket(0, characterList);
            List<SubPacket> subPackets = characterlistPacket.BuildPackets();
            BasePacket basePacket = BasePacket.CreatePacket(subPackets, true, false);
            BasePacket.EncryptPacket(client.blowfish, basePacket);
            client.QueuePacket(basePacket);
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

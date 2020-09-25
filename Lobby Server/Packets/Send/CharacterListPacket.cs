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
using System.Text;
using Meteor.Common;
using Meteor.Lobby.DataObjects;

namespace Meteor.Lobby.Packets
{
    class CharacterListPacket
    {
        public const ushort OPCODE = 0x0D;
        public const ushort MAXPERPACKET = 2;

        private ulong sequence;
        private List<Character> characterList;

        public CharacterListPacket(ulong sequence, List<Character> characterList)
        {
            this.sequence = sequence;
            this.characterList = characterList;
        }        

        public List<SubPacket> BuildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int numCharacters = characterList.Count >= 8 ? 8 : characterList.Count + 1;

            int characterCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (Character chara in characterList)
            {
                Appearance appearance = Database.GetAppearance(chara.id);

                if (totalCount == 0 || characterCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x3B0);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(numCharacters - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write(numCharacters - totalCount <= MAXPERPACKET ? (UInt32)(numCharacters - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                binWriter.Seek(0x10 + (0x1D0 * characterCount), SeekOrigin.Begin);

                //Write Entries
                World world = Database.GetServer(chara.serverId);
                string worldname = world == null ? "Unknown" : world.name;

                binWriter.Write((uint)0); //???
                binWriter.Write((uint)chara.id); //Character Id            
                binWriter.Write((byte)(totalCount)); //Slot

                byte options = 0;
                if (chara.state == 1)
                    options |= 0x01;
                if (chara.doRename)
                    options |= 0x02;
                if (chara.isLegacy)
                    options |= 0x08;
                
                binWriter.Write((byte)options); //Options (0x01: Service Account not active, 0x72: Change Chara Name) 
                binWriter.Write((ushort)0);  
                binWriter.Write((uint)chara.currentZoneId); //Logged out zone
                binWriter.Write(Encoding.ASCII.GetBytes(chara.name.PadRight(0x20, '\0'))); //Name
                binWriter.Write(Encoding.ASCII.GetBytes(worldname.PadRight(0xE, '\0'))); //World Name

                binWriter.Write(CharaInfo.BuildForCharaList(chara, appearance)); //Appearance Data
                //binWriter.Write(CharaInfo.Debug()); //Appearance Data
                
                characterCount++;
                totalCount++;                

                //Send this chunk of character list
                if (characterCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
                    subpacket.SetTargetId(0xe0006868);
                    subPackets.Add(subpacket);
                    characterCount = 0;
                }                

                //Incase DB came back with more than max
                if (totalCount >= 8)
                    break;
            }

            //Add a 'NEW' slot if there is space
            if (characterList.Count < 8)
            {
                if (characterCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x3B0);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(numCharacters - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write(numCharacters - totalCount <= MAXPERPACKET ? (UInt32)(numCharacters-totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                binWriter.Seek(0x10 + (0x1D0 * characterCount), SeekOrigin.Begin);

                //Write Entries
                binWriter.Write((uint)0); //???
                binWriter.Write((uint)0); //Character Id            
                binWriter.Write((byte)(totalCount)); //Slot
                binWriter.Write((byte)0); //Options (0x01: Service Account not active, 0x72: Change Chara Name) 
                binWriter.Write((ushort)0);
                binWriter.Write((uint)0); //Logged out zone

                characterCount++;
                totalCount++;

                //Send this chunk of character list
                if (characterCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
                    subpacket.SetTargetId(0xe0006868);
                    subPackets.Add(subpacket);
                    characterCount = 0;
                }
            }

            //If there is anything left that was missed or the list is empty
            if (characterCount > 0 || numCharacters == 0)
            {                
                byte[] data = memStream.GetBuffer();
                binWriter.Dispose();
                memStream.Dispose();
                SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
                subpacket.SetTargetId(0xe0006868);
                subPackets.Add(subpacket);
            }

            return subPackets;
        }
    }
}

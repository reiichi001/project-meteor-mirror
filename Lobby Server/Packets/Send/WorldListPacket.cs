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
    class WorldListPacket
    {
        public const ushort OPCODE = 0x15;
        public const ushort MAXPERPACKET = 6;

        private UInt64 sequence;
        private List<World> worldList;

        public WorldListPacket(UInt64 sequence, List<World> serverList)
        {
            this.sequence = sequence;
            this.worldList = serverList;
        }        

        public List<SubPacket> BuildPackets()
        {
            List<SubPacket> subPackets = new List<SubPacket>();

            int serverCount = 0;
            int totalCount = 0;

            MemoryStream memStream = null;
            BinaryWriter binWriter = null;

            foreach (World world in worldList)
            {
                if (totalCount == 0 || serverCount % MAXPERPACKET == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(worldList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write(worldList.Count - totalCount <= MAXPERPACKET ? (UInt32)(worldList.Count - totalCount) : (UInt32)MAXPERPACKET);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

                //Write Entries
                binWriter.Write((ushort)world.id);
                binWriter.Write((ushort)world.listPosition);
                binWriter.Write((uint)world.population);
                binWriter.Write((UInt64)0);
                binWriter.Write(Encoding.ASCII.GetBytes(world.name.PadRight(64, '\0')));

                serverCount++;
                totalCount++;

                //Send this chunk of world list
                if (serverCount >= MAXPERPACKET)
                {
                    byte[] data = memStream.GetBuffer();
                    binWriter.Dispose();
                    memStream.Dispose();
                    SubPacket subpacket = new SubPacket(OPCODE, 0xe0006868, data);
                    subpacket.SetTargetId(0xe0006868);
                    subPackets.Add(subpacket);
                    serverCount = 0;
                }

            }

            //If there is anything left that was missed or the list is empty
            if (serverCount > 0 || worldList.Count == 0)
            {
                if (worldList.Count == 0)
                {
                    memStream = new MemoryStream(0x210);
                    binWriter = new BinaryWriter(memStream);

                    //Write Empty List Info
                    binWriter.Write((UInt64)sequence);
                    byte listTracker = (byte)((MAXPERPACKET * 2) * subPackets.Count);
                    binWriter.Write(worldList.Count - totalCount <= MAXPERPACKET ? (byte)(listTracker + 1) : (byte)(listTracker));
                    binWriter.Write((UInt32)0);
                    binWriter.Write((byte)0);
                    binWriter.Write((UInt16)0);
                }

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

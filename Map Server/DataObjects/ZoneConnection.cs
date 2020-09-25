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
using System.Net.Sockets;

using Meteor.Common;
using System.Collections.Concurrent;
using System.Net;
using Meteor.Map.packets.WorldPackets.Send;

namespace Meteor.Map.dataobjects
{
    class ZoneConnection
    {
        //Connection stuff
        public Socket socket;
        public byte[] buffer;
        private BlockingCollection<SubPacket> SendPacketQueue = new BlockingCollection<SubPacket>(1000);
        public int lastPartialSize = 0;

        public void QueuePacket(SubPacket subpacket)
        {
            if (SendPacketQueue.Count == SendPacketQueue.BoundedCapacity - 1)
                FlushQueuedSendPackets();

            SendPacketQueue.Add(subpacket);
        }

        public void FlushQueuedSendPackets()
        {
            if (socket == null || !socket.Connected)
                return;

            while (SendPacketQueue.Count > 0)
            {
                SubPacket packet = SendPacketQueue.Take();

                byte[] packetBytes = packet.GetBytes();

                try
                {
                    socket.Send(packetBytes);
                }
                catch (Exception e)
                { Program.Log.Error(e, "Weird case, socket was d/ced: {0}"); }
            }
        }

        public String GetAddress()
        {
            return String.Format("{0}:{1}", (socket.RemoteEndPoint as IPEndPoint).Address, (socket.RemoteEndPoint as IPEndPoint).Port);
        }

        public bool IsConnected()
        {
            return (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }

        public void Disconnect()
        {
            if (socket.Connected)
                socket.Disconnect(false);
        }

        public void RequestZoneChange(uint sessionId, uint destinationZoneId, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            WorldRequestZoneChangePacket.BuildPacket(sessionId, destinationZoneId, spawnType, spawnX, spawnY, spawnZ, spawnRotation).DebugPrintSubPacket();
            QueuePacket(WorldRequestZoneChangePacket.BuildPacket(sessionId, destinationZoneId, spawnType, spawnX, spawnY, spawnZ, spawnRotation));
        }
    }
}

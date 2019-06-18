/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net;
using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;

namespace FFXIVClassic_World_Server
{
    class ClientConnection
    {
        //Connection stuff
        public Socket socket;
        public byte[] buffer;
        private BlockingCollection<BasePacket> SendPacketQueue = new BlockingCollection<BasePacket>(1000);
        public int lastPartialSize = 0;

        //Instance Stuff
        public Session owner;

        public void QueuePacket(BasePacket packet)
        {
            SendPacketQueue.Add(packet);
        }

        public void QueuePacket(SubPacket subpacket)
        {
            bool isAuthed = true;
            bool isEncrypted = false;
            subpacket.SetTargetId(owner.sessionId);
            SendPacketQueue.Add(BasePacket.CreatePacket(subpacket, isAuthed, isEncrypted));
        }

        public void FlushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (SendPacketQueue.Count > 0)
            {
                BasePacket packet = SendPacketQueue.Take();
                
                byte[] packetBytes = packet.GetPacketBytes();

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
    }
}

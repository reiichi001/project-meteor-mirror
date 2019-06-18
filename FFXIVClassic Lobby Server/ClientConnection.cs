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
using FFXIVClassic.Common;
using System.Collections.Concurrent;
using Cyotek.Collections.Generic;
using System.Net;

namespace FFXIVClassic_Lobby_Server
{
    class ClientConnection
    {
        //Connection stuff
        public Blowfish blowfish;
        public Socket socket;
        public byte[] buffer = new byte[0xffff];
        public CircularBuffer<byte> incomingStream = new CircularBuffer<byte>(1024);
        public BlockingCollection<BasePacket> SendPacketQueue = new BlockingCollection<BasePacket>(100);
        public int lastPartialSize = 0;

        //Instance Stuff
        public uint currentUserId = 0;
        public uint currentAccount;
        public string currentSessionToken;

        //Chara Creation
        public string newCharaName;
        public uint newCharaPid;
        public uint newCharaCid;
        public ushort newCharaSlot;
        public ushort newCharaWorldId;
        

        public void ProcessIncoming(int bytesIn)
        {
            if (bytesIn == 0)
                return;

            incomingStream.Put(buffer, 0, bytesIn);
        }

        public void QueuePacket(BasePacket packet)
        {
            SendPacketQueue.Add(packet);
        }

        public void FlushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (SendPacketQueue.Count > 0)
            {
                BasePacket packet = SendPacketQueue.Take();
                byte[] packetBytes = packet.GetPacketBytes();
                byte[] buffer = new byte[0xffff];
                Array.Copy(packetBytes, buffer, packetBytes.Length);
                try { 
                    socket.Send(packetBytes);
                }
                catch(Exception e)
                { Program.Log.Error(e, "Weird case, socket was d/ced: {0}"); }
            }
        }

        public String GetAddress()
        {
            return String.Format("{0}:{1}", (socket.RemoteEndPoint as IPEndPoint).Address, (socket.RemoteEndPoint as IPEndPoint).Port);
        }        

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
        }
    }
}

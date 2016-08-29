using System;
using System.Net.Sockets;

using FFXIVClassic.Common;
using System.Collections.Concurrent;
using System.Net;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server
{
    class ZoneConnection
    {
        //Connection stuff
        public Socket socket;
        public byte[] buffer;
        private BlockingCollection<SubPacket> SendPacketQueue = new BlockingCollection<SubPacket>(1000);
        public int lastPartialSize = 0;

        public void QueuePacket(BasePacket packet)
        {
            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket s in subPackets)
                SendPacketQueue.Add(s);
        }

        public void QueuePacket(SubPacket subpacket, bool isAuthed, bool isEncrypted)
        {
            SendPacketQueue.Add(subpacket);
        }

        public void FlushQueuedSendPackets()
        {
            if (!socket.Connected)
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
                { Program.Log.Error("Weird case, socket was d/ced: {0}", e); }
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

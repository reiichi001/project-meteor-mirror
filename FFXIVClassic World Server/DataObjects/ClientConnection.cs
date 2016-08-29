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

        public void QueuePacket(SubPacket subpacket, bool isAuthed, bool isEncrypted)
        {
            SendPacketQueue.Add(BasePacket.CreatePacket(subpacket, isAuthed, isEncrypted));
        }

        public void FlushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (SendPacketQueue.Count > 0)
            {
                BasePacket packet = SendPacketQueue.Take();

                packet.DebugPrintPacket();

                byte[] packetBytes = packet.GetPacketBytes();

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

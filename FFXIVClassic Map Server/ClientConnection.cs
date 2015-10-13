using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using FFXIVClassic_Lobby_Server.packets;
using System.Diagnostics;
using FFXIVClassic_Lobby_Server.common;
using System.Collections.Concurrent;
using System.IO;
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
        private BlockingCollection<BasePacket> sendPacketQueue = new BlockingCollection<BasePacket>(100);

        //Instance Stuff
        public uint owner = 0;
        public uint connType = 0;


        public void processIncoming(int bytesIn)
        {
            if (bytesIn == 0)
                return;

            incomingStream.Put(buffer, 0, bytesIn);
        }

        public void queuePacket(BasePacket packet)
        {
            sendPacketQueue.Add(packet);
        }

        public void flushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (sendPacketQueue.Count > 0)
            {
                BasePacket packet = sendPacketQueue.Take();
                byte[] packetBytes = packet.getPacketBytes();
                byte[] buffer = new byte[0xffff];
                Array.Copy(packetBytes, buffer, packetBytes.Length);
                try
                {
                    socket.Send(packetBytes);
                }
                catch (Exception e)
                { Log.error(String.Format("Weird case, socket was d/ced: {0}", e)); }
            }
        }

        public String getAddress()
        {
            return String.Format("{0}:{1}", (socket.RemoteEndPoint as IPEndPoint).Address, (socket.RemoteEndPoint as IPEndPoint).Port);
        }

        public void disconnect()
        {
            if (socket.Connected)
                socket.Disconnect(false);
        }
    }
}

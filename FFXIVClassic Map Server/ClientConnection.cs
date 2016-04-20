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
        public byte[] buffer;
        private BlockingCollection<BasePacket> sendPacketQueue = new BlockingCollection<BasePacket>(1000);
        public int lastPartialSize = 0;

        //Instance Stuff
        public uint owner = 0;
        public int connType = 0;

        public void queuePacket(BasePacket packet)
        {
            sendPacketQueue.Add(packet);
        }

        public void queuePacket(SubPacket subpacket, bool isAuthed, bool isEncrypted)
        {
            sendPacketQueue.Add(BasePacket.createPacket(subpacket, isAuthed, isEncrypted));
        }

        public void flushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (sendPacketQueue.Count > 0)
            {
                BasePacket packet = sendPacketQueue.Take();                

                byte[] packetBytes = packet.getPacketBytes();

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

        public bool isConnected()
        {
            return (socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        }

        public void disconnect()
        {
            if (socket.Connected)
                socket.Disconnect(false);
        }
    }
}

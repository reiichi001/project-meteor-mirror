using System;
using System.Net.Sockets;

using FFXIVClassic.Common;
using System.Collections.Concurrent;
using System.Net;
using FFXIVClassic_Map_Server.packets.WorldPackets.Send;

namespace FFXIVClassic_Map_Server.dataobjects
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

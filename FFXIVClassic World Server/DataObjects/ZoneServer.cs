using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_World_Server.Packets.WorldPackets.Send;

namespace FFXIVClassic_World_Server.DataObjects
{
    class ZoneServer
    {
        public readonly string zoneServerIp;
        public readonly int zoneServerPort;
        public readonly List<uint> ownedZoneIds;
        public bool isConnected = false;
        public Socket zoneServerConnection;
        private ClientConnection conn;        

        private byte[] buffer = new byte[0xFFFF];

        public ZoneServer(string ip, int port, uint firstId)
        {
            zoneServerIp = ip;
            zoneServerPort = port;

            ownedZoneIds = new List<uint>();
            ownedZoneIds.Add(firstId);
        }

        public void AddLoadedZone(uint id)
        {
            ownedZoneIds.Add(id);
        }

        public bool Connect()
        {
            Program.Log.Info("Connecting to zone server @ {0}:{1}", zoneServerIp, zoneServerPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(zoneServerIp), zoneServerPort);
            zoneServerConnection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                zoneServerConnection.Connect(remoteEP);
                isConnected = true;
                conn = new ClientConnection();
                conn.socket = zoneServerConnection;
                conn.buffer = new byte[0xFFFF];

                try
                {
                    zoneServerConnection.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Error occured starting listeners, check inner exception", e);
                }
            }
            catch (Exception e)
            { Program.Log.Error("Failed to connect"); return false; }

            return true;
        }

        public void SendPacket(SubPacket subpacket)
        {
            if (isConnected)
            {
                byte[] packetBytes = subpacket.GetBytes();

                try
                {
                    zoneServerConnection.Send(packetBytes);
                }
                catch (Exception e)
                { Program.Log.Error("Weird case, socket was d/ced: {0}", e); }
            }    
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;            
            //Check if disconnected
            if ((conn.socket.Poll(1, SelectMode.SelectRead) && conn.socket.Available == 0))
            {
                conn = null;
                isConnected = false;
                Program.Log.Info("Zone server @ {0}:{1} disconnected!", zoneServerIp, zoneServerPort);
                return;
            }

            try
            {
                int bytesRead = conn.socket.EndReceive(result);

                bytesRead += conn.lastPartialSize;

                if (bytesRead >= 0)
                {
                    int offset = 0;

                    //Build packets until can no longer or out of data
                    while (true)
                    {
                        SubPacket subpacket = SubPacket.CreatePacket(ref offset, conn.buffer, bytesRead);

                        //If can't build packet, break, else process another
                        if (subpacket == null)
                            break;
                        else
                            Server.GetServer().OnReceiveSubPacketFromZone(this, subpacket);
                    }

                    //Not all bytes consumed, transfer leftover to beginning
                    if (offset < bytesRead)
                        Array.Copy(conn.buffer, offset, conn.buffer, 0, bytesRead - offset);

                    conn.lastPartialSize = bytesRead - offset;

                    //Build any queued subpackets into basepackets and send
                    conn.FlushQueuedSendPackets();

                    if (offset < bytesRead)
                        //Need offset since not all bytes consumed
                        conn.socket.BeginReceive(conn.buffer, bytesRead - offset, conn.buffer.Length - (bytesRead - offset), SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                    else
                        //All bytes consumed, full buffer available
                        conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                }
                else
                {
                    conn = null;
                    isConnected = false;
                    Program.Log.Info("Zone server @ {0}:{1} disconnected!", zoneServerIp, zoneServerPort);
                }
            }
            catch (SocketException)
            {
                conn = null;
                    isConnected = false;
                    Program.Log.Info("Zone server @ {0}:{1} disconnected!", zoneServerIp, zoneServerPort);
            }
        }

        public void SendSessionStart(Session session)
        {
            SendPacket(SessionBeginPacket.BuildPacket(session));
        }

        public void SendSessionEnd(Session session)
        {
            SendPacket(SessionEndPacket.BuildPacket(session));
        }

        public void SendSessionEnd(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            SendPacket(SessionEndPacket.BuildPacket(session, destinationZoneId, destinationPrivateArea, spawnType, spawnX, spawnY, spawnZ, spawnRotation));
        }

    }
}

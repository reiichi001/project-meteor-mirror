using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects
{
    class ZoneServer
    {
        public readonly string zoneServerIp;
        public readonly int zoneServerPort;
        public bool isConnected = false;
        public Socket zoneServerConnection;

        public ZoneServer(string ip, int port)
        {
            zoneServerIp = ip;
            zoneServerPort = port;
        }

        public void Connect()
        {
            Program.Log.Info("Connecting to zone server @ {0}:{1}", zoneServerIp, zoneServerPort);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(zoneServerIp), zoneServerPort);
            zoneServerConnection = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                zoneServerConnection.Connect(remoteEP);
                isConnected = true;
            }
            catch (Exception e)
            { Program.Log.Error("Failed to connect"); return; }            
        }
    }
}

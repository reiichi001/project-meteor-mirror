using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_MAP_PORT     = 54992;
        public const int BUFFER_SIZE        = 0x400;
        public const int BACKLOG            = 100;

        private Socket mServerSocket;

        private Dictionary<uint,Player> mConnectedPlayerList = new Dictionary<uint,Player>();
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private PacketProcessor mProcessor;
        private Thread mProcessorThread;

        #region Socket Handling
        public bool startServer()
        {
            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), FFXIV_MAP_PORT);

            try{
                mServerSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);         
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not create socket, check to make sure not duplicating port", e);
            }
            try
            {
                mServerSocket.Bind(serverEndPoint);
                mServerSocket.Listen(BACKLOG);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured while binding socket, check inner exception", e);
            }
            try
            {
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured starting listeners, check inner exception", e);
            }

            Console.Write("Game server has started @ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0}:{1}", (mServerSocket.LocalEndPoint as IPEndPoint).Address, (mServerSocket.LocalEndPoint as IPEndPoint).Port);
            Console.ForegroundColor = ConsoleColor.Gray;

            mProcessor = new PacketProcessor(mConnectedPlayerList, mConnectionList);
            mProcessorThread = new Thread(new ThreadStart(mProcessor.update));
            mProcessorThread.Start();
            return true;
        }

        private void acceptCallback(IAsyncResult result)
        {
            ClientConnection conn = null;
            Socket socket = (System.Net.Sockets.Socket)result.AsyncState;
           
            try
            {

                conn = new ClientConnection();
                conn.socket = socket.EndAccept(result);
                conn.buffer = new byte[BUFFER_SIZE];

                lock (mConnectionList)
                {
                    mConnectionList.Add(conn);
                }

                Log.conn(String.Format("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port));
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                //Queue the accept of the next incomming connection
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (SocketException)
            {
                if (conn != null)
                {
                
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (Exception)
            {
                if (conn != null)
                {                   
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
        }

        private Player findPlayerBySocket(Socket s)
        {
            lock (mConnectedPlayerList)
            {
                foreach (KeyValuePair<uint,Player> p in mConnectedPlayerList)
                {
                    if ((p.Value.getConnection1().socket.RemoteEndPoint as IPEndPoint).Address.Equals((s.RemoteEndPoint as IPEndPoint).Address))
                    {
                        return p.Value;
                    }

                    if ((p.Value.getConnection2().socket.RemoteEndPoint as IPEndPoint).Address.Equals((s.RemoteEndPoint as IPEndPoint).Address))
                    {
                        return p.Value;
                    }
                }
            }

            return null;
        }

        private Player findPlayerByClientConnection(ClientConnection conn)
        {
            throw new NotImplementedException();
        } 

        private void receiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;            

            try
            {
                int bytesRead = conn.socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    conn.processIncoming(bytesRead);

                    //Queue the next receive
                    conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                }
                else
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.owner == 0 ? conn.getAddress() : "User " + conn.owner));
                  
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
            }
            catch (SocketException)
            {                
                if (conn.socket != null)
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.owner == 0 ? conn.getAddress() : "User " + conn.owner));
                    
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }          
                }
            }
        }

        #endregion


        public void sendPacket(string path, int conn)
        {
            mProcessor.sendPacket(path, conn);
        }
    }
}

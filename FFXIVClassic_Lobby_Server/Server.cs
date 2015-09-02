using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_LOBBY_PORT   = 54994;
        public const int BUFFER_SIZE        = 0x400;
        public const int BACKLOG            = 100;

        private Socket mServerSocket;
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private PacketProcessor mProcessor;
        private Thread mProcessorThread;

        #region Socket Handling
        public bool startServer()
        {
            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("141.117.161.40"), FFXIV_LOBBY_PORT);
           
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

            Console.Write("Server has started @ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0}:{1}", (mServerSocket.LocalEndPoint as IPEndPoint).Address, (mServerSocket.LocalEndPoint as IPEndPoint).Port);
            Console.ForegroundColor = ConsoleColor.Gray;

            mProcessor = new PacketProcessor(mConnectionList);
            mProcessorThread = new Thread(new ThreadStart(mProcessor.update));
            mProcessorThread.Start();
            return true;
        }

        private void acceptCallback(IAsyncResult result)
        {
            ClientConnection conn = null;
            try
            {
                System.Net.Sockets.Socket s = (System.Net.Sockets.Socket)result.AsyncState;
                conn = new ClientConnection();
                conn.socket = s.EndAccept(result);
                conn.buffer = new byte[BUFFER_SIZE];
                lock (mConnectionList)
                {
                    mConnectionList.Add(conn);
                }
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                //Queue the accept of the next incomming connection
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
                Console.WriteLine("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port);
            }
            catch (SocketException)
            {
                if (conn.socket != null)
                {
                    conn.socket.Close();
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (Exception)
            {
                if (conn.socket != null)
                {
                    conn.socket.Close();
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
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
                    Console.WriteLine("{0} has disconnected.", conn.currentUserId == 0 ? conn.getAddress() : "User " + conn.currentUserId);
                    conn.socket.Close();
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
                    Console.WriteLine("Connection @ {0} has disconnected.", conn.currentUserId == 0 ? "Unknown User " : "User " + conn.currentUserId);
                    conn.socket.Close();
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }                   
                }
            }
        }        

        #endregion

        #region Packet Handling
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_LOBBY_PORT   = 54994;
        public const int BUFFER_SIZE        = 0xFFFF;
        public const int BACKLOG            = 100;

        private const int CLEANUP_THREAD_SLEEP_TIME = 60;

        private Socket mServerSocket;
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private PacketProcessor mProcessor;

        private Thread cleanupThread;
        private bool killCleanupThread = false;

        private void socketCleanup()
        {
            Console.WriteLine("Cleanup thread started; it will run every {0} seconds.", CLEANUP_THREAD_SLEEP_TIME);
            while (!killCleanupThread)
            {
                int count = 0;
                for (int i = mConnectionList.Count - 1; i >= 0; i--)
                {
                    ClientConnection conn = mConnectionList[i];
                    if (conn.socket.Poll(1, SelectMode.SelectRead) && conn.socket.Available == 0)
                    {
                        conn.socket.Disconnect(false);
                        mConnectionList.Remove(conn);
                        count++;
                    }
                }
                if (count != 0)
                    Log.conn(String.Format("{0} connections were cleaned up.", count));
                Thread.Sleep(CLEANUP_THREAD_SLEEP_TIME*1000);
            }
        }

        #region Socket Handling
        public bool startServer()
        {
            //cleanupThread = new Thread(new ThreadStart(socketCleanup));
            //cleanupThread.Name = "LobbyThread:Cleanup";
            //cleanupThread.Start();

            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), FFXIV_LOBBY_PORT);
           
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

            mProcessor = new PacketProcessor();

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
                Log.conn(String.Format("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port));
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

                bytesRead += conn.lastPartialSize;

                if (bytesRead > 0)
                {
                    int offset = 0;

                    //Build packets until can no longer or out of data
                    while (true)
                    {
                        BasePacket basePacket = buildPacket(ref offset, conn.buffer, bytesRead);

                        //If can't build packet, break, else process another
                        if (basePacket == null)
                            break;
                        else
                            mProcessor.processPacket(conn, basePacket);
                    }

                    //Not all bytes consumed, transfer leftover to beginning
                    if (offset < bytesRead)
                        Array.Copy(conn.buffer, offset, conn.buffer, 0, bytesRead - offset);

                    Array.Clear(conn.buffer, bytesRead - offset, conn.buffer.Length - (bytesRead - offset));

                    conn.lastPartialSize = bytesRead - offset;

                    //Build any queued subpackets into basepackets and send
                    conn.flushQueuedSendPackets();

                    if (offset < bytesRead)
                        //Need offset since not all bytes consumed
                        conn.socket.BeginReceive(conn.buffer, bytesRead - offset, conn.buffer.Length - (bytesRead - offset), SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                    else
                        //All bytes consumed, full buffer available
                        conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                }
                else
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.currentUserId == 0 ? conn.getAddress() : "User " + conn.currentUserId));

                    lock (mConnectionList)
                    {
                        conn.disconnect();
                        mConnectionList.Remove(conn);
                    }
                }
            }
            catch (SocketException)
            {
                if (conn.socket != null)
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.currentUserId == 0 ? conn.getAddress() : "User " + conn.currentUserId));

                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
            }
        }      

        /// <summary>
        /// Builds a packet from the incoming buffer + offset. If a packet can be built, it is returned else null.
        /// </summary>
        /// <param name="offset">Current offset in buffer.</param>
        /// <param name="buffer">Incoming buffer.</param>
        /// <returns>Returns either a BasePacket or null if not enough data.</returns>
        public BasePacket buildPacket(ref int offset, byte[] buffer, int bytesRead)
        {
            BasePacket newPacket = null;

            //Too small to even get length
            if (bytesRead <= offset)
                return null;

            ushort packetSize = BitConverter.ToUInt16(buffer, offset);

            //Too small to whole packet
            if (bytesRead < offset + packetSize)
                return null;

            if (buffer.Length < offset + packetSize)
                return null;

            try
            {
                newPacket = new BasePacket(buffer, ref offset);
            }
            catch (OverflowException)
            {
                return null;
            }

            return newPacket;
        }

        #endregion

    }
}

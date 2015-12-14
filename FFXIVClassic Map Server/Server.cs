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
using System.IO;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_MAP_PORT     = 54992;
        public const int BUFFER_SIZE        = 0x400;
        public const int BACKLOG            = 100;

        private Socket mServerSocket;

        private Dictionary<uint,ConnectedPlayer> mConnectedPlayerList = new Dictionary<uint,ConnectedPlayer>();
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private PacketProcessor mProcessor;
        private Thread mProcessorThread;
        private Thread mGameThread;

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

            //mGameThread = new Thread(new ThreadStart(mProcessor.update));
            //mGameThread.Start();
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

        /// <summary>
        /// Receive Callback. Reads in incoming data, converting them to base packets. Base packets are sent to be parsed. If not enough data at the end to build a basepacket, move to the beginning and prepend.
        /// </summary>
        /// <param name="result"></param>
        private void receiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;            

            try
            {
                int bytesRead = conn.socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    int offset = 0;

                    //Build packets until can no longer or out of data
                    while(true)
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

        public void sendPacket(string path, int conn)
        {
            mProcessor.sendPacket(path, conn);
        }

        public void testCodePacket(uint id, uint value, string target)
        {
            SetActorPropetyPacket changeProperty = new SetActorPropetyPacket();
            changeProperty.addInt(id, value);
            changeProperty.setTarget(target);

            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
            {
                SubPacket changePropertyPacket = changeProperty.buildPacket((entry.Value.actorID), (entry.Value.actorID));
                BasePacket packet = BasePacket.createPacket(changePropertyPacket, true, false);
                packet.debugPrintPacket();
                if (entry.Value.getConnection1() != null)
                    entry.Value.getConnection1().queuePacket(packet);
                else
                    Log.error("Connection was null");
                if (entry.Value.getConnection2() != null)
                    entry.Value.getConnection2().queuePacket(packet);
                else
                    Log.error("Connection was null");
            }
        }

        public void testCodePacket2(string name, string target)
        {
            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
            {
                SetActorPropetyPacket changeProperty = new SetActorPropetyPacket();
                changeProperty.addProperty(entry.Value.getActor(), name);
                changeProperty.addProperty(entry.Value.getActor(), "charaWork.parameterSave.hpMax[0]");
                changeProperty.setTarget(target);

                SubPacket changePropertyPacket = changeProperty.buildPacket((entry.Value.actorID), (entry.Value.actorID));
                BasePacket packet = BasePacket.createPacket(changePropertyPacket, true, false);
                packet.debugPrintPacket();
                entry.Value.getConnection1().queuePacket(packet);
                entry.Value.getConnection2().queuePacket(packet);
            }
        }

    }
}

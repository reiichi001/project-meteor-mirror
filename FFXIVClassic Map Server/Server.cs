using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FFXIVClassic_Map_Server.dataobjects;

using FFXIVClassic.Common;
using NLog;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;

namespace FFXIVClassic_Map_Server
{
    class Server
    {
        public const int FFXIV_MAP_PORT = 54992;
        public const int BUFFER_SIZE = 0xFFFF; //Max basepacket size is 0xFFFF
        public const int BACKLOG = 100;
        public const int HEALTH_THREAD_SLEEP_TIME = 5;

        public const string STATIC_ACTORS_PATH = "./staticactors.bin";

        private static Server mSelf;

        private Socket mServerSocket;

        private Dictionary<uint, ConnectedPlayer> mConnectedPlayerList = new Dictionary<uint, ConnectedPlayer>();
        private ZoneConnection mWorldConnection = new ZoneConnection();
        private LuaEngine mLuaEngine = new LuaEngine();

        private static WorldManager mWorldManager;
        private static Dictionary<uint, Item> gamedataItems;
        private static StaticActors mStaticActors;

        private PacketProcessor mProcessor;

        private Thread mConnectionHealthThread;
        private bool killHealthThread = false;

        private void ConnectionHealth()
        {
            Program.Log.Info("Connection Health thread started; it will run every {0} seconds.", HEALTH_THREAD_SLEEP_TIME);
            while (!killHealthThread)
            {
                lock (mConnectedPlayerList)
                {
                    List<ConnectedPlayer> dcedPlayers = new List<ConnectedPlayer>();
                    foreach (ConnectedPlayer cp in mConnectedPlayerList.Values)
                    {
                        if (cp.CheckIfDCing())
                            dcedPlayers.Add(cp);
                    }

                    foreach (ConnectedPlayer cp in dcedPlayers)
                        cp.GetActor().CleanupAndSave();
                }
                Thread.Sleep(HEALTH_THREAD_SLEEP_TIME * 1000);
            }
        }

        public Server()
        {
            mSelf = this;
        }

        public static Server GetServer()
        {
            return mSelf;
        }

        public bool StartServer()
        {
            mConnectionHealthThread = new Thread(new ThreadStart(ConnectionHealth));
            mConnectionHealthThread.Name = "MapThread:Health";
            //mConnectionHealthThread.Start();

            mStaticActors = new StaticActors(STATIC_ACTORS_PATH);

            gamedataItems = Database.GetItemGamedata();
            Program.Log.Info("Loaded {0} items.", gamedataItems.Count);

            mWorldManager = new WorldManager(this);
            mWorldManager.LoadZoneList();
            mWorldManager.LoadZoneEntranceList();
            mWorldManager.LoadActorClasses();
            mWorldManager.LoadSpawnLocations();
            mWorldManager.SpawnAllActors();

            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), int.Parse(ConfigConstants.OPTIONS_PORT));

            try
            {
                mServerSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not Create socket, check to make sure not duplicating port", e);
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
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured starting listeners, check inner exception", e);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Program.Log.Info("Map Server has started @ {0}:{1}", (mServerSocket.LocalEndPoint as IPEndPoint).Address, (mServerSocket.LocalEndPoint as IPEndPoint).Port);
            Console.ForegroundColor = ConsoleColor.Gray;

            mProcessor = new PacketProcessor(this, mConnectedPlayerList);

            //mGameThread = new Thread(new ThreadStart(mProcessor.update));
            //mGameThread.Start();
            return true;
        }

        public void RemovePlayer(Player player)
        {
            lock (mConnectedPlayerList)
            {
                if (mConnectedPlayerList.ContainsKey(player.actorId))
                    mConnectedPlayerList.Remove(player.actorId);
            }
        }

        #region Socket Handling
        private void AcceptCallback(IAsyncResult result)
        {
            ZoneConnection conn = null;
            Socket socket = (System.Net.Sockets.Socket)result.AsyncState;

            try
            {

                conn = new ZoneConnection();
                conn.socket = socket.EndAccept(result);
                conn.buffer = new byte[BUFFER_SIZE];

                mWorldConnection = conn;
                
                Program.Log.Info("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port);
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), conn);
                //Queue the accept of the next incomming connection
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (SocketException)
            {
                if (conn != null)
                {
                    mWorldConnection = null;
                }
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
            catch (Exception)
            {
                if (conn != null)
                {
                    mWorldConnection = null;
                }
                mServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), mServerSocket);
            }
        }

        public static Actor GetStaticActors(uint id)
        {
            return mStaticActors.GetActor(id);
        }

        public static Actor GetStaticActors(string name)
        {
            return mStaticActors.FindStaticActor(name);
        }

        public static Item GetItemGamedata(uint id)
        {
            if (gamedataItems.ContainsKey(id))
                return gamedataItems[id];
            else
                return null;
        }

        /// <summary>
        /// Receive Callback. Reads in incoming data, converting them to base packets. Base packets are sent to be parsed. If not enough data at the end to build a basepacket, move to the beginning and prepend.
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            ZoneConnection conn = (ZoneConnection)result.AsyncState;

            //Check if disconnected
            if ((conn.socket.Poll(1, SelectMode.SelectRead) && conn.socket.Available == 0))
            {
                mWorldConnection = null;
                Program.Log.Info("Disconnected from world server!");
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
                        SubPacket subPacket = SubPacket.CreatePacket(ref offset, conn.buffer, bytesRead);

                        //If can't build packet, break, else process another
                        if (subPacket == null)
                            break;
                        else
                            mProcessor.ProcessPacket(conn, subPacket);
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
                    mWorldConnection = null;
                    Program.Log.Info("Disconnected from world server!");
                }
            }
            catch (SocketException)
            {
                if (conn.socket != null)
                {
                    mWorldConnection = null;
                    Program.Log.Info("Disconnected from world server!");
                }
            }
        }

        #endregion


        public static WorldManager GetWorldManager()
        {
            return mWorldManager;
        }

        public Dictionary<uint, ConnectedPlayer> GetConnectedPlayerList()
        {
            return mConnectedPlayerList;
        }

        public static Dictionary<uint, Item> GetGamedataItems()
        {
            return gamedataItems;
        }

    }
}
/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Meteor.Map.dataobjects;

using Meteor.Common;
using Meteor.Map.Actors;

namespace Meteor.Map
{
    class Server
    {
        public const int FFXIV_MAP_PORT = 54992;
        public const int BUFFER_SIZE = 0xFFFF; //Max basepacket size is 0xFFFF
        public const int BACKLOG = 100;

        public const string STATIC_ACTORS_PATH = "./staticactors.bin";

        private static Server mSelf;

        private Socket mServerSocket;

        private Dictionary<uint, Session> mSessionList = new Dictionary<uint, Session>();        
     
        private static CommandProcessor mCommandProcessor = new CommandProcessor();
        private static ZoneConnection mWorldConnection = new ZoneConnection();
        private static WorldManager mWorldManager;
        private static Dictionary<uint, ItemData> mGamedataItems;
        private static Dictionary<uint, GuildleveData> mGamedataGuildleves;
        private static StaticActors mStaticActors;

        private PacketProcessor mProcessor;        

        public Server()
        {
            mSelf = this;
        }
        
        public bool StartServer()
        {           
            mStaticActors = new StaticActors(STATIC_ACTORS_PATH);

            mGamedataItems = Database.GetItemGamedata();
            Program.Log.Info("Loaded {0} items.", mGamedataItems.Count);
            mGamedataGuildleves = Database.GetGuildleveGamedata();
            Program.Log.Info("Loaded {0} guildleves.", mGamedataGuildleves.Count);

            mWorldManager = new WorldManager(this);
            mWorldManager.LoadZoneList();
            mWorldManager.LoadZoneEntranceList();
            mWorldManager.LoadSeamlessBoundryList();
            mWorldManager.LoadActorClasses();
            mWorldManager.LoadSpawnLocations();
            mWorldManager.LoadBattleNpcs();
            mWorldManager.LoadStatusEffects();
            mWorldManager.LoadBattleCommands();
            mWorldManager.LoadBattleTraits();
            mWorldManager.SpawnAllActors();
            mWorldManager.StartZoneThread();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), int.Parse(ConfigConstants.OPTIONS_PORT));

            try
            {
                mServerSocket = new Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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

            mProcessor = new PacketProcessor(this);

            //mGameThread = new Thread(new ThreadStart(mProcessor.update));
            //mGameThread.Start();
            return true;
        }

        #region Session Handling

        public Session AddSession(uint id)
        {
            if (mSessionList.ContainsKey(id))
            {
                mSessionList[id].ClearInstance();
                return mSessionList[id];
            }

            Session session = new Session(id);
            mSessionList.Add(id, session);
            return session;
        }

        public void RemoveSession(uint id)
        {
            if (mSessionList.ContainsKey(id))
            {
                mSessionList.Remove(id);                
            }
        }

        public Session GetSession(uint id)
        {
            if (mSessionList.ContainsKey(id))
                return mSessionList[id];
            else
                return null;
        }

        public Session GetSession(string name)
        {
            foreach (Session s in mSessionList.Values)
            {
                if (s.GetActor().customDisplayName.ToLower().Equals(name.ToLower()))
                    return s;
            }
            return null;
        }

        public Dictionary<uint, Session> GetSessionList()
        {
            return mSessionList;
        }

        #endregion

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

        public static ZoneConnection GetWorldConnection()
        {
            return mWorldConnection;
        }

        public static Server GetServer()
        {
            return mSelf;
        }

        public static CommandProcessor GetCommandProcessor()
        {
            return mCommandProcessor;
        }        

        public static WorldManager GetWorldManager()
        {
            return mWorldManager;
        }
        
        public static Dictionary<uint, ItemData> GetGamedataItems()
        {
            return mGamedataItems;
        }

        public static Actor GetStaticActors(uint id)
        {
            return mStaticActors.GetActor(id);
        }

        public static Actor GetStaticActors(string name)
        {
            return mStaticActors.FindStaticActor(name);
        }

        public static ItemData GetItemGamedata(uint id)
        {
            if (mGamedataItems.ContainsKey(id))
                return mGamedataItems[id];
            else
                return null;
        }

        public static GuildleveData GetGuildleveGamedata(uint id)
        {
            if (mGamedataGuildleves.ContainsKey(id))
                return mGamedataGuildleves[id];
            else
                return null;
        }

    }
}

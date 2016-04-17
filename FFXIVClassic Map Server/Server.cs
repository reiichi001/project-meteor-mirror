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
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.actors.chara.player;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_MAP_PORT     = 54992;
        public const int BUFFER_SIZE        = 0xFFFF; //Max basepacket size is 0xFFFF
        public const int BACKLOG            = 100;
        public const int HEALTH_THREAD_SLEEP_TIME = 5;

        public const string STATIC_ACTORS_PATH = "./staticactors.bin";

        private static Server mSelf;

        private Socket mServerSocket;

        private Dictionary<uint,ConnectedPlayer> mConnectedPlayerList = new Dictionary<uint,ConnectedPlayer>();
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private LuaEngine mLuaEngine = new LuaEngine();

        private static WorldManager mWorldManager;
        private static Dictionary<uint, Item> gamedataItems;
        private static StaticActors mStaticActors;

        private PacketProcessor mProcessor;

        private Thread mConnectionHealthThread;
        private bool killHealthThread = false;

        private void connectionHealth()
        {
            Log.info(String.Format("Connection Health thread started; it will run every {0} seconds.", HEALTH_THREAD_SLEEP_TIME));
            while (!killHealthThread)
            {
                lock (mConnectedPlayerList)
                {
                    List<ConnectedPlayer> dcedPlayers = new List<ConnectedPlayer>();
                    foreach (ConnectedPlayer cp in mConnectedPlayerList.Values)
                    {
                        if (cp.checkIfDCing())
                            dcedPlayers.Add(cp);
                    }

                    foreach (ConnectedPlayer cp in dcedPlayers)
                        cp.getActor().cleanupAndSave();
                }
                Thread.Sleep(HEALTH_THREAD_SLEEP_TIME * 1000);
            }
        }

        public Server()
        {
            mSelf = this;
        }

        public static Server getServer()
        {
            return mSelf;
        }

        public bool startServer()
        {
            mConnectionHealthThread = new Thread(new ThreadStart(connectionHealth));
            mConnectionHealthThread.Name = "MapThread:Health";
            //mConnectionHealthThread.Start();

            mStaticActors = new StaticActors(STATIC_ACTORS_PATH);
            
            gamedataItems = Database.getItemGamedata();
            Log.info(String.Format("Loaded {0} items.",gamedataItems.Count));

            mWorldManager = new WorldManager(this);
            mWorldManager.LoadZoneList();
            mWorldManager.LoadZoneEntranceList();
            mWorldManager.LoadNPCs();

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

            mProcessor = new PacketProcessor(this, mConnectedPlayerList, mConnectionList);

            //mGameThread = new Thread(new ThreadStart(mProcessor.update));
            //mGameThread.Start();
            return true;
        }

        public void removePlayer(Player player)
        {
            lock (mConnectedPlayerList)
            {
                if (mConnectedPlayerList.ContainsKey(player.actorId))
                    mConnectedPlayerList.Remove(player.actorId);
            }
        }

        #region Socket Handling
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

        public static Actor getStaticActors(uint id)
        {
            return mStaticActors.getActor(id);
        }

        public static Actor getStaticActors(string name)
        {
            return mStaticActors.findStaticActor(name);
        }

        public static Item getItemGamedata(uint id)
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
        private void receiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;

            //Check if disconnected
            if ((conn.socket.Poll(1, SelectMode.SelectRead) && conn.socket.Available == 0))
            {
                if (mConnectedPlayerList.ContainsKey(conn.owner))
                    mConnectedPlayerList.Remove(conn.owner);
                lock (mConnectionList)
                {
                    mConnectionList.Remove(conn);
                } 
                if (conn.connType == BasePacket.TYPE_ZONE)
                    Log.conn(String.Format("{0} has disconnected.", conn.owner == 0 ? conn.getAddress() : "User " + conn.owner));
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

        public void sendPacket(ConnectedPlayer client, string path)
        {
            BasePacket packet = new BasePacket(path);
    
            if (client != null)
            {
                packet.replaceActorID(client.actorID);
                client.queuePacket(packet);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    packet.replaceActorID(entry.Value.actorID);
                    entry.Value.queuePacket(packet);
                }
            }
        }

        public void changeProperty(uint id, uint value, string target)
        {
            SetActorPropetyPacket changeProperty = new SetActorPropetyPacket(target);

            changeProperty.setTarget(target);
            changeProperty.addInt(id, value);
            changeProperty.addTarget();

            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
            {
                SubPacket changePropertyPacket = changeProperty.buildPacket((entry.Value.actorID), (entry.Value.actorID));
                
                BasePacket packet = BasePacket.createPacket(changePropertyPacket, true, false);
                packet.debugPrintPacket();

                entry.Value.queuePacket(packet);               
            }
        }

        public void doMusic(ConnectedPlayer client, string music)
        {
            ushort musicId;
            
            if (music.ToLower().StartsWith("0x"))
                musicId = Convert.ToUInt16(music, 16);
            else
                musicId = Convert.ToUInt16(music);

            if (client != null)
                client.queuePacket(BasePacket.createPacket(SetMusicPacket.buildPacket(client.actorID, musicId, 1), true, false));
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    BasePacket musicPacket = BasePacket.createPacket(SetMusicPacket.buildPacket(entry.Value.actorID, musicId, 1), true, false);
                    entry.Value.queuePacket(musicPacket);
                }
            }
        }

        public void doWarp(ConnectedPlayer client, string entranceId)
        {            
            uint id;

            try
            {
                if (entranceId.ToLower().StartsWith("0x"))
                    id = Convert.ToUInt32(entranceId, 16);
                else
                    id = Convert.ToUInt32(entranceId);
            }
            catch(FormatException e)
            {return;}

            FFXIVClassic_Map_Server.WorldManager.ZoneEntrance ze = mWorldManager.getZoneEntrance(id);

            if (ze == null)
                return;

            if (client != null)
                mWorldManager.DoZoneChange(client.getActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, 0.0f);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    mWorldManager.DoZoneChange(entry.Value.getActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, 0.0f);
                }
            }
        }

        public void doWarp(ConnectedPlayer client, string zone, string privateArea, string sx, string sy, string sz, string spawnType)
        {
            uint zoneId;
            float x,y,z;
            byte sType; 

            if (zone.ToLower().StartsWith("0x"))
                zoneId = Convert.ToUInt32(zone, 16);
            else
                zoneId = Convert.ToUInt32(zone);

            if (spawnType.ToLower().StartsWith("0x"))
                sType = Convert.ToByte(spawnType, 16);
            else
                
                sType = Convert.ToByte(spawnType);

            if (mWorldManager.GetZone(zoneId) == null)
            {
                if (client != null)
                    client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Zone does not exist or setting isn't valid."), true, false));
                Log.error("Zone does not exist or setting isn't valid.");
            }

            x = Single.Parse(sx);
            y = Single.Parse(sy);
            z = Single.Parse(sz);

            if (client != null)
            {
                if (zoneId == client.getActor().zoneId)
                    mWorldManager.DoPlayerMoveInZone(client.getActor(), x, y, z, 0.0f, sType);
                else
                    mWorldManager.DoZoneChange(client.getActor(), zoneId, privateArea, sType, x, y, z, 0.0f);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    if (zoneId == entry.Value.getActor().zoneId)
                        mWorldManager.DoPlayerMoveInZone(entry.Value.getActor(), x, y, z, 0.0f, 0x0);
                    else
                        mWorldManager.DoZoneChange(entry.Value.getActor(), zoneId, privateArea, 0x2, x, y, z, 0.0f);
                }
            }
        }

        public static WorldManager GetWorldManager()
        {
            return mWorldManager;
        }

        public void printPos(ConnectedPlayer client)
        {
            if (client != null)
            {
                Player p = client.getActor();
                client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Position: {1}, {2}, {3}, {4}", p.customDisplayName, p.positionX, p.positionY, p.positionZ, p.rotation)), true, false));
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    Log.info(String.Format("{0} position: {1}, {2}, {3}, {4}", p.customDisplayName, p.positionX, p.positionY, p.positionZ, p.rotation));
                }
            }
        }

        private void setGraphic(ConnectedPlayer client, uint slot, uint wId, uint eId, uint vId, uint cId)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.graphicChange(slot, wId, eId, vId, cId);
                p.sendAppearance();
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.graphicChange(slot, wId, eId, vId, cId);
                    p.sendAppearance();
                }
            }
        }

        private void giveItem(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.NORMAL).addItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.NORMAL).addItem(itemId, quantity);
                }
            }
        }

        private void giveItem(ConnectedPlayer client, uint itemId, int quantity, ushort type)
        {
            if (client != null)
            {
                Player p = client.getActor();

                if (p.getInventory(type) != null)
                    p.getInventory(type).addItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();

                    if (p.getInventory(type) != null)
                        p.getInventory(type).addItem(itemId, quantity);
                }
            }
        }

        private void removeItem(ConnectedPlayer client, uint itemId, int quantity)        
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.NORMAL).removeItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.NORMAL).removeItem(itemId, quantity);
                }
            }
        }

        private void removeItem(ConnectedPlayer client, uint itemId, int quantity, ushort type)
        {
            if (client != null)
            {
                Player p = client.getActor();

                if (p.getInventory(type) != null)
                    p.getInventory(type).removeItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();

                    if (p.getInventory(type) != null)
                        p.getInventory(type).removeItem(itemId, quantity);
                }
            }
        }

        private void giveCurrancy(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.CURRANCY).addItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.CURRANCY).addItem(itemId, quantity);
                }
            }
        }

        private void removeCurrancy(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.CURRANCY).removeItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.CURRANCY).removeItem(itemId, quantity);
                }
            }
        }

        private void giveKeyItem(ConnectedPlayer client, uint itemId)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.KEYITEMS).addItem(itemId, 1);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.KEYITEMS).addItem(itemId, 1);
                }
            }
        }

        private void removeKeyItem(ConnectedPlayer client, uint itemId)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.KEYITEMS).removeItem(itemId, 1);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.KEYITEMS).removeItem(itemId, 1);
                }
            }
        }

        internal bool doCommand(string input, ConnectedPlayer client)
        {
            input.Trim();
            if (input.StartsWith("!"))
                input = input.Substring(1);

            String[] split = input.Split(' ');

            if (split.Length >= 1)
            {
                if (split[0].Equals("mypos"))
                {
                    try
                    {
                        printPos(client);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not load packet: " + e);
                    }
                }
                else if (split[0].Equals("resetzone"))
                {                    
                    if (client != null)
                    {
                        Log.info(String.Format("Got request to reset zone: {0}", client.getActor().zoneId));
                        client.getActor().zone.clear();
                        client.getActor().zone.addActorToZone(client.getActor());
                        client.getActor().sendInstanceUpdate();
                        client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Resting zone {0}...", client.getActor().zoneId)), true, false));
                    }
                    mWorldManager.reloadZone(client.getActor().zoneId);
                    return true;
                }
                else if (split[0].Equals("reloaditems"))
                {
                    Log.info(String.Format("Got request to reload item gamedata"));
                    if (client != null)                    
                        client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Reloading Item Gamedata..."));                    
                    gamedataItems.Clear();
                    gamedataItems = Database.getItemGamedata();
                    Log.info(String.Format("Loaded {0} items.", gamedataItems.Count));
                    if (client != null)
                        client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Loaded {0} items.", gamedataItems.Count)));                    
                    return true;
                }
                else if (split[0].Equals("sendpacket"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        sendPacket(client, "./packets/" + split[1]);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not load packet: " + e);
                    }
                }
                else if (split[0].Equals("graphic"))
                {
                    try
                    {
                        if (split.Length == 6)
                            setGraphic(client, UInt32.Parse(split[1]), UInt32.Parse(split[2]), UInt32.Parse(split[3]), UInt32.Parse(split[4]), UInt32.Parse(split[5]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not give item.");
                    }
                }
                else if (split[0].Equals("giveitem"))
                {                    
                    try
                    {
                        if (split.Length == 2)
                            giveItem(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            giveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        else if (split.Length == 4)
                            giveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]), UInt16.Parse(split[3]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not give item.");
                    }
                }
                else if (split[0].Equals("removeitem"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        if (split.Length == 2)
                            removeItem(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            removeItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        else if (split.Length == 4)
                            removeItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]), UInt16.Parse(split[3]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not remove item.");
                    }
                }
                else if (split[0].Equals("givekeyitem"))
                {
                    try
                    {
                        if (split.Length == 2)
                            giveKeyItem(client, UInt32.Parse(split[1]));                       
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not give keyitem.");
                    }
                }
                else if (split[0].Equals("removekeyitem"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        if (split.Length == 2)
                            removeKeyItem(client, UInt32.Parse(split[1]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not remove keyitem.");
                    }
                }
                else if (split[0].Equals("givecurrancy"))
                {
                    try
                    {
                        if (split.Length == 2)
                            giveCurrancy(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            giveCurrancy(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not give currancy.");
                    }
                }
                else if (split[0].Equals("removecurrancy"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        if (split.Length == 2)
                            removeCurrancy(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            removeCurrancy(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not remove currancy.");
                    }
                }
                else if (split[0].Equals("music"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        doMusic(client, split[1]);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not change music: " + e);
                    }
                }
                else if (split[0].Equals("warp"))
                {
                    if (split.Length == 2)                    
                        doWarp(client, split[1]);
                    else if (split.Length == 6)
                        doWarp(client, split[1], null, split[2], split[3], split[4], split[5]);
                    else if (split.Length == 7)
                        doWarp(client, split[1], split[2], split[3], split[4], split[5], split[6]);
                    return true;
                }
                else if (split[0].Equals("property"))
                {
                    if (split.Length == 4)
                    {
                        changeProperty(Utils.MurmurHash2(split[1], 0), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                else if (split[0].Equals("property2"))
                {
                    if (split.Length == 4)
                    {
                        changeProperty(Convert.ToUInt32(split[1], 16), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
            }
            return false;
        }
    }

}

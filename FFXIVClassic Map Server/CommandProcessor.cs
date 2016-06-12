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
using FFXIVClassic_Map_Server.Properties;

namespace FFXIVClassic_Lobby_Server
{
    class CommandProcessor
    {
        private Dictionary<uint, ConnectedPlayer> mConnectedPlayerList;
        private static Dictionary<uint, Item> gamedataItems = Server.GetGamedataItems();

        // For the moment, this is the only predefined item
        // TODO: make a list/enum in the future so that items can be given by name, instead of by id
        const UInt32 ITEM_GIL = 1000001;

        public CommandProcessor(Dictionary<uint, ConnectedPlayer> playerList)
        {
            mConnectedPlayerList = playerList;
        }

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

        /// <summary>
        /// Teleports player to a location on a predefined list
        /// </summary>
        /// <param name="client">The current player</param>
        /// <param name="id">Predefined list: &lt;ffxiv_database&gt;\server_zones_spawnlocations</param>
        public void doWarp(ConnectedPlayer client, uint id)
        {
            WorldManager worldManager = Server.GetWorldManager();
            FFXIVClassic_Map_Server.WorldManager.ZoneEntrance ze = worldManager.getZoneEntrance(id);

            if (ze == null)
                return;

            if (client != null)
                worldManager.DoZoneChange(client.getActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    worldManager.DoZoneChange(entry.Value.getActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
                }
            }
        }

        public void doWarp(ConnectedPlayer client, uint zoneId, string privateArea, byte spawnType, float x, float y, float z, float r)
        {
            WorldManager worldManager = Server.GetWorldManager();
            if (worldManager.GetZone(zoneId) == null)
            {
                if (client != null)
                    client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Zone does not exist or setting isn't valid."), true, false));
                Log.error("Zone does not exist or setting isn't valid.");
            }

            if (client != null)
                worldManager.DoZoneChange(client.getActor(), zoneId, privateArea, spawnType, x, y, z, r);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    worldManager.DoZoneChange(entry.Value.getActor(), zoneId, privateArea, spawnType, x, y, z, r);
                }
            }
        }

        public void printPos(ConnectedPlayer client)
        {
            if (client != null)
            {
                Player p = client.getActor();
                client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("{0}\'s position: ZoneID: {1}, X: {2}, Y: {3}, Z: {4}, Rotation: {5}", p.customDisplayName, p.zoneId, p.positionX, p.positionY, p.positionZ, p.rotation)), true, false));
            }
            else
            { 
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    Log.info(String.Format("{0}\'s position: ZoneID: {1}, X: {2}, Y: {3}, Z: {4}, Rotation: {5}", p.customDisplayName, p.zoneId, p.positionX, p.positionY, p.positionZ, p.rotation));
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

        private void giveCurrency(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.CURRENCY).addItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.CURRENCY).addItem(itemId, quantity);
                }
            }
        }

        // TODO:  make removeCurrency() remove all quantity of a currency if quantity_to_remove > quantity_in_inventory instead of silently failing
        private void removeCurrency(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.getActor();
                p.getInventory(Inventory.CURRENCY).removeItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    p.getInventory(Inventory.CURRENCY).removeItem(itemId, quantity);
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

        private void parseWarp(ConnectedPlayer client, string[] split)
        {
            float x = 0, y = 0, z = 0, r = 0.0f;
            uint zoneId = 0;
            string privatearea = null;

            if (split.Length == 2) // Predefined list
            {
                // TODO: Handle !warp Playername
                #region !warp (predefined list)
                try
                {
                    if (split[1].ToLower().StartsWith("0x"))
                        zoneId = Convert.ToUInt32(split[1], 16);
                    else
                        zoneId = Convert.ToUInt32(split[1]);
                }
                catch{return;}
                #endregion

                doWarp(client, zoneId);
            }
            else if (split.Length == 4)
            {
                #region !warp X Y Z
                if (split[1].StartsWith("@"))
                {
                    split[1] = split[1].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[1]))
                        split[1] = "0";

                    try { x = Single.Parse(split[1]) + client.getActor().positionX; }
                    catch{return;}

                    split[1] = x.ToString();
                }
                if (split[2].StartsWith("@"))
                {
                    split[2] = split[2].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[2]))
                        split[2] = "0";

                    try { y = Single.Parse(split[2]) + client.getActor().positionY; }
                    catch{return;}

                    split[2] = y.ToString();
                }
                if (split[3].StartsWith("@"))
                {
                    split[3] = split[3].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[3]))
                        split[3] = "0";

                    try { z = Single.Parse(split[3]) + client.getActor().positionZ; }
                    catch{return;}

                    split[3] = z.ToString();
                }

                try
                {
                    x = Single.Parse(split[1]);
                    y = Single.Parse(split[2]);
                    z = Single.Parse(split[3]);
                }
                catch{return;}

                zoneId = client.getActor().zoneId;
                r = client.getActor().rotation;
                #endregion

                sendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                doWarp(client, zoneId, privatearea, 0x00, x, y, z, r);
            }
            else if (split.Length == 5)
            {
                #region !warp Zone X Y Z
                try
                {
                    x = Single.Parse(split[2]);
                    y = Single.Parse(split[3]);
                    z = Single.Parse(split[4]);
                }
                catch{return;}

                if (split[1].ToLower().StartsWith("0x"))
                {
                    try { zoneId = Convert.ToUInt32(split[1], 16); }
                    catch{return;}
                }
                else
                {
                    try { zoneId = Convert.ToUInt32(split[1]); }
                    catch{return;}
                }
                #endregion

                sendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                doWarp(client, zoneId, privatearea, 0x2, x, y, z, r);
            }
            else if (split.Length == 6)
            {
                #region !warp Zone Instance X Y Z
                try
                {
                    x = Single.Parse(split[3]);
                    y = Single.Parse(split[4]);
                    z = Single.Parse(split[5]);
                }
                catch{return;}

                if (split[1].ToLower().StartsWith("0x"))
                {
                    try { zoneId = Convert.ToUInt32(split[1], 16); }
                    catch{return;}
                }
                else
                {
                    try { zoneId = Convert.ToUInt32(split[1]); }
                    catch{return;}
                }

                privatearea = split[2];
                #endregion

                sendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                doWarp(client, zoneId, privatearea, 0x2, x, y, z, r);
            }
            else
                return; // catch any invalid warps here
        }

        private void doWeather(ConnectedPlayer client, string weatherID, string value)
        {
            ushort weather = Convert.ToUInt16(weatherID);

            if (client != null)
            {
                client.queuePacket(BasePacket.createPacket(SetWeatherPacket.buildPacket(client.actorID, weather, Convert.ToUInt16(value)), true, false));
            }

            /*
             * WIP: Change weather serverside, currently only clientside
             * 
            uint currentZoneID;
            if (client != null)
            {
                currentZoneID = client.getActor().zoneId;

                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    // Change the weather for everyone in the same zone
                    if (currentZoneID == entry.Value.getActor().zoneId)
                    {
                        BasePacket weatherPacket = BasePacket.createPacket(SetWeatherPacket.buildPacket(entry.Value.actorID, weather), true, false);
                        entry.Value.queuePacket(weatherPacket);
                    }                    
                }
            }
            */
        }

        /// <summary>
        /// We only use the default options for SendMessagePacket.
        /// May as well make it less unwieldly to view
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void sendMessage(ConnectedPlayer client, String message)
        {
            if (client != null)
               client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", message));
        }

        internal bool doCommand(string input, ConnectedPlayer client)
        {
            input.Trim();
            if (input.StartsWith("!"))
                input = input.Substring(1);

            String[] split = input.Split(' ');
            split = split.Select(temp => temp.ToLower()).ToArray(); // Ignore case on commands
            split = split.Where(temp => temp != "").ToArray(); // strips extra whitespace from commands

            // Debug
            //sendMessage(client, string.Join(",", split));
            
            if (split.Length >= 1)
            {
                #region !help
                if (split[0].Equals("help"))
                {
                    if (split.Length == 1)
                    {
                        sendMessage(client, Resources.CPhelp);
                    }
                    if (split.Length == 2)
                    {
                        if (split[1].Equals("mypos"))
                            sendMessage(client, Resources.CPmypos);
                        else if (split[1].Equals("music"))
                            sendMessage(client, Resources.CPmusic);
                        else if (split[1].Equals("warp"))
                            sendMessage(client, Resources.CPwarp);
                        else if (split[1].Equals("givecurrency"))
                            sendMessage(client, Resources.CPgivecurrency);
                        else if (split[1].Equals("giveitem"))
                            sendMessage(client, Resources.CPgiveitem);
                        else if (split[1].Equals("givekeyitem"))
                            sendMessage(client, Resources.CPgivekeyitem);
                        else if (split[1].Equals("removecurrency"))
                            sendMessage(client, Resources.CPremovecurrency);
                        else if (split[1].Equals("removeitem"))
                            sendMessage(client, Resources.CPremoveitem);
                        else if (split[1].Equals("removekeyitem"))
                            sendMessage(client, Resources.CPremovekeyitem);
                        else if (split[1].Equals("reloaditems"))
                            sendMessage(client, Resources.CPreloaditems);
                        else if (split[1].Equals("reloadzones"))
                            sendMessage(client, Resources.CPreloadzones);
                        /*
                        else if (split[1].Equals("property"))
                            sendMessage(client, Resources.CPproperty);
                        else if (split[1].Equals("property2"))
                            sendMessage(client, Resources.CPproperty2);
                        else if (split[1].Equals("sendpacket"))
                             sendMessage(client, Resources.CPsendpacket);
                        else if (split[1].Equals("setgraphic"))
                               sendMessage(client, Resources.CPsetgraphic);
                        */
                     }
                    if (split.Length == 3)
                    {
                        if(split[1].Equals("test"))
                        {
                            if (split[2].Equals("weather"))
                                sendMessage(client, Resources.CPtestweather);
                        }
                    }

                    return true;
                }
                #endregion

                #region !test
                else if (split[0].Equals("test"))
                {
                    if (split.Length == 1)
                    {
                        // catch invalid commands
                        sendMessage(client, Resources.CPhelp);
                    }
                    else if (split.Length >= 2)
                    {
                        #region !test weather
                        if (split[1].Equals("weather"))
                        {
                            try
                            {
                                doWeather(client, split[2], split[3]);
                                return true;
                            }
                            catch (Exception e)
                            {
                                Log.error("Could not change weather: " + e);
                            }
                        }
                        #endregion
                    }

                }
                #endregion

                #region !mypos
                else if (split[0].Equals("mypos"))
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
                #endregion

                #region !reloadzones
                else if (split[0].Equals("reloadzones"))
                {
                    if (client != null)
                    {
                        Log.info(String.Format("Got request to reset zone: {0}", client.getActor().zoneId));
                        client.getActor().zone.clear();
                        client.getActor().zone.addActorToZone(client.getActor());
                        client.getActor().sendInstanceUpdate();
                        client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Reseting zone {0}...", client.getActor().zoneId)), true, false));
                    }
                    Server.GetWorldManager().reloadZone(client.getActor().zoneId);
                    return true;
                }
                #endregion

                #region !reloaditems
                else if (split[0].Equals("reloaditems"))
                {
                    Log.info(String.Format("Got request to reload item gamedata"));
                    sendMessage(client, "Reloading Item Gamedata...");
                    gamedataItems.Clear();
                    gamedataItems = Database.getItemGamedata();
                    Log.info(String.Format("Loaded {0} items.", gamedataItems.Count));
                    sendMessage(client, String.Format("Loaded {0} items.", gamedataItems.Count));
                    return true;
                }
                #endregion

                #region !sendpacket
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
                #endregion

                #region !graphic
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
                #endregion

                #region !giveitem
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
                #endregion

                #region !removeitem
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
                #endregion

                #region !givekeyitem
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
                #endregion

                #region !removekeyitem
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
                #endregion

                #region !givecurrency
                else if (split[0].Equals("givecurrency"))
                {
                    try
                    {
                        if (split.Length == 2)
                            giveCurrency(client, ITEM_GIL, Int32.Parse(split[1]));
                        else if (split.Length == 3)
                            giveCurrency(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not give currency.");
                    }
                }
                #endregion

                #region !removecurrency
                else if (split[0].Equals("removecurrency"))
                {
                    if (split.Length < 2)
                        return false;

                    try
                    {
                        if (split.Length == 2)
                            removeCurrency(client, ITEM_GIL, Int32.Parse(split[1]));
                        else if (split.Length == 3)
                            removeCurrency(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not remove currency.");
                    }
                }
                #endregion

                #region !music
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
                #endregion

                #region !warp
                else if (split[0].Equals("warp"))
                {
                    parseWarp(client, split);
                    return true;
                }
                #endregion

                #region !property
                else if (split[0].Equals("property"))
                {
                    if (split.Length == 4)
                    {
                        changeProperty(Utils.MurmurHash2(split[1], 0), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion

                #region !property2
                else if (split[0].Equals("property2"))
                {
                    if (split.Length == 4)
                    {
                        changeProperty(Convert.ToUInt32(split[1], 16), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion
            }
            return false;
        }
    }

}

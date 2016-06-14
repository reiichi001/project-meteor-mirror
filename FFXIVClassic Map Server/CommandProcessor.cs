using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.Properties;

namespace FFXIVClassic_Map_Server
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

        public void SendPacket(ConnectedPlayer client, string path)
        {
            BasePacket packet = new BasePacket(path);

            if (client != null)
            {
                packet.ReplaceActorID(client.actorID);
                client.QueuePacket(packet);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    packet.ReplaceActorID(entry.Value.actorID);
                    entry.Value.QueuePacket(packet);
                }
            }
        }

        public void ChangeProperty(uint id, uint value, string target)
        {
            SetActorPropetyPacket ChangeProperty = new SetActorPropetyPacket(target);

            ChangeProperty.SetTarget(target);
            ChangeProperty.AddInt(id, value);
            ChangeProperty.AddTarget();

            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
            {
                SubPacket ChangePropertyPacket = ChangeProperty.BuildPacket((entry.Value.actorID), (entry.Value.actorID));

                BasePacket packet = BasePacket.CreatePacket(ChangePropertyPacket, true, false);
                packet.DebugPrintPacket();

                entry.Value.QueuePacket(packet);
            }
        }

        public void DoMusic(ConnectedPlayer client, string music)
        {
            ushort musicId;

            if (music.ToLower().StartsWith("0x"))
                musicId = Convert.ToUInt16(music, 16);
            else
                musicId = Convert.ToUInt16(music);

            if (client != null)
                client.QueuePacket(BasePacket.CreatePacket(SetMusicPacket.BuildPacket(client.actorID, musicId, 1), true, false));
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    BasePacket musicPacket = BasePacket.CreatePacket(SetMusicPacket.BuildPacket(entry.Value.actorID, musicId, 1), true, false);
                    entry.Value.QueuePacket(musicPacket);
                }
            }
        }

        /// <summary>
        /// Teleports player to a location on a predefined list
        /// </summary>
        /// <param name="client">The current player</param>
        /// <param name="id">Predefined list: &lt;ffxiv_database&gt;\server_zones_spawnlocations</param>
        public void DoWarp(ConnectedPlayer client, uint id)
        {
            WorldManager worldManager = Server.GetWorldManager();
            FFXIVClassic_Map_Server.WorldManager.ZoneEntrance ze = worldManager.GetZoneEntrance(id);

            if (ze == null)
                return;

            if (client != null)
                worldManager.DoZoneChange(client.GetActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    worldManager.DoZoneChange(entry.Value.GetActor(), ze.zoneId, ze.privateAreaName, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
                }
            }
        }

        public void DoWarp(ConnectedPlayer client, uint zoneId, string privateArea, byte spawnType, float x, float y, float z, float r)
        {
            WorldManager worldManager = Server.GetWorldManager();
            if (worldManager.GetZone(zoneId) == null)
            {
                if (client != null)
                    client.QueuePacket(BasePacket.CreatePacket(SendMessagePacket.BuildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Zone Does not exist or setting isn't valid."), true, false));
                Program.Log.Error("Zone Does not exist or setting isn't valid.");
            }

            if (client != null)
                worldManager.DoZoneChange(client.GetActor(), zoneId, privateArea, spawnType, x, y, z, r);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    worldManager.DoZoneChange(entry.Value.GetActor(), zoneId, privateArea, spawnType, x, y, z, r);
                }
            }
        }

        public void PrintPos(ConnectedPlayer client)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                client.QueuePacket(BasePacket.CreatePacket(SendMessagePacket.BuildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("{0}\'s position: ZoneID: {1}, X: {2}, Y: {3}, Z: {4}, Rotation: {5}", p.customDisplayName, p.zoneId, p.positionX, p.positionY, p.positionZ, p.rotation)), true, false));
            }
            else
            { 
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    Program.Log.Info("{0}\'s position: ZoneID: {1}, X: {2}, Y: {3}, Z: {4}, Rotation: {5}", p.customDisplayName, p.zoneId, p.positionX, p.positionY, p.positionZ, p.rotation);
                }
            }
        }

        private void SetGraphic(ConnectedPlayer client, uint slot, uint wId, uint eId, uint vId, uint cId)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GraphicChange(slot, wId, eId, vId, cId);
                p.SendAppearance();
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GraphicChange(slot, wId, eId, vId, cId);
                    p.SendAppearance();
                }
            }
        }

        private void GiveItem(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.NORMAL).AddItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.NORMAL).AddItem(itemId, quantity);
                }
            }
        }

        private void GiveItem(ConnectedPlayer client, uint itemId, int quantity, ushort type)
        {
            if (client != null)
            {
                Player p = client.GetActor();

                if (p.GetInventory(type) != null)
                    p.GetInventory(type).AddItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();

                    if (p.GetInventory(type) != null)
                        p.GetInventory(type).AddItem(itemId, quantity);
                }
            }
        }

        private void RemoveItem(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.NORMAL).RemoveItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.NORMAL).RemoveItem(itemId, quantity);
                }
            }
        }

        private void RemoveItem(ConnectedPlayer client, uint itemId, int quantity, ushort type)
        {
            if (client != null)
            {
                Player p = client.GetActor();

                if (p.GetInventory(type) != null)
                    p.GetInventory(type).RemoveItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();

                    if (p.GetInventory(type) != null)
                        p.GetInventory(type).RemoveItem(itemId, quantity);
                }
            }
        }

        private void GiveCurrency(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.CURRENCY).AddItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.CURRENCY).AddItem(itemId, quantity);
                }
            }
        }

        // TODO:  make RemoveCurrency() remove all quantity of a currency if quantity_to_remove > quantity_in_inventory instead of silently failing
        private void RemoveCurrency(ConnectedPlayer client, uint itemId, int quantity)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.CURRENCY).RemoveItem(itemId, quantity);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.CURRENCY).RemoveItem(itemId, quantity);
                }
            }
        }

        private void GiveKeyItem(ConnectedPlayer client, uint itemId)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.KEYITEMS).AddItem(itemId, 1);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.KEYITEMS).AddItem(itemId, 1);
                }
            }
        }

        private void RemoveKeyItem(ConnectedPlayer client, uint itemId)
        {
            if (client != null)
            {
                Player p = client.GetActor();
                p.GetInventory(Inventory.KEYITEMS).RemoveItem(itemId, 1);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.GetActor();
                    p.GetInventory(Inventory.KEYITEMS).RemoveItem(itemId, 1);
                }
            }
        }

        private void ParseWarp(ConnectedPlayer client, string[] split)
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

                DoWarp(client, zoneId);
            }
            else if (split.Length == 4)
            {
                #region !warp X Y Z
                if (split[1].StartsWith("@"))
                {
                    split[1] = split[1].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[1]))
                        split[1] = "0";

                    try { x = Single.Parse(split[1]) + client.GetActor().positionX; }
                    catch{return;}

                    split[1] = x.ToString();
                }
                if (split[2].StartsWith("@"))
                {
                    split[2] = split[2].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[2]))
                        split[2] = "0";

                    try { y = Single.Parse(split[2]) + client.GetActor().positionY; }
                    catch{return;}

                    split[2] = y.ToString();
                }
                if (split[3].StartsWith("@"))
                {
                    split[3] = split[3].Replace("@", string.Empty);

                    if (String.IsNullOrEmpty(split[3]))
                        split[3] = "0";

                    try { z = Single.Parse(split[3]) + client.GetActor().positionZ; }
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

                zoneId = client.GetActor().zoneId;
                r = client.GetActor().rotation;
                #endregion

                SendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                DoWarp(client, zoneId, privatearea, 0x00, x, y, z, r);
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

                SendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                DoWarp(client, zoneId, privatearea, 0x2, x, y, z, r);
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

                SendMessage(client, String.Format("Warping to: ZoneID: {0} X: {1}, Y: {2}, Z: {3}", zoneId, x, y, z));
                DoWarp(client, zoneId, privatearea, 0x2, x, y, z, r);
            }
            else
                return; // catch any invalid warps here
        }

        private void DoWeather(ConnectedPlayer client, string weatherID, string value)
        {
            ushort weather = Convert.ToUInt16(weatherID);

            if (client != null)
            {
                client.QueuePacket(BasePacket.CreatePacket(SetWeatherPacket.BuildPacket(client.actorID, weather, Convert.ToUInt16(value)), true, false));
            }

            /*
             * WIP: Change weather serverside, currently only clientside
             * 
            uint currentZoneID;
            if (client != null)
            {
                currentZoneID = client.GetActor().zoneId;

                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    // Change the weather for everyone in the same zone
                    if (currentZoneID == entry.Value.GetActor().zoneId)
                    {
                        BasePacket weatherPacket = BasePacket.CreatePacket(SetWeatherPacket.BuildPacket(entry.Value.actorID, weather), true, false);
                        entry.Value.QueuePacket(weatherPacket);
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
        private void SendMessage(ConnectedPlayer client, String message)
        {
            if (client != null)
               client.GetActor().QueuePacket(SendMessagePacket.BuildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", message));
        }

        internal bool DoCommand(string input, ConnectedPlayer client)
        {
            input.Trim();
            if (input.StartsWith("!"))
                input = input.Substring(1);

            String[] split = input.Split(' ');
            split = split.Select(temp => temp.ToLower()).ToArray(); // Ignore case on commands
            split = split.Where(temp => temp != "").ToArray(); // strips extra whitespace from commands

            // Debug
            //SendMessage(client, string.Join(",", split));
            
            if (split.Length >= 1)
            {
                #region !help
                if (split[0].Equals("help"))
                {
                    if (split.Length == 1)
                    {
                        SendMessage(client, Resources.CPhelp);
                    }
                    if (split.Length == 2)
                    {
                        if (split[1].Equals("mypos"))
                            SendMessage(client, Resources.CPmypos);
                        else if (split[1].Equals("music"))
                            SendMessage(client, Resources.CPmusic);
                        else if (split[1].Equals("warp"))
                            SendMessage(client, Resources.CPwarp);
                        else if (split[1].Equals("givecurrency"))
                            SendMessage(client, Resources.CPgivecurrency);
                        else if (split[1].Equals("giveitem"))
                            SendMessage(client, Resources.CPgiveitem);
                        else if (split[1].Equals("givekeyitem"))
                            SendMessage(client, Resources.CPgivekeyitem);
                        else if (split[1].Equals("removecurrency"))
                            SendMessage(client, Resources.CPremovecurrency);
                        else if (split[1].Equals("removeitem"))
                            SendMessage(client, Resources.CPremoveitem);
                        else if (split[1].Equals("removekeyitem"))
                            SendMessage(client, Resources.CPremovekeyitem);
                        else if (split[1].Equals("reloaditems"))
                            SendMessage(client, Resources.CPreloaditems);
                        else if (split[1].Equals("reloadzones"))
                            SendMessage(client, Resources.CPreloadzones);
                        /*
                        else if (split[1].Equals("property"))
                            SendMessage(client, Resources.CPproperty);
                        else if (split[1].Equals("property2"))
                            SendMessage(client, Resources.CPproperty2);
                        else if (split[1].Equals("sendpacket"))
                             SendMessage(client, Resources.CPsendpacket);
                        else if (split[1].Equals("setgraphic"))
                               SendMessage(client, Resources.CPsetgraphic);
                        */
                     }
                    if (split.Length == 3)
                    {
                        if(split[1].Equals("test"))
                        {
                            if (split[2].Equals("weather"))
                                SendMessage(client, Resources.CPtestweather);
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
                        SendMessage(client, Resources.CPhelp);
                    }
                    else if (split.Length >= 2)
                    {
                        #region !test weather
                        if (split[1].Equals("weather"))
                        {
                            try
                            {
                                DoWeather(client, split[2], split[3]);
                                return true;
                            }
                            catch (Exception e)
                            {
                                Program.Log.Error("Could not Change weather: " + e);
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
                        PrintPos(client);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not load packet: " + e);
                    }
                }
                #endregion

                #region !reloadzones
                else if (split[0].Equals("reloadzones"))
                {
                    if (client != null)
                    {
                        Program.Log.Info("Got request to reset zone: {0}", client.GetActor().zoneId);
                        client.GetActor().zone.Clear();
                        client.GetActor().zone.AddActorToZone(client.GetActor());
                        client.GetActor().SendInstanceUpdate();
                        client.QueuePacket(BasePacket.CreatePacket(SendMessagePacket.BuildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Reseting zone {0}...", client.GetActor().zoneId)), true, false));
                    }
                    Server.GetWorldManager().ReloadZone(client.GetActor().zoneId);
                    return true;
                }
                #endregion

                #region !reloaditems
                else if (split[0].Equals("reloaditems"))
                {
                    Program.Log.Info("Got request to reload item gamedata");
                    SendMessage(client, "Reloading Item Gamedata...");
                    gamedataItems.Clear();
                    gamedataItems = Database.GetItemGamedata();
                    Program.Log.Info("Loaded {0} items.", gamedataItems.Count);
                    SendMessage(client, String.Format("Loaded {0} items.", gamedataItems.Count));
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
                        SendPacket(client, "./packets/" + split[1]);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not load packet: " + e);
                    }
                }
                #endregion

                #region !graphic
                else if (split[0].Equals("graphic"))
                {
                    try
                    {
                        if (split.Length == 6)
                            SetGraphic(client, UInt32.Parse(split[1]), UInt32.Parse(split[2]), UInt32.Parse(split[3]), UInt32.Parse(split[4]), UInt32.Parse(split[5]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not give item.");
                    }
                }
                #endregion

                #region !giveitem
                else if (split[0].Equals("giveitem"))
                {
                    try
                    {
                        if (split.Length == 2)
                            GiveItem(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            GiveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        else if (split.Length == 4)
                            GiveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]), UInt16.Parse(split[3]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not give item.");
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
                            RemoveItem(client, UInt32.Parse(split[1]), 1);
                        else if (split.Length == 3)
                            RemoveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        else if (split.Length == 4)
                            RemoveItem(client, UInt32.Parse(split[1]), Int32.Parse(split[2]), UInt16.Parse(split[3]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not remove item.");
                    }
                }
                #endregion

                #region !givekeyitem
                else if (split[0].Equals("givekeyitem"))
                {
                    try
                    {
                        if (split.Length == 2)
                            GiveKeyItem(client, UInt32.Parse(split[1]));
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not give keyitem.");
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
                            RemoveKeyItem(client, UInt32.Parse(split[1]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not remove keyitem.");
                    }
                }
                #endregion

                #region !givecurrency
                else if (split[0].Equals("givecurrency"))
                {
                    try
                    {
                        if (split.Length == 2)
                            GiveCurrency(client, ITEM_GIL, Int32.Parse(split[1]));
                        else if (split.Length == 3)
                            GiveCurrency(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not give currency.");
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
                            RemoveCurrency(client, ITEM_GIL, Int32.Parse(split[1]));
                        else if (split.Length == 3)
                            RemoveCurrency(client, UInt32.Parse(split[1]), Int32.Parse(split[2]));
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not remove currency.");
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
                        DoMusic(client, split[1]);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("Could not Change music: " + e);
                    }
                }
                #endregion

                #region !warp
                else if (split[0].Equals("warp"))
                {
                    ParseWarp(client, split);
                    return true;
                }
                #endregion

                #region !property
                else if (split[0].Equals("property"))
                {
                    if (split.Length == 4)
                    {
                        ChangeProperty(Utils.MurmurHash2(split[1], 0), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion

                #region !property2
                else if (split[0].Equals("property2"))
                {
                    if (split.Length == 4)
                    {
                        ChangeProperty(Convert.ToUInt32(split[1], 16), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion
            }
            return false;
        }
    }

}

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
    class CommandProcessor
    {
        private Dictionary<uint, ConnectedPlayer> mConnectedPlayerList;
        private static WorldManager mWorldManager = Server.getWorldManager();
        private static Dictionary<uint, Item> gamedataItems = Server.getItemGamedataList();

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

        public void doWarp(ConnectedPlayer client, string zone, string privateArea, string sx, string sy, string sz)
        {
            uint zoneId;
            float x,y,z;

            if (zone.ToLower().StartsWith("0x"))
                zoneId = Convert.ToUInt32(zone, 16);
            else
                zoneId = Convert.ToUInt32(zone);

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
                mWorldManager.DoZoneChange(client.getActor(), zoneId, privateArea, 0x2, x, y, z, 0.0f);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    mWorldManager.DoZoneChange(entry.Value.getActor(), zoneId, privateArea, 0x2, x, y, z, 0.0f);
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

        internal bool doCommand(string input, ConnectedPlayer client)
        {
            input.Trim();
            if (input.StartsWith("!"))
                input = input.Substring(1);

            String[] split = input.Split(' ');
            split = split.Select(temp => temp.ToLower()).ToArray(); // Ignore case on commands


            // Debug
            //            if (client != null)
            //                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
            //                string.Join(",", split)
            //            ));

            if (split.Length >= 1)
            {

                if (split[0].Equals("help"))
                {
                    if (split.Length == 1)
                    {
                        if (client != null)
                            client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Use !help (command) for details\n\nAvailable commands:\nStandard: mypos, music, warp\nServer Administration: givecurrency, giveitem, givekeyitem, removecurrency, removekeyitem, reloaditems, resetzone\nDebug: property, property2, sendpacket, setgraphic"
                            ));
                    }
                    if (split.Length == 2)
                    {
                        if (split[1].Equals("mypos"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Prints out your current location\n\n*Note: The X/Y/Z coordinates do not correspond to the coordinates listed in the in-game map, they are based on the underlying game data"
                            ));
                        }
                        else if (split[1].Equals("music"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Changes the currently playing background music\n\n*Syntax: music <music id>\n<music id> is the key item's specific id as defined in the server database"
                            ));
                        }
                        else if (split[1].Equals("warp"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Teleports the player to the specified location\n\n*Syntax:\twarp <location list>\n\twarp <zone id> <X coordinate> <Y coordinate> <Z coordinate>\n\twarp <zone id> <instance> <X coordinate> <Y coordinate> <Z coordinate>\n<location list> is a pre-defined list of locations from the server database\n<instance> is an instanced copy of the desired zone that's only visible to the current player"
                            ));
                        }
                        else if (split[1].Equals("givecurrency"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Adds the specified currency to the current player's inventory\n\n*Syntax:\tgivecurrency <quantity>\n\tgivecurrency <quantity> <type>\n<type> is the specific type of currency desired, defaults to gil if no type specified"
                            ));
                        }
                        else if (split[1].Equals("giveitem"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Adds the specified items to the current player's inventory\n\n*Syntax:\tgiveitem <item id>\n\tgiveitem <item id> <quantity>\n\tgiveitem <item id> <quantity> <type>\n<item id> is the item's specific id as defined in the server database\n<type> is the type as defined in the server database (defaults to gil if not specified)"
                            ));
                        }
                        else if (split[1].Equals("givekeyitem"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Adds the specified key item to the current player's inventory\n\n*Syntax: givekeyitem <item id>\n<item id> is the key item's specific id as defined in the server database"
                            ));
                        }
                        else if (split[1].Equals("removecurrency"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Removes the specified currency from the current player's inventory\n\n*Syntax:\tremovecurrency <quantity>\n\tremovecurrency <quantity> <type>\n<type> is the specific type of currency desired, defaults to gil if no type specified"
                            ));
                        }
                        else if (split[1].Equals("removeitem"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Removes the specified items to the current player's inventory\n\n*Syntax:\tremoveitem <itemid>\n\tremoveitem <itemid> <quantity>\n<item id> is the item's specific id as defined in the server database"
                            ));
                        }
                        else if (split[1].Equals("removekeyitem"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Removes the specified key item to the current player's inventory\n\n*Syntax: removekeyitem <itemid>\n<item id> is the key item's specific id as defined in the server database"
                            ));
                        }
                        else if (split[1].Equals("reloaditems"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Reloads the current item data from the database"
                            ));
                        }
                        else if (split[1].Equals("resetzone"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Reloads the current zone data from the server files"
                            ));
                        }
                        else if (split[1].Equals("property"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "\n*Syntax: property <value 1> <value 2> <value 3>"
                            ));
                        }
                        else if (split[1].Equals("property2"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "\n*Syntax: property2 <value 1> <value 2> <value 3>"
                            ));
                        }
                        else if (split[1].Equals("sendpacket"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Server sends a special packet to the client\n\n*Syntax: sendpacket <path to packet>\n<Path to packet> is the path to the packet, starting in <map server install location>\\packet"
                            ));
                        }
                        else if (split[1].Equals("setgraphic"))
                        {
                            if (client != null)
                                client.getActor().queuePacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "",
                                "Overrides the currently displayed character equipment in a specific slot\n\n*Note: Similar to Glamours in FFXIV:ARR, the overridden graphics are purely cosmetic, they do not affect the underlying stats of whatever is equipped on that slot\n\n*Syntax: sendpacket <slot> <wid> <eid> <vid> <cid>\n<w/e/v/c id> are as defined in the client game data"
                            ));
                        }
                    }

                    return true;
                }
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
                else if (split[0].Equals("resetzone"))
                {
                    if (client != null)
                    {
                        Log.info(String.Format("Got request to reset zone: {0}", client.getActor().zoneId));
                        client.getActor().zone.clear();
                        client.getActor().zone.addActorToZone(client.getActor());
                        client.getActor().sendInstanceUpdate();
                        client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Reseting zone {0}...", client.getActor().zoneId)), true, false));
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
                    else if (split.Length == 5)
                        doWarp(client, split[1], null, split[2], split[3], split[4]);
                    else if (split.Length == 6)
                        doWarp(client, split[1], split[2], split[3], split[4], split[5]);
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

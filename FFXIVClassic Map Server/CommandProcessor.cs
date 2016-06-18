using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets;
using System.IO;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;
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
            if (!input.Any())
                return false;

            input.Trim();
            input = input.StartsWith("!") ? input.Substring(1) : input;

            var split = input.Split('"')
                     .Select((str, index) => index % 2 == 0
                                           ? str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           : new String[] { str }
                             )
                     .SelectMany(str => str).ToArray();

            split = split.Select(temp => temp.ToLower()).ToArray(); // Ignore case on commands

            var cmd = split?.ElementAt(0);

            if (cmd.Any())
            {
                // if client isnt null, take player to be the player actor
                var player = client?.GetActor();

                if (cmd.Equals("help"))
                {
                    // if there's another string after this, take it as the command we want the description for
                    if (split.Length > 1)
                    {
                        LuaEngine.RunGMCommand(player, split[1], null, true);
                        return true;
                    }

                    // print out all commands
                    foreach (var str in Directory.GetFiles("./scripts/commands/gm/"))
                    {
                        var c = str.Replace(".lua", "");
                        c = c.Replace("./scripts/commands/gm/", "");

                        LuaEngine.RunGMCommand(player, c, null, true);
                    }
                    return true;
                }

                LuaEngine.RunGMCommand(player, cmd.ToString(), split.ToArray());
                return true;
            }
            // Debug
            //SendMessage(client, string.Join(",", split));

            if (split.Length >= 1)
            {
            
            #region !reloaditems
            if (split[0].Equals("reloaditems"))
                {
                    Program.Log.Info(String.Format("Got request to reload item gamedata"));
                    SendMessage(client, "Reloading Item Gamedata...");
                    gamedataItems.Clear();
                    gamedataItems = Database.GetItemGamedata();
                    Program.Log.Info(String.Format("Loaded {0} items.", gamedataItems.Count));
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

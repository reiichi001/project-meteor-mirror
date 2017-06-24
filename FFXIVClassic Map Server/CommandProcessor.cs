using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;

using System.IO;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server
{
    class CommandProcessor
    {
        private static Dictionary<uint, ItemData> gamedataItems = Server.GetGamedataItems();

        const UInt32 ITEM_GIL = 1000001;
      
        /// <summary>
        /// We only use the default options for SendMessagePacket.
        /// May as well make it less unwieldly to view
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void SendMessage(Session session, String message)
        {
            if (session != null)
                session.GetActor().QueuePacket(SendMessagePacket.BuildPacket(session.id, session.id, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", message));
        }

        internal bool DoCommand(string input, Session session)
        {
            if (!input.Any() || input.Equals(""))
                return false;

            input.Trim();
            input = input.StartsWith("!") ? input.Substring(1) : input;

            var split = input.Split('"')
                     .Select((str, index) => index % 2 == 0
                                           ? str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           : new String[] { str }
                             )
                     .SelectMany(str => str).ToArray();

            split = split.ToArray(); // Ignore case on commands

            var cmd = split[0];

            if (cmd.Any())
            {
                // if client isnt null, take player to be the player actor
                Player player = null;
                if (session != null)
                    player = session.GetActor();

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
            
                // TODO: reloadzones

            #region !reloaditems
            if (split[0].Equals("reloaditems"))
                {
                    Program.Log.Info(String.Format("Got request to reload item gamedata"));
                    SendMessage(session, "Reloading Item Gamedata...");
                    gamedataItems.Clear();
                    gamedataItems = Database.GetItemGamedata();
                    Program.Log.Info(String.Format("Loaded {0} items.", gamedataItems.Count));
                    SendMessage(session, String.Format("Loaded {0} items.", gamedataItems.Count));
                    return true;
                }
                #endregion

                #region !property
                else if (split[0].Equals("property"))
                {
                    if (split.Length == 4)
                    {
                       // ChangeProperty(Utils.MurmurHash2(split[1], 0), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion

                #region !property2
                else if (split[0].Equals("property2"))
                {
                    if (split.Length == 4)
                    {
                        //ChangeProperty(Convert.ToUInt32(split[1], 16), Convert.ToUInt32(split[2], 16), split[3]);
                    }
                    return true;
                }
                #endregion
            }
            return false;
        }
    }

}

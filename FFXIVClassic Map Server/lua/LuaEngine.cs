
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.lua
{
    class LuaEngine
    {
        const string FILEPATH_PLAYER = "./scripts/player.lua";
        const string FILEPATH_ZONE = "./scripts/zones/{0}/zone.lua";
        const string FILEPATH_COMMANDS = "./scripts/commands/{0}.lua";
        const string FILEPATH_DIRECTORS = "./scripts/directors/{0}.lua";
        const string FILEPATH_NPCS = "./scripts/zones/{0}/npcs/{1}.lua";

        public LuaEngine()
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        }

        public static List<LuaParam> DoActorInstantiate(Player player, Actor target)
        {
            string luaPath;

            if (target is Npc)
            {
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.GetName());
                if (File.Exists(luaPath))
                {
                    LuaScript script = LoadScript(luaPath);

                    if (script == null)
                        return null;

                    DynValue result = script.Call(script.Globals["init"], target);
                    List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
                    return lparams;
                }
                else
                {
                    SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
                    return null;
                }
            }

            return null;
        }

        public static Coroutine DoActorOnEventStarted(Player player, Actor target, EventStartPacket eventStart)
        {           
            string luaPath;

            if (target is Command)
            {
                luaPath = String.Format(FILEPATH_COMMANDS, target.GetName());
            }
            else if (target is Director)
            {
                luaPath = String.Format(FILEPATH_DIRECTORS, target.GetName());
            }
            else
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.GetName());

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return null;

                if (!script.Globals.Get("onEventStarted").IsNil())
                    return script.CreateCoroutine(script.Globals["onEventStarted"]).Coroutine;
                else
                    return null;
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
                return null;
            }

        }

        public static void DoActorOnSpawn(Player player, Npc target)
        {
            string luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.GetName());

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onSpawn").IsNil())
                    script.Call(script.Globals["onSpawn"], player, target);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }

        }

        public static void DoActorOnEventUpdated(Player player, Actor target, EventUpdatePacket eventUpdate)
        {
            if (target is Npc)
            {
                ((Npc)target).DoEventUpdate(player, eventUpdate);
                return;
            }

            string luaPath;

            if (target is Command)
                luaPath = String.Format(FILEPATH_COMMANDS, target.GetName());
            else if (target is Director)
                luaPath = String.Format(FILEPATH_DIRECTORS, target.GetName());
            else
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.GetName());

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Have to Do this to combine LuaParams
                List<Object> objects = new List<Object>();
                objects.Add(player);
                objects.Add(target);
                objects.Add(eventUpdate.val2);
                objects.AddRange(LuaUtils.CreateLuaParamObjectList(eventUpdate.luaParams));

                //Run Script
                if (!script.Globals.Get("onEventUpdate").IsNil())
                    script.Call(script.Globals["onEventUpdate"], objects.ToArray());
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }
        }

        public static void OnZoneIn(Player player)
        {
            string luaPath = String.Format(FILEPATH_ZONE, player.GetZone().actorId);

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onZoneIn").IsNil())
                    script.Call(script.Globals["onZoneIn"], player);
            }
        }

        public static void OnBeginLogin(Player player)
        {
            if (File.Exists(FILEPATH_PLAYER))
            {
                LuaScript script = LoadScript(FILEPATH_PLAYER);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onBeginLogin").IsNil())
                    script.Call(script.Globals["onBeginLogin"], player);
            }
        }

        public static void OnLogin(Player player)
        {
            if (File.Exists(FILEPATH_PLAYER))
            {
                LuaScript script = LoadScript(FILEPATH_PLAYER);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onLogin").IsNil())
                    script.Call(script.Globals["onLogin"], player);
            }
        }

        #region RunGMCommand
        public static void RunGMCommand(Player player, String cmd, string[] param, bool help = false)
        {
            // load from scripts/commands/gm/ directory
            var path = String.Format("./scripts/commands/gm/{0}.lua", cmd.ToLower());

            // check if the file exists
            if (File.Exists(path))
            {
                // load global functions
                LuaScript script = LoadGlobals();

                // see if this script has any syntax errors
                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error("LuaEngine.RunGMCommand: {0}.", e.Message);
                    return;
                }

                // can we run this script
                if (!script.Globals.Get("onTrigger").IsNil())
                {
                    // can i run this command
                    var permissions = 0;

                    // parameter types (string, integer, double, float)
                    var parameters = "";
                    var description = "!" + cmd + ": ";

                    // get the properties table
                    var res = script.Globals.Get("properties");

                    // make sure properties table exists
                    if (!res.IsNil())
                    {
                        try
                        {
                            // returns table if one is found
                            var table = res.Table;

                            // find each key/value pair
                            foreach (var pair in table.Pairs)
                            {
                                if (pair.Key.String == "permissions")
                                {
                                    permissions = (int)pair.Value.Number;
                                }
                                else if (pair.Key.String == "parameters")
                                {
                                    parameters = pair.Value.String;
                                }
                                else if (pair.Key.String == "description")
                                {
                                    description = pair.Value.String;
                                }
                            }
                        }
                        catch (Exception e) { LuaScript.Log.Error("LuaEngine.RunGMCommand: " + e.Message); return; }
                    }

                    // if this isnt a console command, make sure player exists
                    if (player != null)
                    {
                        if (permissions > 0 && !player.isGM)
                        {
                            Program.Log.Info("LuaEngine.RunGMCommand: {0}'s GM level is too low to use command {1}.", player.actorName, cmd);
                            return;
                        }
                        // i hate to do this, but cant think of a better way to keep !help
                        else if (help)
                        {
                            player.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, String.Format("[Commands] [{0}]", cmd), description);
                            return;
                        }
                    }
                    else if (help)
                    {
                        LuaScript.Log.Info("[Commands] [{0}]: {1}", cmd, description);
                        return;
                    }

                    // we'll push our lua params here
                    List<object> LuaParam = new List<object>();

                    var i = 0;
                    for (; i < parameters.Length; ++i)
                    {
                        try
                        {
                            // convert chat parameters to command parameters
                            switch (parameters[i])
                            {
                                case 'i':
                                    LuaParam.Add(Convert.ChangeType(param[i + 1], typeof(int)));
                                    continue;
                                case 'd':
                                    LuaParam.Add(Convert.ChangeType(param[i + 1], typeof(double)));
                                    continue;
                                case 'f':
                                    LuaParam.Add(Convert.ChangeType(param[i + 1], typeof(float)));
                                    continue;
                                case 's':
                                    LuaParam.Add(param[i + 1]);
                                    continue;
                                default:
                                    LuaScript.Log.Info("LuaEngine.RunGMCommand: {0} unknown parameter {1}.", path, parameters[i]);
                                    LuaParam.Add(param[i + 1]);
                                    continue;
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is IndexOutOfRangeException) break;
                            LuaParam.Add(param[i + 1]);
                        }
                    }

                    // the script can double check the player exists, we'll push them anyways
                    LuaParam.Insert(0, player);
                    // push the arg count too
                    LuaParam.Insert(1, i);

                    // run the script
                    script.Call(script.Globals["onTrigger"], LuaParam.ToArray());
                    return;
                }
            }
            LuaScript.Log.Error("LuaEngine.RunGMCommand: Unable to find script {0}", path);
            return;
        }
        #endregion

        public static LuaScript LoadScript(string filename)
        {
            LuaScript script = LoadGlobals();

            try
            {
                script.DoFile(filename);
            }
            catch (SyntaxErrorException e)
            {
                Program.Log.Error("LUAERROR: {0}.", e.DecoratedMessage);
                return null;
            }
            return script;
        }

        public static LuaScript LoadGlobals(LuaScript script = null)
        {
            script = script ?? new LuaScript();

            // register and load all global functions here
            ((FileSystemScriptLoader)script.Options.ScriptLoader).ModulePaths = FileSystemScriptLoader.UnpackStringPaths("./scripts/?;./scripts/?.lua");
            script.Globals["GetWorldManager"] = (Func<WorldManager>)Server.GetWorldManager;
            script.Globals["GetStaticActor"] = (Func<string, Actor>)Server.GetStaticActors;
            script.Globals["GetWorldMaster"] = (Func<Actor>)Server.GetWorldManager().GetActor;
            script.Globals["GetItemGamedata"] = (Func<uint, Item>)Server.GetItemGamedata;

            script.Options.DebugPrint = s => { Program.Log.Debug(s); };
            return script;
        }

        public static void SendError(Player player, string message)
        {
            List<SubPacket> SendError = new List<SubPacket>();
            SendError.Add(EndEventPacket.BuildPacket(player.actorId, player.currentEventOwner, player.currentEventName));
            player.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", message);
            player.playerSession.QueuePacket(BasePacket.CreatePacket(SendError, true, false));
        }


        internal static void DoDirectorOnTalked(Director director, Player player, Npc npc)
        {
            string luaPath = String.Format(FILEPATH_DIRECTORS, director.GetName());

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onTalked").IsNil())
                    script.Call(script.Globals["onTalked"], player, npc);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for director {0}.", director.GetName()));
            }
        }

        internal static void DoDirectorOnCommand(Director director, Player player, Command command)
        {
            string luaPath = String.Format(FILEPATH_DIRECTORS, director.GetName());

            if (File.Exists(luaPath))
            {
                LuaScript script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onCommand").IsNil())
                    script.Call(script.Globals["onCommand"], player, command);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for director {0}.", director.GetName()));
            }
        }
    }
}

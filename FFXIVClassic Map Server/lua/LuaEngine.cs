
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
using FFXIVClassic_Map_Server.actors.area;
using System.Threading;

namespace FFXIVClassic_Map_Server.lua
{
    class LuaEngine
    {
        const string FILEPATH_PLAYER = "./scripts/player.lua";
        const string FILEPATH_ZONE = "./scripts/unique/{0}/zone.lua";
        const string FILEPATH_COMMANDS = "./scripts/commands/{0}.lua";
        const string FILEPATH_DIRECTORS = "./scripts/directors/{0}.lua";
        const string FILEPATH_NPCS = "./scripts/unique/{0}/{1}/{2}.lua";

        private static LuaEngine mThisEngine;
        private Dictionary<Coroutine, ulong> mSleepingOnTime = new Dictionary<Coroutine, ulong>();
        private Dictionary<string, List<Coroutine>> mSleepingOnSignal = new Dictionary<string, List<Coroutine>>();
        private Dictionary<uint, Coroutine> mSleepingOnPlayerEvent = new Dictionary<uint, Coroutine>();

        private Timer luaTimer;


        private LuaEngine()
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

            luaTimer = new Timer(new TimerCallback(PulseSleepingOnTime),
                           null, TimeSpan.Zero, TimeSpan.FromMilliseconds(50));
        }

        public static LuaEngine GetInstance()
        {
            if (mThisEngine == null)
                mThisEngine = new LuaEngine();

            return mThisEngine;
        }

        public void AddWaitCoroutine(Coroutine coroutine, float seconds)
        {
            ulong time = Utils.MilisUnixTimeStampUTC() + (ulong)(seconds * 1000);
            mSleepingOnTime.Add(coroutine, time);
        }

        public void AddWaitSignalCoroutine(Coroutine coroutine, string signal)
        {
            if (!mSleepingOnSignal.ContainsKey(signal))
                mSleepingOnSignal.Add(signal, new List<Coroutine>());
            mSleepingOnSignal[signal].Add(coroutine);
        }

        public void AddWaitEventCoroutine(Player player, Coroutine coroutine)
        {
            if (!mSleepingOnPlayerEvent.ContainsKey(player.actorId))
                mSleepingOnPlayerEvent.Add(player.actorId, coroutine);
        }

        public void PulseSleepingOnTime(object state)
        {
            ulong currentTime = Utils.MilisUnixTimeStampUTC();
            List<Coroutine> mToAwake = new List<Coroutine>();

            foreach (KeyValuePair<Coroutine, ulong> entry in mSleepingOnTime)
            {
                if (entry.Value <= currentTime)
                    mToAwake.Add(entry.Key);
            }

            foreach (Coroutine key in mToAwake)
            {
                mSleepingOnTime.Remove(key);
                DynValue value = key.Resume();
                ResolveResume(null, key, value);
            }
        }

        public void OnSignal(string signal)
        {
            List<Coroutine> mToAwake = new List<Coroutine>();

            if (mSleepingOnSignal.ContainsKey(signal))
            {
                mToAwake.AddRange(mSleepingOnSignal[signal]);
                mSleepingOnSignal.Remove(signal);
            }

            foreach (Coroutine key in mToAwake)
            { 
                DynValue value = key.Resume();
                ResolveResume(null, key, value);
            }
        }

        public void OnEventUpdate(Player player, List<LuaParam> args)
        {
            if (mSleepingOnPlayerEvent.ContainsKey(player.actorId))
            {
                Coroutine coroutine = mSleepingOnPlayerEvent[player.actorId];                
                mSleepingOnPlayerEvent.Remove(player.actorId);               
                DynValue value = coroutine.Resume(LuaUtils.CreateLuaParamObjectList(args));
                ResolveResume(null, coroutine, value);
            }
            else
                player.EndEvent();
        }

        private static string GetScriptPath(Actor target)
        {
            if (target is Player)
            {
                return String.Format(FILEPATH_PLAYER);
            }
            else if (target is Npc)
            {
                return null;
            }
            else if (target is Command)
            {
                return String.Format(FILEPATH_COMMANDS, target.GetName());
            }
            else if (target is Director)
            {
                return String.Format(FILEPATH_DIRECTORS, ((Director)target).GetScriptPath());
            }
            else if (target is Area)
            {
                return String.Format(FILEPATH_ZONE, ((Zone)target).zoneName);
            }
            else
                return "";
        }

        private List<LuaParam> CallLuaFunctionNpcForReturn(Player player, Npc target, string funcName, params object[] args)
        {
            LuaScript parent = null, child = null;

            if (File.Exists("./scripts/base/" + target.classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + target.classPath + ".lua");

            Area area = target.zone;
            if (area is PrivateArea)
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/privatearea/{1}/{2}/{3}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/privatearea/{1}/{2}/{3}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), target.className, target.GetUniqueId()));
            }
            else
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId()));
            }

            if (parent == null && child == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }

            //Run Script
            DynValue result;

            if (child != null && child.Globals[funcName] != null)
                result = child.Call(child.Globals[funcName], this);
            else if (parent != null && parent.Globals[funcName] != null)
                result = parent.Call(parent.Globals[funcName], this);
            else
                return null;

            List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
            return lparams;
        }

        private void CallLuaFunctionNpc(Player player, Npc target, string funcName, params object[] args)
        {
            object[] args2 = new object[args.Length + 2];
            Array.Copy(args, 0, args2, 2, args.Length);
            args2[0] = player;
            args2[1] = target;

            LuaScript parent = null, child = null;

            if (File.Exists("./scripts/base/" + target.classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + target.classPath + ".lua");

            Area area = target.zone;
            if (area is PrivateArea)
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/privatearea/{1}/{2}/{3}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/privatearea/{1}/{2}/{3}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), target.className, target.GetUniqueId()));
            }
            else
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId()));
            }

            if (parent == null && child == null)
            {
                LuaEngine.SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
                return;
            }

            //Run Script
            Coroutine coroutine = null;

            if (child != null && !child.Globals.Get(funcName).IsNil())
                coroutine = child.CreateCoroutine(child.Globals[funcName]).Coroutine;
            else if (parent.Globals.Get(funcName) != null && !parent.Globals.Get(funcName).IsNil())
                coroutine = parent.CreateCoroutine(parent.Globals[funcName]).Coroutine;

            if (coroutine != null)
            {
                DynValue value = coroutine.Resume(args2);
                ResolveResume(player, coroutine, value);
            }
        }

        public List<LuaParam> CallLuaFunctionForReturn(Player player, Actor target, string funcName, params object[] args)
        {
            //Need a seperate case for NPCs cause that child/parent thing.
            if (target is Npc)
                return CallLuaFunctionNpcForReturn(player, (Npc)target, funcName, args);

            string luaPath = GetScriptPath(target);
            LuaScript script = LoadScript(luaPath);
            if (script != null)
            {
                if (!script.Globals.Get(funcName).IsNil())
                {                    
                    //Run Script
                    DynValue result = script.Call(script.Globals[funcName], this);                    
                    List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
                    return lparams;
                }
                else
                {
                    SendError(player, String.Format("ERROR: Could not find function '{0}' for actor {1}.", funcName, target.GetName()));
                }
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }
            return null;
        }

        public void CallLuaFunction(Player player, Actor target, string funcName, params object[] args)
        {
            //Need a seperate case for NPCs cause that child/parent thing.
            if (target is Npc)
            {
                CallLuaFunctionNpc(player, (Npc)target, funcName, args);
                return;
            }

            object[] args2 = new object[args.Length + 2];
            Array.Copy(args, 0, args2, 2, args.Length);
            args2[0] = player;
            args2[1] = target;

            string luaPath = GetScriptPath(target);
            LuaScript script = LoadScript(luaPath);
            if (script != null)
            {
                if (!script.Globals.Get(funcName).IsNil())
                {
                    Coroutine coroutine = script.CreateCoroutine(script.Globals[funcName]).Coroutine;
                    DynValue value = coroutine.Resume(args2);
                    ResolveResume(player, coroutine, value);
                }
                else
                {
                    SendError(player, String.Format("ERROR: Could not find function '{0}' for actor {1}.", funcName, target.GetName()));
                }
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }            
        }

        public void EventStarted(Player player, Actor target, EventStartPacket eventStart)
        {
            List<LuaParam> lparams = eventStart.luaParams;
            lparams.Insert(0, new LuaParam(2, eventStart.triggerName));
            if (mSleepingOnPlayerEvent.ContainsKey(player.actorId))
            {
                Coroutine coroutine = mSleepingOnPlayerEvent[player.actorId];                
                mSleepingOnPlayerEvent.Remove(player.actorId);                
                DynValue value = coroutine.Resume();
                ResolveResume(null, coroutine, value);
            }
            else                
                CallLuaFunction(player, target, "onEventStarted", LuaUtils.CreateLuaParamObjectList(lparams));
        }

        private DynValue ResolveResume(Player player, Coroutine coroutine, DynValue value)
        {
            if (value == null || value.IsVoid())
                return value;

            if (value.String != null && value.String.Equals("_WAIT_EVENT"))
            {                
                GetInstance().AddWaitEventCoroutine(player, coroutine);      
            }
            else if (value.Tuple != null && value.Tuple.Length >= 1 && value.Tuple[0].String != null)
            {
                switch (value.Tuple[0].String)
                {
                    case "_WAIT_TIME":
                        GetInstance().AddWaitCoroutine(coroutine, (float)value.Tuple[1].Number);
                        break;
                    case "_WAIT_SIGNAL":
                        GetInstance().AddWaitSignalCoroutine(coroutine, (string)value.Tuple[1].String);
                        break;
                    case "_WAIT_EVENT":
                        GetInstance().AddWaitEventCoroutine((Player)value.Tuple[1].UserData.Object, coroutine);
                        break;
                    default:
                        return value;
                }
            }

            return value;
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
                    //script.Call(script.Globals["onTrigger"], LuaParam.ToArray());

                    Coroutine coroutine = script.CreateCoroutine(script.Globals["onTrigger"]).Coroutine;
                    DynValue value = coroutine.Resume(player, LuaParam.ToArray());
                    GetInstance().ResolveResume(player, coroutine, value);
                    return;
                }
            }
            LuaScript.Log.Error("LuaEngine.RunGMCommand: Unable to find script {0}", path);
            return;
        }
        #endregion

        public static LuaScript LoadScript(string path)
        {
            if (!File.Exists(path))
                return null;

            LuaScript script = LoadGlobals();

            try
            {
                script.DoFile(path);
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
            script.Globals["GetStaticActorById"] = (Func<uint, Actor>)Server.GetStaticActors;
            script.Globals["GetWorldMaster"] = (Func<Actor>)Server.GetWorldManager().GetActor;
            script.Globals["GetItemGamedata"] = (Func<uint, Item>)Server.GetItemGamedata;
            script.Globals["GetLuaInstance"] = (Func<LuaEngine>)LuaEngine.GetInstance;

            script.Options.DebugPrint = s => { Program.Log.Debug(s); };
            return script;
        }

        private static void SendError(Player player, string message)
        {
            if (player == null)
                return;
            List<SubPacket> SendError = new List<SubPacket>();
            SendError.Add(EndEventPacket.BuildPacket(player.actorId, player.currentEventOwner, player.currentEventName));
            player.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", message);
            player.playerSession.QueuePacket(BasePacket.CreatePacket(SendError, true, false));
        }
        
    }
}

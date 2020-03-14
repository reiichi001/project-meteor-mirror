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


using Meteor.Map.actors.director;
using Meteor.Map.Actors;
using Meteor.Map.dataobjects;
using Meteor.Map.packets.receive.events;
using Meteor.Map.packets.send;
using Meteor.Map.packets.send.events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using Meteor.Common;
using Meteor.Map.actors.area;
using System.Threading;
using Meteor.Map.actors.chara.ai;
using Meteor.Map.actors.chara.ai.controllers;

namespace Meteor.Map.lua
{
    class LuaEngine
    {
        public const string FILEPATH_PLAYER = "./scripts/player.lua";
        public const string FILEPATH_ZONE = "./scripts/unique/{0}/zone.lua";
        public const string FILEPATH_CONTENT = "./scripts/content/{0}.lua";
        public const string FILEPATH_COMMANDS = "./scripts/commands/{0}.lua";
        public const string FILEPATH_DIRECTORS = "./scripts/directors/{0}.lua";
        public const string FILEPATH_NPCS = "./scripts/unique/{0}/{1}/{2}.lua";
        public const string FILEPATH_QUEST = "./scripts/quests/{0}/{1}.lua";

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

        public void OnSignal(string signal, params object[] args)
        {
            List<Coroutine> mToAwake = new List<Coroutine>();

            if (mSleepingOnSignal.ContainsKey(signal))
            {
                mToAwake.AddRange(mSleepingOnSignal[signal]);
                mSleepingOnSignal.Remove(signal);
            }

            foreach (Coroutine key in mToAwake)
            {
                DynValue value = key.Resume(args);
                ResolveResume(null, key, value);
            }
        }

        public void OnEventUpdate(Player player, List<LuaParam> args)
        {
            if (mSleepingOnPlayerEvent.ContainsKey(player.actorId))
            {
                try
                {
                    Coroutine coroutine = mSleepingOnPlayerEvent[player.actorId];
                    mSleepingOnPlayerEvent.Remove(player.actorId);
                    DynValue value = coroutine.Resume(LuaUtils.CreateLuaParamObjectList(args));
                    ResolveResume(player, coroutine, value);
                }
                catch (ScriptRuntimeException e)
                {
                    LuaEngine.SendError(player, String.Format("OnEventUpdated: {0}", e.DecoratedMessage));
                    player.EndEvent();
                }
            }
            else
                player.EndEvent();
        }

        /// <summary> 
        /// // todo: this is dumb, should probably make a function for each action with different default return values
        /// or just make generic function and pass default value as first arg after functionName
        /// </summary>
        public static void CallLuaBattleFunction(Character actor, string functionName, params object[] args)
        {
            // todo: should use "scripts/zones/ZONE_NAME/battlenpcs/NAME.lua" instead of scripts/unique
            string path = "";

            // todo: should we call this for players too?
            if (actor is Player)
            {
                // todo: check this is correct
                path = FILEPATH_PLAYER;
            }
            else if (actor is Npc)
            {
                // todo: this is probably unnecessary as im not sure there were pets for players
                if (!(actor.aiContainer.GetController<PetController>()?.GetPetMaster() is Player))
                    path = String.Format("./scripts/unique/{0}/{1}/{2}.lua", actor.zone.zoneName, actor is BattleNpc ? "Monster" : "PopulaceStandard", ((Npc)actor).GetUniqueId());
            }
            // dont wanna throw an error if file doesnt exist
            if (File.Exists(path))
            {
                var script = LoadGlobals();
                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleFunction [{functionName}] {e.Message}");
                }
                DynValue res = new DynValue();

                if (!script.Globals.Get(functionName).IsNil())
                {
                    res = script.Call(script.Globals.Get(functionName), args);
                }
            }
        }

        public static int CallLuaStatusEffectFunction(Character actor, StatusEffect effect, string functionName, params object[] args)
        {
            // todo: this is stupid, load the actual effect name from db table
            string path = $"./scripts/effects/{effect.GetName()}.lua";

            if (File.Exists(path))
            {
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaStatusEffectFunction [{functionName}] {e.Message}");
                }
                DynValue res = new DynValue();

                if (!script.Globals.Get(functionName).IsNil())
                {
                    res = script.Call(script.Globals.Get(functionName), args);
                    if (res != null)
                        return (int)res.Number;
                }
            }
            else
            {
                Program.Log.Error($"LuaEngine.CallLuaStatusEffectFunction [{effect.GetName()}] Unable to find script {path}");
            }
            return -1;
        }

        public static int CallLuaBattleCommandFunction(Character actor, BattleCommand command, string folder, string functionName, params object[] args)
        {
            string path = $"./scripts/commands/{folder}/{command.name}.lua";

            if (File.Exists(path))
            {
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction [{functionName}] {e.Message}");
                }
                DynValue res = new DynValue();
                
                if (!script.Globals.Get(functionName).IsNil())
                {
                    res = script.Call(script.Globals.Get(functionName), args);
                    if (res != null)
                        return (int)res.Number;
                }
            }
            else
            {
                path = $"./scripts/commands/{folder}/default.lua";
                //Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction [{command.name}] Unable to find script {path}");
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction [{functionName}] {e.Message}");
                }
                DynValue res = new DynValue();
               // DynValue r = script.Globals.Get(functionName);

                if (!script.Globals.Get(functionName).IsNil())
                {
                    res = script.Call(script.Globals.Get(functionName), args);
                    if (res != null)
                        return (int)res.Number;
                }
            }
            return -1;
        }


        public static void LoadBattleCommandScript(BattleCommand command, string folder)
        {
            string path = $"./scripts/commands/{folder}/{command.name}.lua";

            if (File.Exists(path))
            {
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction {e.Message}");
                }
                command.script = script;
            }
            else
            {
                path = $"./scripts/commands/{folder}/default.lua";
                //Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction [{command.name}] Unable to find script {path}");
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction {e.Message}");
                }

                command.script = script;
            }
        }

        public static void LoadStatusEffectScript(StatusEffect effect)
        {
            string path = $"./scripts/effects/{effect.GetName()}.lua";

            if (File.Exists(path))
            {
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction {e.Message}");
                }
                effect.script = script;
            }
            else
            {
                path = $"./scripts/effects/default.lua";
                //Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction [{command.name}] Unable to find script {path}");
                var script = LoadGlobals();

                try
                {
                    script.DoFile(path);
                }
                catch (Exception e)
                {
                    Program.Log.Error($"LuaEngine.CallLuaBattleCommandFunction {e.Message}");
                }

                effect.script = script;
            }
        }


        public static string GetScriptPath(Actor target)
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
            else if (target is PrivateAreaContent)
            {
                return String.Format(FILEPATH_CONTENT, ((PrivateAreaContent)target).GetPrivateAreaName());
            }
            else if (target is Area)
            {
                return String.Format(FILEPATH_ZONE, ((Area)target).zoneName);
            }
            else if (target is Quest)
            {
                string initial = ((Quest)target).actorName.Substring(0, 3);
                string questName = ((Quest)target).actorName;
                return String.Format(FILEPATH_QUEST, initial, questName);
            }
            else
                return "";
        }

        private List<LuaParam> CallLuaFunctionNpcForReturn(Player player, Npc target, string funcName, bool optional, params object[] args)
        {
            object[] args2 = new object[args.Length + (player == null ? 1 : 2)];
            Array.Copy(args, 0, args2, (player == null ? 1 : 2), args.Length);
            if (player != null)
            {
                args2[0] = player;
                args2[1] = target;
            }
            else
                args2[0] = target;

            LuaScript parent = null, child = null;

            if (File.Exists("./scripts/base/" + target.classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + target.classPath + ".lua");

            Area area = target.zone;
            if (area is PrivateArea)
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/privatearea/{1}_{2}/{3}/{4}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), ((PrivateArea)area).GetPrivateAreaType(), target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/privatearea/{1}_{2}/{3}/{4}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), ((PrivateArea)area).GetPrivateAreaType(), target.className, target.GetUniqueId()));
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
                result = child.Call(child.Globals[funcName], args2);
            else if (parent != null && parent.Globals[funcName] != null)
                result = parent.Call(parent.Globals[funcName], args2);
            else
                return null;

            List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
            return lparams;
        }

        private void CallLuaFunctionNpc(Player player, Npc target, string funcName, bool optional, params object[] args)
        {
            object[] args2 = new object[args.Length + (player == null ? 1 : 2)];
            Array.Copy(args, 0, args2, (player == null ? 1 : 2), args.Length);
            if (player != null)
            {
                args2[0] = player;
                args2[1] = target;
            }
            else
                args2[0] = target;

            LuaScript parent = null, child = null;

            if (File.Exists("./scripts/base/" + target.classPath + ".lua"))
                parent = LuaEngine.LoadScript("./scripts/base/" + target.classPath + ".lua");

            Area area = target.zone;
            if (area is PrivateArea)
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/privatearea/{1}_{2}/{3}/{4}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), ((PrivateArea)area).GetPrivateAreaType(), target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/privatearea/{1}_{2}/{3}/{4}.lua", area.zoneName, ((PrivateArea)area).GetPrivateAreaName(), ((PrivateArea)area).GetPrivateAreaType(), target.className, target.GetUniqueId()));
            }
            else
            {
                if (File.Exists(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId())))
                    child = LuaEngine.LoadScript(String.Format("./scripts/unique/{0}/{1}/{2}.lua", area.zoneName, target.className, target.GetUniqueId()));
            }

            if (parent == null && child == null)
            {
                LuaEngine.SendError(player, String.Format("Could not find script for actor {0}.", target.GetName()));
                return;
            }

            //Run Script
            Coroutine coroutine = null;

            if (child != null && !child.Globals.Get(funcName).IsNil())
                coroutine = child.CreateCoroutine(child.Globals[funcName]).Coroutine;
            else if (parent != null && parent.Globals.Get(funcName) != null && !parent.Globals.Get(funcName).IsNil())
                coroutine = parent.CreateCoroutine(parent.Globals[funcName]).Coroutine;

            if (coroutine != null)
            {
                try
                {
                    DynValue value = coroutine.Resume(args2);
                    ResolveResume(player, coroutine, value);
                }
                catch (ScriptRuntimeException e)
                {
                    SendError(player, e.DecoratedMessage);
                }
            }
        }

        public List<LuaParam> CallLuaFunctionForReturn(Player player, Actor target, string funcName, bool optional, params object[] args)
        {
            //Need a seperate case for NPCs cause that child/parent thing.
            if (target is Npc)
                return CallLuaFunctionNpcForReturn(player, (Npc)target, funcName, optional, args);

            object[] args2 = new object[args.Length + (player == null ? 1 : 2)];
            Array.Copy(args, 0, args2, (player == null ? 1 : 2), args.Length);
            if (player != null)
            {
                args2[0] = player;
                args2[1] = target;
            }
            else
                args2[0] = target;

            string luaPath = GetScriptPath(target);
            LuaScript script = LoadScript(luaPath);
            if (script != null)
            {
                if (!script.Globals.Get(funcName).IsNil())
                {
                    //Run Script
                    DynValue result = script.Call(script.Globals[funcName], args2);
                    List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
                    return lparams;
                }
                else
                {
                    if (!optional)
                        SendError(player, String.Format("Could not find function '{0}' for actor {1}.", funcName, target.GetName()));
                }
            }
            else
            {
                if (!optional)
                    SendError(player, String.Format("Could not find script for actor {0}.", target.GetName()));
            }
            return null;
        }

        public List<LuaParam> CallLuaFunctionForReturn(string path, string funcName, bool optional, params object[] args)
        {
            string luaPath = path;
            LuaScript script = LoadScript(luaPath);
            if (script != null)
            {
                if (!script.Globals.Get(funcName).IsNil())
                {
                    //Run Script
                    DynValue result = script.Call(script.Globals[funcName], args);
                    List<LuaParam> lparams = LuaUtils.CreateLuaParamList(result);
                    return lparams;
                }
            }
            return null;
        }

        public void CallLuaFunction(Player player, Actor target, string funcName, bool optional, params object[] args)
        {
            //Need a seperate case for NPCs cause that child/parent thing.
            if (target is Npc)
            {
                CallLuaFunctionNpc(player, (Npc)target, funcName, optional, args);
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
                    try
                    {
                        Coroutine coroutine = script.CreateCoroutine(script.Globals[funcName]).Coroutine;
                        DynValue value = coroutine.Resume(args2);
                        ResolveResume(player, coroutine, value);
                    }
                    catch(Exception e)
                    {
                        player.SendMessage(0x20, "", e.Message);
                        player.EndEvent();

                    }
                }
                else
                {
                    if (!optional)
                        SendError(player, String.Format("Could not find function '{0}' for actor {1}.", funcName, target.GetName()));
                }
            }
            else
            {
                if (!(target is Area) && !optional)
                    SendError(player, String.Format("Could not find script for actor {0}.", target.GetName()));
            }
        }

        public void EventStarted(Player player, Actor target, EventStartPacket eventStart)
        {
            List<LuaParam> lparams = new List<LuaParam>();
            lparams.AddRange(eventStart.luaParams);
            lparams.Insert(0, new LuaParam(2, eventStart.eventName));
            if (mSleepingOnPlayerEvent.ContainsKey(player.actorId))
            {
                Coroutine coroutine = mSleepingOnPlayerEvent[player.actorId];
                mSleepingOnPlayerEvent.Remove(player.actorId);

                try
                {
                    DynValue value = coroutine.Resume();
                    ResolveResume(null, coroutine, value);
                }
                catch (ScriptRuntimeException e)
                {
                    LuaEngine.SendError(player, String.Format("OnEventStarted: {0}", e.DecoratedMessage));
                    player.EndEvent();
                }
            }
            else
            {
                if (target is Director)
                    ((Director)target).OnEventStart(player, LuaUtils.CreateLuaParamObjectList(lparams));
                else
                    CallLuaFunction(player, target, "onEventStarted", false, LuaUtils.CreateLuaParamObjectList(lparams));
            }
        }

        public DynValue ResolveResume(Player player, Coroutine coroutine, DynValue value)
        {
            if (value == null || value.IsVoid())
                return value;

            if (player != null && value.String != null && value.String.Equals("_WAIT_EVENT"))
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
            bool playerNull = player == null;

            if (playerNull)
            {
                if (param.Length >= 2 && param[1].Contains("\""))
                    player = Server.GetWorldManager().GetPCInWorld(param[1]);
                else if (param.Length > 2)
                    player = Server.GetWorldManager().GetPCInWorld(param[1] + param[2]);
            }

            if (playerNull && param.Length >= 3)
                player = Server.GetWorldManager().GetPCInWorld(param[1] + " " + param[2]);
            
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

                    var i = playerNull ? 2 : 0;
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
                    LuaParam.Insert(1, i - (playerNull ? 2 : 0));

                    // run the script                    
                    //script.Call(script.Globals["onTrigger"], LuaParam.ToArray());

                    // gm commands dont need to be coroutines?
                    try
                    {
                        Coroutine coroutine = script.CreateCoroutine(script.Globals["onTrigger"]).Coroutine;
                        DynValue value = coroutine.Resume(LuaParam.ToArray());
                        GetInstance().ResolveResume(player, coroutine, value);
                    }
                    catch (Exception e)
                    {
                        Program.Log.Error("LuaEngine.RunGMCommand: {0} - {1}", path, e.Message);
                    }
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
                Program.Log.Error("{0}.", e.DecoratedMessage);
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
            script.Globals["GetItemGamedata"] = (Func<uint, ItemData>)Server.GetItemGamedata;
            script.Globals["GetGuildleveGamedata"] = (Func<uint, GuildleveData>)Server.GetGuildleveGamedata;
            script.Globals["GetLuaInstance"] = (Func<LuaEngine>)LuaEngine.GetInstance;

            script.Options.DebugPrint = s => { Program.Log.Debug(s); };
            return script;
        }

        public static void SendError(Player player, string message)
        {
            message = "[LuaError] " + message;
            if (player == null)
                return;
            List<SubPacket> SendError = new List<SubPacket>();
            player.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", message);
            player.QueuePacket(EndEventPacket.BuildPacket(player.actorId, player.currentEventOwner, player.currentEventName, 0));
        }

    }
}

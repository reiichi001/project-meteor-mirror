using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.packets;
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
                    Script script = LoadScript(luaPath);

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

        public static void DoActorOnEventStarted(Player player, Actor target, EventStartPacket eventStart)
        {
            if (target is Npc)
            {
                ((Npc)target).DoEventStart(player, eventStart);
                return;
            }

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
                Script script = LoadScript(luaPath);

                if (script == null)
                    return;

                //Have to Do this to combine LuaParams
                List<Object> objects = new List<Object>();
                objects.Add(player);
                objects.Add(target);
                objects.Add(eventStart.triggerName);

                if (eventStart.luaParams != null)
                    objects.AddRange(LuaUtils.CreateLuaParamObjectList(eventStart.luaParams));

                //Run Script
                if (!script.Globals.Get("onEventStarted").IsNil())
                    script.Call(script.Globals["onEventStarted"], objects.ToArray());
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.GetName()));
            }
           
        }

        public static void DoActorOnSpawn(Player player, Npc target)
        {
            string luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.GetName());

            if (File.Exists(luaPath))
            {
                Script script = LoadScript(luaPath);

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
                Script script = LoadScript(luaPath);

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
                Script script = LoadScript(luaPath);

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
                Script script = LoadScript(FILEPATH_PLAYER);

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
                Script script = LoadScript(FILEPATH_PLAYER);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onLogin").IsNil())
                    script.Call(script.Globals["onLogin"], player);
            }
        }

        public static Script LoadScript(string filename)
        {
            Script script = new Script();
            ((FileSystemScriptLoader)script.Options.ScriptLoader).ModulePaths = FileSystemScriptLoader.UnpackStringPaths("./scripts/?;./scripts/?.lua");
            script.Globals["GetWorldManager"] = (Func<WorldManager>)Server.GetWorldManager;
            script.Globals["GetStaticActor"] = (Func<string, Actor>)Server.GetStaticActors;
            script.Globals["GetWorldMaster"] = (Func<Actor>)Server.GetWorldManager().GetActor;
            script.Globals["GetItemGamedata"] = (Func<uint, Item>)Server.GetItemGamedata;

            try
            {
                script.DoFile(filename);
            }
            catch(SyntaxErrorException e)
            {
                Program.Log.Error("LUAERROR: {0}.", e.DecoratedMessage);
                return null;
            }
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
                Script script = LoadScript(luaPath);

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
                Script script = LoadScript(luaPath);

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

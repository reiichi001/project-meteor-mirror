using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static List<LuaParam> doActorInstantiate(Player player, Actor target)
        {
            string luaPath;

            if (target is Npc)
            {
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());
                if (File.Exists(luaPath))
                {                    
                    Script script = loadScript(luaPath);

                    if (script == null)
                        return null;

                    DynValue result = script.Call(script.Globals["init"], target);
                    List<LuaParam> lparams = LuaUtils.createLuaParamList(result);
                    return lparams;
                }
                else
                {
                    SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    return null;
                }
            }

            return null;
        }       

        public static void doActorOnEventStarted(Player player, Actor target, EventStartPacket eventStart)
        {
            string luaPath;

            if (target is Command)
            {
                luaPath = String.Format(FILEPATH_COMMANDS, target.getName());
            }
            else if (target is Director)
            {
                luaPath = String.Format(FILEPATH_DIRECTORS, target.getName());
            }
            else 
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());

            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Have to do this to combine LuaParams
                List<Object> objects = new List<Object>();
                objects.Add(player);
                objects.Add(target);
                objects.Add(eventStart.triggerName);

                if (eventStart.luaParams != null)
                    objects.AddRange(LuaUtils.createLuaParamObjectList(eventStart.luaParams));

                //Run Script
                if (!script.Globals.Get("onEventStarted").IsNil())
                    script.Call(script.Globals["onEventStarted"], objects.ToArray());
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
            }
           
        }

        public static void doActorOnSpawn(Player player, Npc target)
        {
            string luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());

            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onSpawn").IsNil())
                    script.Call(script.Globals["onSpawn"], player, target);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
            }

        }

        public static void doActorOnEventUpdated(Player player, Actor target, EventUpdatePacket eventUpdate)
        {
            string luaPath; 

            if (target is Command)            
                luaPath = String.Format(FILEPATH_COMMANDS, target.getName());
            else if (target is Director)
                luaPath = String.Format(FILEPATH_DIRECTORS, target.getName());            
            else
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());

            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Have to do this to combine LuaParams
                List<Object> objects = new List<Object>();
                objects.Add(player);
                objects.Add(target);
                objects.Add(eventUpdate.val2);
                objects.AddRange(LuaUtils.createLuaParamObjectList(eventUpdate.luaParams));

                //Run Script
                if (!script.Globals.Get("onEventUpdate").IsNil())
                    script.Call(script.Globals["onEventUpdate"], objects.ToArray());
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
            }                  
        }

        public static void onZoneIn(Player player)
        {
            string luaPath = String.Format(FILEPATH_ZONE, player.getZone().actorId);
          
            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onZoneIn").IsNil())
                    script.Call(script.Globals["onZoneIn"], player);
            }            
        }

        public static void onBeginLogin(Player player)
        {
            if (File.Exists(FILEPATH_PLAYER))
            {
                Script script = loadScript(FILEPATH_PLAYER);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onBeginLogin").IsNil())
                    script.Call(script.Globals["onBeginLogin"], player);
            }
        }

        public static void onLogin(Player player)
        {
            if (File.Exists(FILEPATH_PLAYER))
            {
                Script script = loadScript(FILEPATH_PLAYER);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onLogin").IsNil())
                    script.Call(script.Globals["onLogin"], player);
            }
        }

        private static Script loadScript(string filename)
        {
            Script script = new Script();
            ((FileSystemScriptLoader)script.Options.ScriptLoader).ModulePaths = FileSystemScriptLoader.UnpackStringPaths("./scripts/?;./scripts/?.lua");
            script.Globals["getWorldManager"] = (Func<WorldManager>)Server.GetWorldManager;
            script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
            script.Globals["getWorldMaster"] = (Func<Actor>)Server.GetWorldManager().GetActor;
            script.Globals["getItemGamedata"] = (Func<uint, Item>)Server.getItemGamedata;

            try
            {
                script.DoFile(filename);
            }
            catch(SyntaxErrorException e)
            {
                Log.error(String.Format("LUAERROR: {0}.", e.DecoratedMessage));
                return null;
            }
            return script;
        }

        private static void SendError(Player player, string message)
        {
            List<SubPacket> sendError = new List<SubPacket>();
            sendError.Add(EndEventPacket.buildPacket(player.actorId, player.currentEventOwner, player.currentEventName));
            player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", message);
            player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
        }


        internal static void doDirectorOnTalked(Director director, Player player, Npc npc)
        {
            string luaPath = String.Format(FILEPATH_DIRECTORS, director.getName());

            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onTalked").IsNil())
                    script.Call(script.Globals["onTalked"], player, npc);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for director {0}.", director.getName()));
            }
        }

        internal static void doDirectorOnCommand(Director director, Player player, Command command)
        {
            string luaPath = String.Format(FILEPATH_DIRECTORS, director.getName());

            if (File.Exists(luaPath))
            {
                Script script = loadScript(luaPath);

                if (script == null)
                    return;

                //Run Script
                if (!script.Globals.Get("onCommand").IsNil())
                    script.Call(script.Globals["onCommand"], player, command);
            }
            else
            {
                SendError(player, String.Format("ERROR: Could not find script for director {0}.", director.getName()));
            }
        }
    }
}

using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
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
        const string FILEPATH_COMMANDS = "./scripts/command/{0}.lua";
        const string FILEPATH_NPCS = "./scripts/zones/{0}/npcs/{1}.lua";

        public LuaEngine()
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        }

        public List<LuaParam> doActorOnInstantiate(Player player, Actor target)
        {
            string luaPath;

            if (target is Npc)
            {
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());
                if (File.Exists(luaPath))
                {
                    Script script = new Script();
                    script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
                    script.DoFile(luaPath);
                    DynValue result = script.Call(script.Globals["onInstantiate"], player, target);
                    List<LuaParam> lparams = LuaUtils.createLuaParamList(result);
                    return lparams;
                }
                else
                {
                    List<SubPacket> sendError = new List<SubPacket>();
                    sendError.Add(EndEventPacket.buildPacket(player.actorId, player.playerSession.eventCurrentOwner, player.playerSession.eventCurrentStarter));
                    player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
                    return null;
                }
            }

            return null;
        }       

        public void doActorOnEventStarted(Player player, Actor target)
        {
            string luaPath;

            if (target is Command)
            {
                luaPath = String.Format(FILEPATH_COMMANDS, target.getName());
                if (File.Exists(luaPath))
                {
                    Script script = new Script();
                    script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
                    script.DoFile(luaPath);
                    DynValue result = script.Call(script.Globals["onEventStarted"], player, target);
                }
                else
                {
                    List<SubPacket> sendError = new List<SubPacket>();
                    sendError.Add(EndEventPacket.buildPacket(player.actorId, player.playerSession.eventCurrentOwner, player.playerSession.eventCurrentStarter));
                    player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
                }
            }
            else if (target is Npc)
            {
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());
                if (File.Exists(luaPath))
                {
                    Script script = new Script();
                    script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
                    script.DoFile(luaPath);
                    DynValue result = script.Call(script.Globals["onEventStarted"], player, target);
                }
                else
                {
                    List<SubPacket> sendError = new List<SubPacket>();
                    sendError.Add(EndEventPacket.buildPacket(player.actorId, player.playerSession.eventCurrentOwner, player.playerSession.eventCurrentStarter));
                    player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
                }
            }
        }

        public void doActorOnEventUpdated(Player player, Actor target, EventUpdatePacket eventUpdate)
        {
            string luaPath;

            if (target is Command)
            {
                luaPath = String.Format(FILEPATH_COMMANDS, target.getName());
                if (File.Exists(luaPath))
                {
                    Script script = new Script();
                    script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
                    script.DoFile(luaPath);
                    DynValue result = script.Call(script.Globals["onEventUpdate"], player, target, eventUpdate.step, eventUpdate.luaParams);
                }
                else
                {
                    List<SubPacket> sendError = new List<SubPacket>();
                    sendError.Add(EndEventPacket.buildPacket(player.actorId, player.playerSession.eventCurrentOwner, player.playerSession.eventCurrentStarter));
                    player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
                }
            }
            else if (target is Npc)
            {
                luaPath = String.Format(FILEPATH_NPCS, target.zoneId, target.getName());
                if (File.Exists(luaPath))
                {
                    Script script = new Script();
                    script.Globals["getStaticActor"] = (Func<string, Actor>)Server.getStaticActors;
                    script.DoFile(luaPath);

                    //Have to do this to combine LuaParams
                    List<Object> objects = new List<Object>();
                    objects.Add(player);
                    objects.Add(target);
                    objects.Add(eventUpdate.step);
                    objects.AddRange(LuaUtils.createLuaParamObjectList(eventUpdate.luaParams));

                    //Run Script
                    DynValue result = script.Call(script.Globals["onEventUpdate"], objects.ToArray());
                }
                else
                {
                    List<SubPacket> sendError = new List<SubPacket>();
                    sendError.Add(EndEventPacket.buildPacket(player.actorId, player.playerSession.eventCurrentOwner, player.playerSession.eventCurrentStarter));
                    player.sendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "", String.Format("ERROR: Could not find script for actor {0}.", target.getName()));
                    player.playerSession.queuePacket(BasePacket.createPacket(sendError, true, false));
                }
            }
        }
    }
}

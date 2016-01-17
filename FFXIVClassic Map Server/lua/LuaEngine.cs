using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.actors;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.dataobjects.chara.npc;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using NLua;
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
        const string FILEPATH_EVENTS = "./scripts/talk/{0}.lua";

        Lua lstate = new Lua();

        public LuaEngine()
        {          
        }


        public void doEventStart(ConnectedPlayer player, Actor target, EventStartPacket packet)
        {
            string luaPath;

            if (target is Command)
                luaPath = String.Format(FILEPATH_COMMANDS, target.getName());
            else if (target is Npc)
                luaPath = String.Format(FILEPATH_EVENTS, target.getName());
            else
                luaPath = "";

            if (File.Exists(luaPath))
            {
                lstate.DoFile(luaPath);
                var eventStarted = lstate["eventStarted"] as LuaFunction;
                eventStarted.Call(new LuaPlayer(player), player.eventCurrentOwner, LuaUtils.createLuaParamObjectList(packet.luaParams));
            }
            else
            {
                List<SubPacket> sendError = new List<SubPacket>();
                sendError.Add(EndEventPacket.buildPacket(player.actorID, player.eventCurrentOwner, player.eventCurrentStarter));
                sendError.Add(SendMessagePacket.buildPacket(player.actorID, player.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "ERROR: Could not find script for event"));
                player.queuePacket(BasePacket.createPacket(sendError, true, false));
            }
        }

        public void doEventUpdated(ConnectedPlayer player, Actor target, EventUpdatePacket packet)
        {
            string luaPath = String.Format(FILEPATH_EVENTS, ((Command)target).getName());

            if (File.Exists(luaPath))
            {
                lstate.DoFile(luaPath);
                var eventStarted = lstate["eventUpdated"] as LuaFunction;
                eventStarted.Call(new LuaPlayer(player), player.eventCurrentOwner, packet.step, LuaUtils.createLuaParamObjectList(packet.luaParams));
            }
            else
            {
                List<SubPacket> sendError = new List<SubPacket>();
                sendError.Add(EndEventPacket.buildPacket(player.actorID, player.eventCurrentOwner, player.eventCurrentStarter));
                sendError.Add(SendMessagePacket.buildPacket(player.actorID, player.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "ERROR: Could not find script for event"));
                player.queuePacket(BasePacket.createPacket(sendError, true, false));
            }
        }
        
    }
}

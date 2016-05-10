using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.lua
{
    [MoonSharpUserData]
    class LuaPlayer
    {
        private Player player;

        public LuaPlayer(Player player)
        {
            this.player = player;
        }

        public void setMusic(ushort musicID, ushort playMode)
        {
            player.playerSession.queuePacket(SetMusicPacket.buildPacket(player.actorId, musicID, playMode), true, false);
        }

        public void setWeather(ushort weatherID)
        {
            player.playerSession.queuePacket(SetWeatherPacket.buildPacket(player.actorId, weatherID, 1), true, false);
        }

        public void getParameter(string paramName)
        {

        }

        public void setParameter(string paramName, object value, string uiToRefresh)
        {
            
        }

        public void getAttributePoints()
        {
            
        }

        public void setAttributePoints(int str, int vit, int dex, int inte, int min, int pie)
        {

        }

        public void logout()
        {
            player.playerSession.queuePacket(LogoutPacket.buildPacket(player.actorId), true, false);
        }

        public void quitGame()
        {
            player.playerSession.queuePacket(QuitPacket.buildPacket(player.actorId), true, false);
        }

        public void runEvent(string functionName, params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.createLuaParamList(parameters);
        //    player.playerSession.queuePacket(RunEventFunctionPacket.buildPacket(player.actorId, player.eventCurrentOwner, player.eventCurrentStarter, functionName, lParams), true, false);
        }

        public void endEvent()
        {
          //  player.playerSession.queuePacket(EndEventPacket.buildPacket(player.actorId, player.eventCurrentOwner, player.eventCurrentStarter), true, false);
        }

    }
}

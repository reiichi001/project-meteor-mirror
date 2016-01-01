using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.events;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.lua
{
    class LuaPlayer
    {
        private ConnectedPlayer player;

        public LuaPlayer(ConnectedPlayer player)
        {
            this.player = player;
        }

        public void setMusic(ushort musicID, ushort playMode)
        {
            player.queuePacket(SetMusicPacket.buildPacket(player.actorID, musicID, playMode), true, false);
        }

        public void setWeather(uint weatherID)
        {
            player.queuePacket(SetWeatherPacket.buildPacket(player.actorID, weatherID), true, false);
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
            player.queuePacket(LogoutPacket.buildPacket(player.actorID), true, false);
        }

        public void quitGame()
        {
            player.queuePacket(QuitPacket.buildPacket(player.actorID), true, false);
        }

        public void runEvent(string functionName, params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.createLuaParamList(parameters);
            player.getConnection1().queuePacket(RunEventFunctionPacket.buildPacket(player.actorID, player.eventCurrentOwner, player.eventCurrentStarter, functionName, lParams), true, false);
        }

        public void endEvent()
        {
            player.getConnection1().queuePacket(EndEventPacket.buildPacket(player.actorID, player.eventCurrentOwner, player.eventCurrentStarter), true, false);
        }

    }
}

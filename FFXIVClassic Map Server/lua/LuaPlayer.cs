using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send;
using MoonSharp.Interpreter;
using System.Collections.Generic;

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

        public void SetMusic(ushort musicID, ushort playMode)
        {
            player.playerSession.QueuePacket(SetMusicPacket.BuildPacket(player.actorId, musicID, playMode), true, false);
        }

        public void SetWeather(ushort weatherID)
        {
            player.playerSession.QueuePacket(SetWeatherPacket.BuildPacket(player.actorId, weatherID, 1), true, false);
        }

        public void GetParameter(string paramName)
        {

        }

        public void SetParameter(string paramName, object value, string uiToRefresh)
        {
            
        }

        public void GetAttributePoints()
        {
            
        }

        public void SetAttributePoints(int str, int vit, int dex, int inte, int min, int pie)
        {

        }

        public void Logout()
        {
            player.playerSession.QueuePacket(LogoutPacket.BuildPacket(player.actorId), true, false);
        }

        public void QuitGame()
        {
            player.playerSession.QueuePacket(QuitPacket.BuildPacket(player.actorId), true, false);
        }

        public void RunEvent(string functionName, params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.CreateLuaParamList(parameters);
        //    player.playerSession.QueuePacket(RunEventFunctionPacket.BuildPacket(player.actorId, player.eventCurrentOwner, player.eventCurrentStarter, functionName, lParams), true, false);
        }

        public void EndEvent()
        {
          //  player.playerSession.QueuePacket(EndEventPacket.BuildPacket(player.actorId, player.eventCurrentOwner, player.eventCurrentStarter), true, false);
        }

    }
}

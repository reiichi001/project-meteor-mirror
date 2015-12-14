using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send
{
    class SetWeatherPacket
    {
        //TODO: Fix these ids!
        public const uint WEATHER_CLEAR = 0x011F41;
		public const uint WEATHER_FINE = 0x011F42;
		public const uint WEATHER_CLOUDY = 0x011F3;
		public const uint WEATHER_FOGGY = 0x011F4;
		public const uint WEATHER_WINDY = 0x011F5;			//NOT SUPPORTED in v1.23
		public const uint WEATHER_BLUSTERY = 0x011F6;
		public const uint WEATHER_RAINY = 0x011F7;
		public const uint WEATHER_SHOWERY = 0x011F8;			//NOT SUPPORTED in v1.23
		public const uint WEATHER_THUNDERY = 0x011F9;			//NOT SUPPORTED in v1.23
		public const uint WEATHER_STORMY = 0x011FA;
		public const uint WEATHER_DUSTY = 0x011FB;				//NOT SUPPORTED in v1.23
		public const uint WEATHER_SANDY = 0x011FC;
		public const uint WEATHER_IFRIT	= 0x011F4E;
		public const uint WEATHER_GARUDA  = 0x011F5C;
		public const uint WEATHER_BLISTERIN = 0x011FD;			//NOT SUPPORTED in v1.23
		public const uint WEATHER_SNOWY = 0x011FE;				//NOT SUPPORTED in v1.23
		public const uint WEATHER_WINTRY = 0x011FF;				//NOT SUPPORTED in v1.23
		public const uint WEATHER_GLOOMY = 0x01200;
		public const uint WEATHER_PREDALAMUD = 0x011F5F;
		public const uint WEATHER_DALAMUD	= 0x011F5E;
		public const uint WEATHER_SCARYDALAMUD = 0x011F60;

        public const ushort OPCODE = 0x000D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, long weatherId)
        {
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(weatherId));
        }
    }
}

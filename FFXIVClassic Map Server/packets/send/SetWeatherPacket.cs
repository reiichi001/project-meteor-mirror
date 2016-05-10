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
        public const ushort WEATHER_CLEAR                 = 8001;
        public const ushort WEATHER_FAIR                  = 8002;
        public const ushort WEATHER_CLOUDY                = 8003;
        public const ushort WEATHER_FOGGY                 = 8004;
        public const ushort WEATHER_WINDY                 = 8005;
        public const ushort WEATHER_BLUSTERY              = 8006;
        public const ushort WEATHER_RAINY                 = 8007;
        public const ushort WEATHER_SHOWERY               = 8008;
        public const ushort WEATHER_THUNDERY              = 8009;
        public const ushort WEATHER_STORMY                = 8010;
        public const ushort WEATHER_DUSTY                 = 8011;
        public const ushort WEATHER_SANDY                 = 8012;
        public const ushort WEATHER_HOT                   = 8013;
        public const ushort WEATHER_BLISTERING            = 8014; //Bowl Of Embers Weather
        public const ushort WEATHER_SNOWY                 = 8015;
        public const ushort WEATHER_WINTRY                = 8016;
        public const ushort WEATHER_GLOOMY                = 8017;
                                                               
        public const ushort WEATHER_SEASONAL              = 8027; //Snow in Black Shroud, nothing elsewhere
        public const ushort WEATHER_PRIMAL                = 8028; //Howling Eye and Thornmarch Weather
        public const ushort WEATHER_SEASONAL_FIREWORKS    = 8029; //Plays fireworks between 20:00 - 21:00 ET
        public const ushort WEATHER_DALAMUD               = 8030;
        public const ushort WEATHER_AURORA                = 8031;
        public const ushort WEATHER_DALAMUD_THUNDER       = 8032;

        public const ushort WEATHER_DAY                   = 8065; //Force skybox to show Day + Fair regardless of current ET
        public const ushort WEATHER_TWILIGHT              = 8066; //Force skybox to show Twilight + Clear regardless of current ET

        public const ushort OPCODE = 0x000D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, ushort weatherId, ushort transitionTime)
        {
            ulong combined = (ulong)(weatherId | (transitionTime << 16));
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(combined));
        }
    }
}

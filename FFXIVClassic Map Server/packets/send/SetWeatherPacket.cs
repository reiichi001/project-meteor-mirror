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
        // Use the first value to change without a transition, the second value to do a standard transition
        public const uint WEATHER_CLEAR             = 0x011F41; // 8001 / 73537
        public const uint WEATHER_FINE              = 0x011F42; // 8002 / 73538
        public const uint WEATHER_CLOUDY            = 0x011F43; // 8003 / 73539
        public const uint WEATHER_FOGGY             = 0x011F44; // 8004 / 73540
        public const uint WEATHER_WINDY             = 0x011F45; // 8005 / 73541 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_BLUSTERY          = 0x011F46; // 8006 / 73542
        public const uint WEATHER_RAINY             = 0x011F47; // 8007 / 73543
        public const uint WEATHER_SHOWERY           = 0x011F48; // 8008 / 73544 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_THUNDERY          = 0x011F49; // 8009 / 73545 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_STORMY            = 0x011F4A; // 8010 / 73546
        public const uint WEATHER_DUSTY             = 0x011F4B; // 8011 / 73547 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_SANDY             = 0x011F4C; // 8012 / 73548
        public const uint WEATHER_HOT               = 0x011F4D; // 8013 / 73549 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_BLISTERING        = 0x011F4E; // 8014 / 73550 - Bowl Of Embers Weather
        public const uint WEATHER_SNOWY             = 0x011F4F; // 8015 / 73551 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_WINTRY            = 0x011F50; // 8016 / 73552 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_GLOOMY            = 0x011F51; // 8017 / 73553
                                                                // 8018 - 8026 / 73554 - 73562 - NOT SUPPORTED in v1.23b
        public const uint WEATHER_SEASONAL          = 0x011F5B; // 8027 / 73563 - Snow in Black Shroud, nothing elsewhere
        public const uint WEATHER_PRIMAL            = 0x011F5C; // 8028 / 73564 - Howling Eye and Thornmarch Weather
                                                                // 8029 / 73565 - Not supported
        public const uint WEATHER_DALAMUD           = 0x011F5E; // 8030 / 73566
        public const uint WEATHER_AURORA            = 0x011F5F; // 8031 / 73567
        public const uint WEATHER_DALAMUDTHUNDER	= 0x011F60; // 8032 / 73568

        public const ushort OPCODE = 0x000D;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket buildPacket(uint playerActorID, long weatherId)
        {
            return new SubPacket(OPCODE, 0, playerActorID, BitConverter.GetBytes(weatherId));
        }
    }
}

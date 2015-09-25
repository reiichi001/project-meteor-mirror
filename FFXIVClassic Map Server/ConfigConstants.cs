using FFXIVClassic_Lobby_Server.common;
using STA.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server
{
    class ConfigConstants
    {
        public static String OPTIONS_BINDIP;
        public static bool   OPTIONS_TIMESTAMP = false;

        public static uint   DATABASE_WORLDID;
        public static String DATABASE_HOST;
        public static String DATABASE_PORT;
        public static String DATABASE_NAME;
        public static String DATABASE_USERNAME;
        public static String DATABASE_PASSWORD;

        public static bool load()
        {
            Console.Write("Loading config.ini file... ");

            if (!File.Exists("./config.ini"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[FILE NOT FOUND]");
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }

            INIFile configIni = new INIFile("./config.ini");

            ConfigConstants.OPTIONS_BINDIP =        configIni.GetValue("General", "server_ip", "127.0.0.1");
            ConfigConstants.OPTIONS_TIMESTAMP =     configIni.GetValue("General", "showtimestamp", "true").ToLower().Equals("true");

            ConfigConstants.DATABASE_WORLDID =      configIni.GetValue("Database", "worldid", (uint)0);
            ConfigConstants.DATABASE_HOST =         configIni.GetValue("Database", "host", "");
            ConfigConstants.DATABASE_PORT =         configIni.GetValue("Database", "port", "");
            ConfigConstants.DATABASE_NAME =         configIni.GetValue("Database", "database", "");
            ConfigConstants.DATABASE_USERNAME =     configIni.GetValue("Database", "username", "");
            ConfigConstants.DATABASE_PASSWORD =     configIni.GetValue("Database", "password", "");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ForegroundColor = ConsoleColor.Gray;

            return true;
        }
    }
}

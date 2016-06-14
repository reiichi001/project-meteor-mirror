using FFXIVClassic.Common;
using System;
using System.IO;

namespace FFXIVClassic_Lobby_Server
{
    class ConfigConstants
    {
        public static String OPTIONS_BINDIP;
        public static String OPTIONS_PORT;
        public static bool   OPTIONS_TIMESTAMP = false;

        public static String DATABASE_HOST;
        public static String DATABASE_PORT;
        public static String DATABASE_NAME;
        public static String DATABASE_USERNAME;
        public static String DATABASE_PASSWORD;

        public static bool load()
        {
            Console.Write("Loading lobby_config.ini file... ");

            if (!File.Exists("./lobby_config.ini"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(String.Format("[FILE NOT FOUND]"));
                Console.ForegroundColor = ConsoleColor.Gray;
                return false;
            }

            INIFile configIni = new INIFile("./lobby_config.ini");

            ConfigConstants.OPTIONS_BINDIP =        configIni.GetValue("General", "server_ip", "127.0.0.1");
            ConfigConstants.OPTIONS_PORT =          configIni.GetValue("General", "server_port", "54994");
            ConfigConstants.OPTIONS_TIMESTAMP =     configIni.GetValue("General", "showtimestamp", "true").ToLower().Equals("true");

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

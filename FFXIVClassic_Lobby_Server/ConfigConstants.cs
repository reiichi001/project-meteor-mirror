using FFXIVClassic_Lobby_Server.common;
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
        public static bool OPTIONS_TIMESTAMP = false;

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
 
            IniFile ini = new IniFile("./config.ini");

            ConfigConstants.OPTIONS_TIMESTAMP = ini.IniReadValue("General", "showtimestamp").ToLower().Equals("true");

            ConfigConstants.DATABASE_HOST = ini.IniReadValue("Database", "host");
            ConfigConstants.DATABASE_PORT = ini.IniReadValue("Database", "port");
            ConfigConstants.DATABASE_NAME = ini.IniReadValue("Database", "database");
            ConfigConstants.DATABASE_USERNAME = ini.IniReadValue("Database", "username");
            ConfigConstants.DATABASE_PASSWORD = ini.IniReadValue("Database", "password");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
            Console.ForegroundColor = ConsoleColor.Gray;

            return true;
        }
    }
}

using System;
using System.IO;

namespace FFXIVClassic_Lobby_Server.common
{
    class Log
    {
        public enum LogType
        {
            Error = ConsoleColor.Red,
            Debug = ConsoleColor.Yellow,
            Info = ConsoleColor.Cyan,
            Sql = ConsoleColor.Magenta,
            Conn = ConsoleColor.Green,
            Default = ConsoleColor.Gray
        }

        public static void error(String message)
        {
            log(message, LogType.Error);
        }

        public static void debug(String message)
        {
            #if DEBUG
                log(message, LogType.Debug);
            #endif
        }

        public static void info(String message)
        {
            log(message, LogType.Info);
        }

        public static void database(String message)
        {
            log(message, LogType.Sql);
        }

        public static void conn(String message)
        {
            log(message, LogType.Conn);
        }

        public static void log(String message, LogType type)
        {
            var timestamp = String.Format("[{0}] ", DateTime.Now.ToString("dd/MMM HH:mm:ss"));
            var typestr = String.Format("[{0}] ", type.ToString().ToUpper());

            Console.Write(timestamp);
            Console.ForegroundColor = (ConsoleColor)type;
            Console.Write(typestr);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);

            message = message.Insert(0, typestr);
            message = message.Insert(0, timestamp);

            Directory.CreateDirectory(ConfigConstants.OPTIONS_LOGPATH);

            try
            {
                File.AppendAllText(ConfigConstants.OPTIONS_LOGPATH + ConfigConstants.OPTIONS_LOGFILE, message + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.common
{
    class Log
    {
        public enum LogType
        {
            Status = ConsoleColor.Green,
            Sql = ConsoleColor.Magenta,
            Info = ConsoleColor.White,
            Debug = ConsoleColor.Cyan,
            Error = ConsoleColor.Red
        }

        public static void Status(String message)
        {
            LogFile(message, LogType.Status);
        }

        public static void Sql(String message)
        {
            LogFile(message, LogType.Sql);
        }

        public static void Info(String message)
        {
            LogFile(message, LogType.Info);
        }

        public static void Debug(String message)
        {
#if DEBUG
            LogFile(message, LogType.Debug);
#endif
        }

        public static void Error(String message)
        {
            LogFile(message, LogType.Error);
        }

        private static void LogFile(String message, LogType type)
        {
            string timestamp = String.Format("[{0}]", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            string messageType = String.Format("[{0}] ", type.ToString().ToUpper());

            Console.Write(timestamp);
            Console.ForegroundColor = (ConsoleColor)type;
            Console.Write(messageType);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("{0}{1}{2}", timestamp, messageType, message));

            if (!Directory.Exists(ConfigConstants.OPTIONS_LOGPATH))
            {
                Directory.CreateDirectory(ConfigConstants.OPTIONS_LOGPATH);
            }

            using (FileStream fs = new FileStream(Path.Combine(ConfigConstants.OPTIONS_LOGPATH, ConfigConstants.OPTIONS_LOGFILE), FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(sb.ToString());
            }
        }
    }
}
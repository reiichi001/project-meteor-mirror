using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace FFXIVClassic.Common
{
    public class Log
    {
        public string LogDirectory;
        public string LogFileName;
        public int EnabledLogTypes;
        public Queue<Tuple<String, LogColour>> LogQueue;

        public Log(string path, string file, int enabledtypes)
        {
            LogQueue = new Queue<Tuple<String, LogColour>>();
            EnabledLogTypes = enabledtypes;
            LogDirectory = path;
            LogFileName = file;
        }

        public enum LogType
        {
            None    = 0x000,
            Console = 0x001,
            File    = 0x002,
            Status  = 0x004,
            Sql     = 0x008,
            Info    = 0x010,
            Debug   = 0x020,
            Error   = 0x040,
        }

        public enum LogColour
        {
            Status = ConsoleColor.Green,
            Sql = ConsoleColor.Magenta,
            Info = ConsoleColor.White,
            Debug = ConsoleColor.Cyan,
            Error = ConsoleColor.Red
        }

        public void Status(String message, params object[] formatargs)
        {
            if (formatargs.Length > 0)
                message = String.Format(message, formatargs);

            QueueMessage(message, LogColour.Status);
        }

        public void Sql(String message, params object[] formatargs)
        {
            if (formatargs.Length > 0)
                message = String.Format(message, formatargs);

            QueueMessage(message, LogColour.Sql);
        }

        public void Info(String message, params object[] formatargs)
        {
            if (formatargs.Length > 0)
                message = String.Format(message, formatargs);

            QueueMessage(message, LogColour.Info);
        }

        public void Debug(String message, params object[] formatargs)
        {
            if (formatargs.Length > 0)
                message = String.Format(message, formatargs);

            QueueMessage(message, LogColour.Debug);
        }

        public void Error(String message, params object[] formatargs)
        {
            if (formatargs.Length > 0)
                message = String.Format(message, formatargs);

            QueueMessage(message, LogColour.Error);
        }

        public void Packet(String message, params object[] formatargs)
        {

        }

        private void QueueMessage(String message, LogColour type)
        {
            LogQueue.Enqueue(Tuple.Create(message, type));
        }

        public void WriteMessage(String message, LogColour type)
        {
            var canLog = false;

            try 
                {
                    canLog = ((EnabledLogTypes & (int)(Enum.Parse(typeof(LogType), type.ToString()))) != 0);
                }
            catch (Exception  e) { }

            string timestamp = String.Format("[{0}]", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            string messageType = String.Format("[{0}] ", type.ToString().ToUpper());

            if ((EnabledLogTypes & (int)LogType.Console) != 0)
            {
                Console.Write(timestamp);
                Console.ForegroundColor = (ConsoleColor)type;
                Console.Write(messageType);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("{0}{1}{2}", timestamp, messageType, message));

            if ((EnabledLogTypes & (int)LogType.File) != 0)
            {
                // todo: add param to see if path has been changed during runtime and then check directory/file
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                using (FileStream fs = new FileStream(Path.Combine(LogDirectory, LogFileName), FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
        }
    }
}
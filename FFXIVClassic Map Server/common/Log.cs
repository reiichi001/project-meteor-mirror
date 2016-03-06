using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server.common
{
    class Log
    {
        public static void error(String message)
        {
            Console.Write("[{0}]", DateTime.Now.ToString("dd/MMM HH:mm"));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.Gray ;
            Console.WriteLine(message);
        }

        public static void debug(String message)
        {         
#if DEBUG
            Console.Write("[{0}]", DateTime.Now.ToString("dd/MMM HH:mm"));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[DEBUG] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
#endif
        }

        public static void info(String message)
        {
            Console.Write("[{0}]", DateTime.Now.ToString("dd/MMM HH:mm"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[INFO] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        public static void database(String message)
        {
            Console.Write("[{0}]", DateTime.Now.ToString("dd/MMM HH:mm"));
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[SQL] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }

        public static void conn(String message)
        {
            Console.Write("[{0}]", DateTime.Now.ToString("dd/MMM HH:mm"));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[CONN] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
        }
    }
}

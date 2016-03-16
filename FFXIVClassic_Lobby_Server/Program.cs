using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Diagnostics;
using System.Threading;
using FFXIVClassic_Lobby_Server.common;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace FFXIVClassic_Lobby_Server
{
    class Program
    {

        static void Main(string[] args)
        {
#if DEBUG
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
#endif

            bool startServer = true;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------FFXIV 1.0 Lobby Server--------");            
            Console.ForegroundColor = ConsoleColor.Gray;


            Assembly assem = Assembly.GetExecutingAssembly();
            Version vers = assem.GetName().Version;
            Console.WriteLine("Version: " + vers.ToString());

            //Load Config
            if (!ConfigConstants.load())
                startServer = false;

            //Test DB Connection
            Console.Write("Testing DB connection to \"{0}\"... ", ConfigConstants.DATABASE_HOST);
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    conn.Close();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[OK]");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                catch (MySqlException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[FAILED]");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    startServer = false; 
                }
            }

            //Start Server if A-OK
            if (startServer)
            {
                Server server = new Server();
                server.startServer();

                while (true) Thread.Sleep(10000);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

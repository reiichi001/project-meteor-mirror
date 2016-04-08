using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Diagnostics;
using System.Threading;
using FFXIVClassic_Lobby_Server.common;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.IO;
using FFXIVClassic_Lobby_Server.dataobjects;
using System.Collections.Generic;
using System.Text;

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

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------FFXIV 1.0 Map Server---------");            
            Console.ForegroundColor = ConsoleColor.Gray;


            Assembly assem = Assembly.GetExecutingAssembly();
            Version vers = assem.GetName().Version;
            Console.WriteLine("Version: " + vers.ToString());

            //Load Config
            if (!ConfigConstants.load())
                startServer = false;

            //Test DB Connection
            Console.Write("Testing DB connection... ");
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

            //Check World ID
            DBWorld thisWorld = Database.getServer(ConfigConstants.DATABASE_WORLDID);
            if (thisWorld != null)
                Console.WriteLine("Successfully pulled world info from DB. Server name is {0}.", thisWorld.name);
            else
                Console.WriteLine("World info could not be retrieved from the DB. Welcome and MOTD will not be displayed.");

            //Start server if A-OK
            if (startServer)
            {
                Server server = new Server();
                CommandProcessor cp = new CommandProcessor(server.getConnectedPlayerList());
                server.startServer();

                while (true)
                {
                    String input = Console.ReadLine();
                    cp.doCommand(input, null);  
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

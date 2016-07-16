using System;
using System.Diagnostics;
using System.Threading;
using System.Text;
using MySql.Data.MySqlClient;
using System.Reflection;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic.Common;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Config;

namespace FFXIVClassic_Map_Server
{
    class Program
    {
        public static Logger Log;

        static void Main(string[] args)
        {

            // set up logging
            Log = LogManager.GetCurrentClassLogger();
#if DEBUG
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
#endif
            bool startServer = true;

            Program.Log.Info("---------FFXIV 1.0 Map Server---------");

            //Load Config
            if (!ConfigConstants.Load())
                startServer = false;

            //Test DB Connection
            Program.Log.Info("Testing DB connection... ");
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    conn.Close();

                    Program.Log.Info("Connection ok.");
                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                    startServer = false; 
                }
            }

            //Check World ID
            DBWorld thisWorld = Database.GetServer(ConfigConstants.DATABASE_WORLDID);
            if (thisWorld != null)
                Program.Log.Info("Successfully pulled world info from DB. Server name is {0}.", thisWorld.name);
            else
                Program.Log.Info("World info could not be retrieved from the DB. Welcome and MOTD will not be displayed.");

            //Start server if A-OK
            if (startServer)
            {
                Server server = new Server();
                CommandProcessor cp = new CommandProcessor(server.GetConnectedPlayerList());
                server.StartServer();

                while (startServer)
                {
                    String input = Console.ReadLine();
                    Log.Info("[Console Input] " + input);
                    cp.DoCommand(input, null);
                }
            }

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

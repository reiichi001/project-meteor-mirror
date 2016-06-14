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
#if DEBUG
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
#endif
            bool startServer = true;

            //Load Config
            if (!ConfigConstants.load())
                startServer = false;

            // set up logging

            Log = LogManager.GetCurrentClassLogger();

            Program.Log.Info("---------FFXIV 1.0 Map Server---------");            

            Assembly assem = Assembly.GetExecutingAssembly();
            Version vers = assem.GetName().Version;
            Program.Log.Info("Version: " + vers.ToString());

            //Test DB Connection
            Program.Log.Info("Testing DB connection... ");
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    conn.Close();

                    Program.Log.Info("[OK]");
                }
                catch (MySqlException e)
                {
                    Program.Log.Error(e.ToString());
                    startServer = false; 
                }
            }

            //Check World ID
            DBWorld thisWorld = Database.getServer(ConfigConstants.DATABASE_WORLDID);
            if (thisWorld != null)
                Program.Log.Info("Successfully pulled world info from DB. Server name is {0}.", thisWorld.name);
            else
                Program.Log.Info("World info could not be retrieved from the DB. Welcome and MOTD will not be displayed.");

            //Start server if A-OK
            if (startServer)
            {
                Server server = new Server();
                CommandProcessor cp = new CommandProcessor(server.getConnectedPlayerList());
                server.startServer();

                while (startServer)
                {
                    String input = Console.ReadLine();
                    Log.Info("[Console Input] " + input);
                    cp.doCommand(input, null);  
                }
            }

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

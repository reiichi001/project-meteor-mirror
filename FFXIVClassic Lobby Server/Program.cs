using System;
using System.Diagnostics;
using System.Threading;
using MySql.Data.MySqlClient;
using NLog;
namespace FFXIVClassic_Lobby_Server
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
            Log.Info("==================================");
            Log.Info("FFXIV Classic Lobby Server");
            Log.Info("Version: 0.1");            
            Log.Info("==================================");

            bool startServer = true;

            //Load Config
            ConfigConstants.Load();
            ConfigConstants.ApplyLaunchArgs(args);
            
            //Test DB Connection
            Program.Log.Info("Testing DB connection to \"{0}\"... ", ConfigConstants.DATABASE_HOST);
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

            //Start Server if A-OK
            if (startServer)
            {
                Server server = new Server();
                server.StartServer();
                while (true) Thread.Sleep(10000);
            }

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

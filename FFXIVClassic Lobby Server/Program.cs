using System;
using System.Diagnostics;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Reflection;
using FFXIVClassic_Lobby_Server.common;

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

            //Load Config
            if (!ConfigConstants.load())
                startServer = false;

            Log.Info("--------FFXIV 1.0 Lobby Server--------");


            Assembly assem = Assembly.GetExecutingAssembly();
            Version vers = assem.GetName().Version;
            Log.Info("Version: " + vers.ToString());

            //Test DB Connection
            Log.Info(String.Format("Testing DB connection to \"{0}\"... ", ConfigConstants.DATABASE_HOST));
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    conn.Close();

                    Log.Status("[OK]");
                }
                catch (MySqlException e)
                {
                    Log.Error(e.ToString());
                    Log.Error("[FAILED]");

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

            Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

using System;
using System.Diagnostics;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Reflection;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server
{
    class Program
    {
        public static Log Log;

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

            Log = new Log(ConfigConstants.OPTIONS_LOGPATH, ConfigConstants.OPTIONS_LOGFILE, Int32.Parse(ConfigConstants.OPTIONS_LOGLEVEL));

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    if (Log.LogQueue.Count > 0)
                    {
                        var message = Program.Log.LogQueue.Dequeue();
                        Program.Log.WriteMessage(message.Item1, message.Item2);
                    }
                }
            });
            thread.Start();

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

                    Program.Log.Status("[OK]");
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
                Program.Log.Info(String.Format("Successfully pulled world info from DB. Server name is {0}.", thisWorld.name));
            else
                Program.Log.Info("World info could not be retrieved from the DB. Welcome and MOTD will not be displayed.");

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

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

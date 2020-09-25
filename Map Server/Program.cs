/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using NLog;

namespace Meteor.Map
{
    class Program
    {
        public static Logger Log;
        public static Server Server;
        public static Random Random;
        public static DateTime LastTick = DateTime.Now;
        public static DateTime Tick = DateTime.Now;

        static void Main(string[] args)
        {
            // set up logging
            Log = LogManager.GetCurrentClassLogger();
#if DEBUG
            TextWriterTraceListener myWriter = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(myWriter);
#endif
            bool startServer = true;

            Log.Info("==================================");
            Log.Info("Project Meteor: Map Server");
            Log.Info("Version: 0.1");
            Log.Info("==================================");

            //Load Config
            ConfigConstants.Load();
            ConfigConstants.ApplyLaunchArgs(args);

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
            
            //Start server if A-OK
            if (startServer)
            {
                Random = new Random();
                Server = new Server();
                Tick = DateTime.Now;
                Server.StartServer();

                while (startServer)
                {
                    String input = Console.ReadLine();
                    Log.Info("[Console Input] " + input);
                    Server.GetCommandProcessor().DoCommand(input, null);
                }
            }

            Program.Log.Info("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

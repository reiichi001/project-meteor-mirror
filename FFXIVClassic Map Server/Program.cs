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

            Utils.FFXIVLoginStringDecodeBinary("C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV\\ffxivlogin.exe");

            Console.WriteLine(Utils.FFXIVLoginStringDecode(new byte[]{0x6F, 0xD6, 0x3C, 0xD6, 0x20, 0x81, 0x3F, 0x06, 0x36, 0x78, 0xD3, 0xAE, 0xDB, 0x4E, 0x08, 0xF1, 0x7D, 0xAE, 0x90, 0x43, 0x18, 0x70, 0x32, 0x08, 0x6B, 0x75, 0x98, 0xA1, 0x51, 0x15, 0xA9, 0xF7, 0x74, 0xB3, 0x6F, 0x10, 0xEA, 0x76, 0x34, 0x0B, 0x7E, 0x2D, 0xD2, 0xAC, 0xD7, 0xC3, 0xD3, 0xC1, 0x4D, 0x96, 0xED, 0xD4, 0xCC, 0x5E, 0x0D, 0xF5, 0x7E, 0x35, 0x99, 0xB9, 0x57, 0x38, 0x51, 0x79, 0x39, 0x3F, 0x08, 0xFB, 0xE8, 0xEE, 0x25, 0x4F, 0xAE, 0xE2, 0xFC, 0x7E, 0x2A, 0x72, 0x34, 0x57, 0x7E}));

            Console.WriteLine(Utils.ByteArrayToHex(Utils.FFXIVLoginStringEncode(0xd018, "http://141.117.163.26/login/")));

            return;

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
                server.startServer();

                while (true)
                {
                    String input = Console.ReadLine();
                    server.doCommand(input, null);  
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

    
    }
}

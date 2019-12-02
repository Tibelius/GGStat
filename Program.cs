using System;
using System.Diagnostics;
using System.Net.Http;

namespace GGStat
{
    class Program
    {
        public readonly static bool DEBUG = true;
        public static Process process;
        public static string PROCESS_NAME = "GuiltyGearXrd";

        private static readonly HttpClient client = new HttpClient();
        private static string STEAM_API_KEY = "1D514CA9A5AE546E2E0C63D63A83B7C3"; //pls no abuse
        private static string STEAM_ID = "76561197993307624"; // Owner steam ID for matching Steam name to GG game name
        static void Main(string[] args) {
            string[] all = System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceNames();
            foreach (string one in all) {
                Console.WriteLine(one);
            }


            Data.initDB();
            process = getProcess();
            while (!isConnected())
            {
                Console.WriteLine("Finding GuiltyGearXrd process...");
                System.Threading.Thread.Sleep(1000);
            }
            Console.Clear();
            do
            {
                Game game = new Game();
                game.Run();
            } while (true);
        }

        private static Process getProcess()
        {
            Process[] processes = Process.GetProcessesByName(PROCESS_NAME);
            if (processes.Length > 0)
            {
                return processes[0];
            } else { return null; }
        }

        public static bool isConnected()
        {
            Process[] processes = Process.GetProcessesByName(PROCESS_NAME);
            return processes.Length != 0;
        }
    }
}

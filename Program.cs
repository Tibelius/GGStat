using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GGStat
{
    class Program
    {
        public readonly static bool DEBUG = true;
        public static Process process;
        public static string PROCESS_NAME = "GuiltyGearXrd";

        static void Main(string[] args) {
            string[] all = System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceNames();
            foreach (string one in all) {
                Console.WriteLine(one);
            }


            Data.initDB();
            Process[] processes = new Process[] { };
            while (processes.Length == 0) {
                processes = Process.GetProcessesByName(PROCESS_NAME);
                Console.WriteLine("Finding GuiltyGearXrd process...");
                System.Threading.Thread.Sleep(2000);
            }
            process = processes[0];
            Game game = new Game();
            Console.Clear();
            game.Run();
        }
    }

    internal class Form1 : Form
    {
    }
}

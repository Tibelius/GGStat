using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GGStat
{
    class Program
    {
        static void Main(string[] args) {
            string[] all = System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceNames();
            foreach (string one in all) {
                Console.WriteLine(one);
            }
            Game game = new Game();
            int[] players = new int[2] { 0, 0 };
            do {
                //Console.Clear();
                game.refreshAllData();
                game.vision.FindPlayers(ref players);
                Console.WriteLine(Game.Character[players[0]] + " VS " + Game.Character[players[1]]);
                System.Threading.Thread.Sleep(1000);
            } while (true);
        }
    }

    internal class Form1 : Form
    {
    }
}

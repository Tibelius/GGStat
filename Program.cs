using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using static GGStat.Data;

namespace GGStat {
    class Program {
        public static Form1 UI;
        public static Thread gameThread;
        public static bool DEBUG => IsDebug() ? true : false;

        private static bool debugMode;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            gameThread = new Thread(GameThread.game);
            gameThread.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UI = new Form1();
            Application.Run(UI);
        }

        delegate void LogCallback(string text);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void Log(string text) {
            if (UI != null) {
                var ctrls = UI.Controls.Find("logBox", true);
                if (ctrls.Length > 0) {
                    TextBox log = (TextBox) ctrls[0];
                    // InvokeRequired required compares the thread ID of the 
                    // calling thread to the thread ID of the creating thread. 
                    // If these threads are different, it returns true. 
                    if (log.InvokeRequired) {
                        LogCallback d = new LogCallback(Log);
                        UI.Invoke(d, new object[] { text });
                    } else {
                        log.AppendText(text);
                        log.AppendText(Environment.NewLine);
                    }
                }
            }
        }

        private static bool IsDebug() {
            SetDebug();
            return debugMode;
        }

        [Conditional("DEBUG")]
        private static void SetDebug() {
            debugMode = true;
        }
    }

    public class GameThread {
        public static Process process;
        public static string PROCESS_NAME = "GuiltyGearXrd";

        //private static readonly HttpClient client = new HttpClient();

        public static void game() {
            string[] all = System.Reflection.Assembly.GetEntryAssembly().
                GetManifestResourceNames();
            foreach (string one in all) {
                Program.Log(one);
            }

            if (Program.DEBUG) {
                TestData.RecreateDBFile();
                TestData.GenerateTestData();
            } else {
                Data.InitDB();
            }

            Program.UI.SetTableData(getAllMatchesWithPlayer(new Player(1)).ToArray());

            
            process = getProcess();
            while (!isConnected()) {
                Program.Log("Finding GuiltyGearXrd process...");
                System.Threading.Thread.Sleep(1000);
            }

            do {
                Game game = new Game();
                game.Run();
            } while (true);
        }

        private static Process getProcess() {
            Process[] processes = Process.GetProcessesByName(PROCESS_NAME);
            if (processes.Length > 0) {
                return processes[0];
            } else { return null; }
        }

        public static bool isConnected() {
            Process[] processes = Process.GetProcessesByName(PROCESS_NAME);
            return processes.Length != 0;
        }
    }
}

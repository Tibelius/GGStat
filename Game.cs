using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GGStat
{
    class Game
    {
        public static string[] Character = new string[]
        {
            "unknown",
            "sol",
            "ky",
            //"millia",
            //"zato",
            //"may",
            //"potemkin",
            //"chipp",
            //"venom",
            //"axl",
            //"i-no",
            //"faust",
            //"slayer",
            "ramlethal",
            //"bedman",
            //"sin",
            "elphelt",
            //"leo",
            //"johnny",
            //"jack-o",
            "jam",
            "raven",
            "kum",
            //"dizzy",
            //"baiken",
            //"answer"
        };

        private const int PROCESS_WM_READ = 0x0010;
        private const string PROCESS_NAME = "GuiltyGearXrd";

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        Int64 lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public Vision vision;
        public Int32 BASEADDRESS;
        public IntPtr processHandle;

        private Dictionary<string, int[]> dataList;
        public Dictionary<string, int> valueList;

        public Game() {
            Process[] processes = new Process[] { };
            while (processes.Length == 0) {
                processes = Process.GetProcessesByName(PROCESS_NAME);
                Console.WriteLine("Finding GuiltyGearXrd");
                System.Threading.Thread.Sleep(1000);
            }
            Process process = processes[0];
            BASEADDRESS = process.MainModule.BaseAddress.ToInt32();
            processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            vision = new Vision(process.MainWindowHandle);

            int[] gameStateOffsets = { 0x009C0DEC, 0x0 };
            int[] player1RoundWinsOffsets = { 0x009B3C44, 0x0 };
            int[] player2RoundWinsOffsets = { 0x009B3C44, 0x4 };
            int[] player1HP = { 0x01B18C78, 0x9CC };
            int[] player2HP = { 0x01B18C7C, 0x9CC };

            //int[] timerOffsets = { 0x0167552C, 0x50, 0xAD4, 0x708 };
            dataList = new Dictionary<string, int[]> {
                {"gameState", gameStateOffsets},
                {"player1Rounds", player1RoundWinsOffsets},
                {"player2Rounds", player2RoundWinsOffsets},
                {"player1HP", player1HP},
                {"player2HP", player2HP},
            };

            valueList = new Dictionary<string, int> {
                {"gameState", 4},
                {"player1Rounds", 0},
                {"player2Rounds", 0},
                {"player1HP", 420},
                {"player2HP", 420},
            };

        }

        public void refreshAllData() {
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            foreach (KeyValuePair<string, int[]> item in dataList) {
                Int32 value = BASEADDRESS;
                foreach (int offset in item.Value) {
                    Int64 address = value + offset;
                    ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
                    value = BitConverter.ToInt32(buffer, 0);
                }
                valueList[item.Key] = value;
            }
            foreach (KeyValuePair<string, int> item in valueList) {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
        }
    }
}
